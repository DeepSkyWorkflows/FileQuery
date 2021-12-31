// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FileQueryDatabase.Database;
using FileQueryDatabase.Tokens;

namespace FileQueryDatabase.Query
{
    /// <summary>
    /// Parses the filter into an expression tree for queries.
    /// </summary>
    public class QueryParser : IQueryParser
    {
        private readonly ITokenizer tokenizer;
        private readonly IOperationParser operationParser;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryParser"/> class.
        /// </summary>
        /// <param name="tokenizer">The tokenizer to break down the command parts.</param>
        /// <param name="operationParser">The parser to transform tokens into operations.</param>
        public QueryParser(ITokenizer tokenizer, IOperationParser operationParser)
        {
            this.tokenizer = tokenizer;
            this.operationParser = operationParser;
        }

        private static Expression<Func<ExtendedProperty, ExtendedProperty, bool>> Contains => (str, pat) => str.Contains(pat);
        private static Expression<Func<ExtendedProperty, ExtendedProperty, bool>> StartsWith => (str, pat) => str.StartsWith(pat);
        private static Expression<Func<ExtendedProperty, ExtendedProperty, bool>> EndsWith => (str, pat) => str.EndsWith(pat);

        /// <summary>
        /// Performs the actual parser work.
        /// </summary>
        /// <param name="command">The command to parse.</param>
        /// <param name="db">The <see cref="FileDatabase"/> instance to use.</param>
        /// <param name="add">A value indicating whether the filter should be added to the existing one.</param>
        /// <returns>The parsed expression.</returns>
        public Expression<Func<FileNode, bool>> Parse(string command, FileDatabase db, bool add)
        {
            var tokens = tokenizer.Tokenize(command);
            var operation = operationParser.ParseTokens(tokens, db);
            ConsoleManager.ShowMessage(operation.ToString());
            var context = new ParserContext(db);
            var expression = ParseOperation(operation, context);

            if (add && db.Filter != null)
            {
                var existingFilter = Expression.Invoke(db.Filter, context.FileNodeParameter);
                var newExpression = Expression.AndAlso(existingFilter, expression);
                expression = newExpression;
            }

            return Expression.Lambda<Func<FileNode, bool>>(expression, context.FileNodeParameter);
        }

        private Expression ParseOperation(Operation operation, ParserContext ctx)
        {
            Expression<Func<ExtendedProperty, ExtendedProperty, bool>> strOp = null;
            switch (operation.Op)
            {
                case Operations.None:
                    return Expression.Empty();

                case Operations.And:
                    return Expression.AndAlso(
                        ParseOperation(operation.Left, ctx),
                        ParseOperation(operation.Right, ctx)).WithTag("&&");

                case Operations.Or:
                    return Expression.OrElse(
                        ParseOperation(operation.Left, ctx),
                        ParseOperation(operation.Right, ctx)).WithTag("||");

                case Operations.ColumnName:
                    var column = operation.Value.ToString();
                    Expression<Func<FileNode, ExtendedProperty>> colSource =
                        node => node[column];
                    var returnTarget = Expression.Label(typeof(ExtendedProperty));
                    var expr = Expression.Block(
                        Expression.Return(
                            returnTarget,
                            Expression.Invoke(colSource.WithTag($"\"{column}\""), ctx.FileNodeParameter)),
                        Expression.Label(returnTarget, Expression.Constant(ExtendedProperty.NULL, typeof(ExtendedProperty)).WithTag("NULL")));
                    return expr.WithTag($"\"{column}\"");

                case Operations.Constant:
                    return Expression.Constant(operation.Value)
                        .WithTag(operation.Value is ExtendedProperty ep ?
                        ep.ValueToString : operation.Value.ToString());

                case Operations.Equal:
                    return Expression.Equal(
                        ParseOperation(operation.Left, ctx),
                        ParseOperation(operation.Right, ctx)).WithTag("==");

                case Operations.NotEqual:
                    return Expression.Equal(
                        ParseOperation(operation.Left, ctx),
                        ParseOperation(operation.Right, ctx)).WithTag("==");

                case Operations.GreaterThan:
                    return Expression.GreaterThan(
                        ParseOperation(operation.Left, ctx),
                        ParseOperation(operation.Right, ctx)).WithTag(">");

                case Operations.GreaterThanOrEqual:
                    return Expression.GreaterThanOrEqual(
                        ParseOperation(operation.Left, ctx),
                        ParseOperation(operation.Right, ctx)).WithTag(">=");

                case Operations.LessThan:
                    return Expression.LessThan(
                        ParseOperation(operation.Left, ctx),
                        ParseOperation(operation.Right, ctx)).WithTag("<");

                case Operations.LessThanOrEqual:
                    return Expression.Equal(
                        ParseOperation(operation.Left, ctx),
                        ParseOperation(operation.Right, ctx)).WithTag("<=");

                case Operations.Contains:
                case Operations.StartsWith:
                case Operations.EndsWith:
                    strOp = operation.Op == Operations.Contains ? Contains.WithTag("contains") :
                    (operation.Op == Operations.StartsWith ? StartsWith.WithTag("startsWith") :
                    EndsWith.WithTag("endsWith"));
                    var strReturnTarget = Expression.Label(typeof(bool));
                    var left = ParseOperation(operation.Left, ctx);
                    var right = ParseOperation(operation.Right, ctx);
                    return Expression.Block(
                        Expression.Return(
                            strReturnTarget,
                            Expression.Invoke(
                                strOp,
                                new[] { left, right }),
                            typeof(bool)),
                        Expression.Label(strReturnTarget, Expression.Constant(false, typeof(bool))))
                            .WithTag($"{TagManager.GetTag(left)} {operation.Op} {TagManager.GetTag(right)}");

                default:
                    throw new InvalidOperationException($"Unable to process: {operation}.");
            }
        }
    }
}
