// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System;
using System.Linq.Expressions;
using FileQueryDatabase.Database;

namespace FileQueryDatabase.Query
{
    /// <summary>
    /// Query parsing logic.
    /// </summary>
    public interface IQueryParser
    {
        /// <summary>
        /// Transforms a filter into the expression tree for that filter.
        /// </summary>
        /// <param name="command">The user command.</param>
        /// <param name="db">The <see cref="FileDatabase"/> instance.</param>
        /// <returns>A compiled predicate to filter based on user input.</returns>
        Expression<Func<FileNode, bool>> Parse(string command, FileDatabase db);
    }
}
