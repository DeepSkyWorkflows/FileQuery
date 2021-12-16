// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using FileQueryDatabase.Database;
using FileQueryDatabase.Tokens;

namespace FileQueryDatabase.Query
{
    /// <summary>
    /// Implementation of token parsing into operations.
    /// </summary>
    public class OperationParser : IOperationParser
    {
        /// <summary>
        /// Main method to process tokens into operations.
        /// </summary>
        /// <param name="tokens">The tokens to process.</param>
        /// <param name="db">The <see cref="FileDatabase"/> instance.</param>
        /// <returns>The operation.</returns>
        /// <exception cref="TokenParseException">Thrown when any error in parsing occurs.</exception>
        public Operation ParseTokens(TokenPayload tokens, FileDatabase db)
        {
            var operation = new Operation();
            var opStack = new Stack<Operation>();

            foreach (var token in tokens.Tokens())
            {
                try
                {
                    if (token.Token == "(")
                    {
                        opStack.Push(operation);
                        operation = new Operation();
                        continue;
                    }

                    if (token.Token == ")")
                    {
                        var savedOp = opStack.Pop();
                        savedOp.Add(operation);
                        operation = savedOp;
                        continue;
                    }

                    if (operation.Left == null)
                    {
                        operation.Add(ParseColumn(token.Token, db));
                        continue;
                    }

                    if (operation.Op != Operations.None && operation.Right == null)
                    {
                        operation.Add(ParseConstant(operation.Left, token.Token));
                        continue;
                    }

                    if (operation.Op == Operations.None)
                    {
                        operation.Op = ParseOperator(token.Token);
                    }
                    else
                    {
                        var newOperation = new Operation();
                        newOperation.Add(operation);
                        newOperation.Op = ParseOperator(token.Token);
                        opStack.Push(newOperation);
                        operation = new Operation();
                    }
                }
                catch (Exception ex)
                {
                    throw new TokenParseException(tokens.Command, token.Pos, ex);
                }
            }

            while (opStack.Count > 0)
            {
                var savedOp = opStack.Pop();
                savedOp.Add(operation);
                operation = savedOp;
            }

            return operation;
        }

        /// <summary>
        /// Parse a constant value.
        /// </summary>
        /// <param name="left">The left (column) side of the operation.</param>
        /// <param name="token">The token.</param>
        /// <returns>The parsed operation.</returns>
        private static Operation ParseConstant(Operation left, string token)
        {
            var type = (Type)left.Left.Value;

            return new Operation
            {
                Op = Operations.Constant,
                Value = token == null ? ExtendedProperty.NULL : ConvertTo(token, type)
                    .AsExtendedProperty(),
            };
        }

        /// <summary>
        /// Parses a token to an operator that is part of the operation.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>The parsed operator.</returns>
        private static Operations ParseOperator(string token)
        {
            if (Globals.OpMap.ContainsKey(token))
            {
                return Globals.OpMap[token];
            }

            throw new InvalidOperationException($"Invalid operator: {token}");
        }

        /// <summary>
        /// Convert the string being parsed to the type of the related column.
        /// </summary>
        /// <param name="value">The string value.</param>
        /// <param name="type">The target type.</param>
        /// <returns>The converted value.</returns>
        private static object ConvertTo(string value, Type type)
        {
            var converter = TypeDescriptor.GetConverter(type);
            if (converter != null)
            {
                return converter.ConvertFrom(value);
            }

            if (typeof(IConvertible).IsAssignableFrom(type))
            {
                return Convert.ChangeType(value, type);
            }

            return value;
        }

        private static Operation ParseColumn(string token, FileDatabase db)
        {
            var column = db.Columns[token];
            if (column == null || column?.Value == null)
            {
                throw new ArgumentException($"Invalid property name: {token}");
            }

            var columnResult = new Operation
            {
                Op = Operations.ColumnName,
                Value = column.Name,
                Left = new Operation
                {
                    Op = Operations.Type,
                    Value = (Type)column.Value,
                },
            };

            return columnResult;
        }
    }
}
