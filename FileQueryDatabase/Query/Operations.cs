// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

namespace FileQueryDatabase.Query
{
    /// <summary>
    /// Possible operations for filters.
    /// </summary>
    public enum Operations
    {
        /// <summary>
        /// No operation.
        /// </summary>
        None,

        /// <summary>
        /// Operation stores a column reference.
        /// </summary>
        ColumnName,

        /// <summary>
        /// Operation stores a constant value.
        /// </summary>
        Constant,

        /// <summary>
        /// Operation references a type.
        /// </summary>
        Type,

        /// <summary>
        /// Logical AND (also).
        /// </summary>
        And,

        /// <summary>
        /// Logical OR (else).
        /// </summary>
        Or,

        /// <summary>
        /// Less than comparison.
        /// </summary>
        LessThan,

        /// <summary>
        /// Less than or equal comparison.
        /// </summary>
        LessThanOrEqual,

        /// <summary>
        /// Greater than comparison.
        /// </summary>
        GreaterThan,

        /// <summary>
        /// Greater than or equal to comparison.
        /// </summary>
        GreaterThanOrEqual,

        /// <summary>
        /// Equality comparison.
        /// </summary>
        Equal,

        /// <summary>
        /// Inequality comparison.
        /// </summary>
        NotEqual,

        /// <summary>
        /// String starts with.
        /// </summary>
        StartsWith,

        /// <summary>
        /// String ends with.
        /// </summary>
        EndsWith,

        /// <summary>
        /// String contains.
        /// </summary>
        Contains,
    }
}
