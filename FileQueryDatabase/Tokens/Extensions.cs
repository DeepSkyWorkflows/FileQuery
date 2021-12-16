// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System.Collections.Generic;
using System.Linq;

namespace FileQueryDatabase.Tokens
{
    /// <summary>
    /// Helpful extensions.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Captures a token from t he command text.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="pos">The token's position.</param>
        /// <param name="inQuotes">Whether or not the token was quoted.</param>
        /// <returns>The <see cref="TokenItem"/>.</returns>
        public static TokenItem AsTokenAtPos(
            this IEnumerable<char> token,
            int pos,
            bool inQuotes = false)
        {
            return new (new string(token.ToArray()), pos) { Quoted = inQuotes };
        }
    }
}
