// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System;

namespace FileQueryDatabase.Query
{
    /// <summary>
    /// A binary operational node in the working filter tree.
    /// </summary>
    public class Operation
    {
        /// <summary>
        /// Gets or sets the operation.
        /// </summary>
        public Operations Op { get; set; } = Operations.None;

        /// <summary>
        /// Gets or sets the left side (input) into the operation.
        /// </summary>
        public Operation Left { get; set; }

        /// <summary>
        /// Gets or sets the right side (operand) into the operation.
        /// </summary>
        public Operation Right { get; set; }

        /// <summary>
        /// Gets or sets the value of the operation.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Gets a value indicating whether operations have relationships.
        /// </summary>
        public bool IsEmpty => Op == Operations.None && Left == null && Right == null;

        /// <summary>
        /// Combines the operations by flowing left to right.
        /// </summary>
        /// <param name="op">The <see cref="Operation"/> to add.</param>
        public void Add(Operation op)
        {
            if (op.IsEmpty)
            {
                return;
            }

            if (Left == null)
            {
                Left = op;
                return;
            }

            if (Right == null)
            {
                Right = op;
                return;
            }

            throw new InvalidOperationException("Invalid operation.");
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>The string representation of the operation.</returns>
        public override string ToString()
        {
            if (Op == Operations.ColumnName)
            {
                return $"\"{Value}\"";
            }

            if (Op == Operations.Constant)
            {
                return Value == null ? "<null>" : Value.ToString();
            }

            return $"{Op} {Left} {Right}";
        }
    }
}
