// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System.Collections.Generic;
using System.Linq;

namespace FileQueryDatabase.Tokens
{
    /// <summary>
    /// Hosts a command and its individual tokens.
    /// </summary>
    public class TokenPayload
    {
        /// <summary>
        /// The parsed tokens.
        /// </summary>
        private readonly List<TokenItem> tokens = new ();

        /// <summary>
        /// Gets or sets the original command text.
        /// </summary>
        public string Command { get; set; }

        /// <summary>
        /// Add a new token to the collection.
        /// </summary>
        /// <param name="item">The <see cref="TokenItem"/> to add.</param>
        public void Add(TokenItem item)
        {
            tokens.Add(item);
        }

        /// <summary>
        /// Get an iterator to parse tokens.
        /// </summary>
        /// <returns>The <see cref="TokenItem"/> enumerator.</returns>
        public IEnumerable<TokenItem> Tokens() =>
            tokens.Where(t => !string.IsNullOrWhiteSpace(t.Token)).OrderBy(t => t.Pos);
    }
}
