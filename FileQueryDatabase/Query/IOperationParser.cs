// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using FileQueryDatabase.Database;
using FileQueryDatabase.Tokens;

namespace FileQueryDatabase.Query
{
    /// <summary>
    /// Parses tokense into operators.
    /// </summary>
    public interface IOperationParser
    {
        /// <summary>
        /// Main method.
        /// </summary>
        /// <param name="tokens">The project with tokens to share.</param>
        /// <param name="db">The <see cref="FileDatabase"/> instance.</param>
        /// <returns>The root operation of an operation tree.</returns>
        Operation ParseTokens(TokenPayload tokens, FileDatabase db);
    }
}
