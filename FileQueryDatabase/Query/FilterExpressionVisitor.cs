// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FileQueryDatabase.Database;

namespace FileQueryDatabase.Query
{
    /// <summary>
    /// Helper for printing expression trees.
    /// </summary>
    public class FilterExpressionVisitor
    {
        private readonly IDictionary<ExpressionType, string> expressions =
            new Dictionary<ExpressionType, string>()
            {
                { ExpressionType.And, "&&" },
                { ExpressionType.AndAlso, "&&" },
                { ExpressionType.Equal, "==" },
                { ExpressionType.GreaterThanOrEqual, ">=" },
                { ExpressionType.GreaterThan, ">" },
                { ExpressionType.LessThan, "<" },
                { ExpressionType.LessThanOrEqual, "<=" },
                { ExpressionType.NotEqual, "!=" },
                { ExpressionType.Or, "||" },
                { ExpressionType.OrElse, "||" },
                { ExpressionType.Coalesce, "??" },
            };

        private string result;

        /// <summary>
        /// Prints an expression.
        /// </summary>
        /// <param name="exp">Prints he expression.</param>
        /// <returns>The printed expression.</returns>
        public string Print(Expression exp)
        {
            result = Recurse(exp);
            return result;
        }

        private string GetOperator(ExpressionType type) => expressions.ContainsKey(type) ?
            expressions[type] : type.ToString();

        private string Recurse(Expression exp)
        {
            if (exp == null)
            {
                return string.Empty;
            }

            var tag = TagManager.GetTag(exp);

            switch (exp)
            {
                case null:
                    return string.Empty;

                case BlockExpression be:
                    if (tag != null)
                    {
                        return tag;
                    }

                    return string.Join(' ', be.Expressions.Select(e => Recurse(e)));

                case ParameterExpression pe:
                    var name = pe.Name ?? "unknown";
                    return $"{{{name}:{pe.Type.Name}}}";

                case BinaryExpression be:
                    var left = Recurse(be.Left);
                    var right = Recurse(be.Right);
                    return $"({left} {tag ?? GetOperator(be.NodeType)} {right})";

                case ConstantExpression ce:
                    if (ce.Value is Expression ex)
                    {
                        return tag ?? Recurse(ex);
                    }

                    if (ce.Value is ExtendedProperty ep)
                    {
                        return tag ?? ep.ValueToString;
                    }

                    if (ce.Value is null)
                    {
                        return tag ?? $"[default({ce.Type.Name})]";
                    }

                    return tag ?? $"[{ce.Value}]";

                case LambdaExpression le:
                    return tag ?? Recurse(le.Body);

                case UnaryExpression ue:
                    var value = Recurse(ue.Operand);
                    return $"({tag ?? GetOperator(ue.NodeType)} {value})";

                case MemberExpression me:
                    var parent = Recurse(me.Expression);
                    return tag ?? $"{parent}=>{me.Member?.Name}";

                default:
                    return exp.ToString();
            }
        }
    }
}
