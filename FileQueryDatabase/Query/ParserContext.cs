// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System.Linq.Expressions;
using FileQueryDatabase.Database;

namespace FileQueryDatabase.Query
{
    /// <summary>
    /// Context for generating expression tree.
    /// </summary>
    public class ParserContext
    {
        /// <summary>
        /// The <see cref="FileDatabase"/> instance.
        /// </summary>
        private readonly FileDatabase db;

        /// <summary>
        /// The ubiquitous parameter.
        /// </summary>
        private readonly ParameterExpression target =
            Expression.Parameter(typeof(FileNode), "node");

        /// <summary>
        /// Initializes a new instance of the <see cref="ParserContext"/> class.
        /// </summary>
        /// <param name="db">The <see cref="FileDatabase"/>.</param>
        public ParserContext(FileDatabase db)
        {
            this.db = db;
        }

        /// <summary>
        /// Gets the <see cref="FileDatabase"/>.
        /// </summary>
        public FileDatabase Db => db;

        /// <summary>
        /// Gets the ubiquitous parameter.
        /// </summary>
        public ParameterExpression FileNodeParameter => target;
    }
}
