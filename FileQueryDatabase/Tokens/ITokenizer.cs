// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

namespace FileQueryDatabase.Tokens
{
    /// <summary>
    /// Interface to blindly parse a command into tokens.
    /// </summary>
    /// <remarks>
    /// The tokenizer handles breaking a string into parts, such as splitting on
    /// commas and operators and properly handing quoted expressions or parameters.
    /// </remarks>
    public interface ITokenizer
    {
        /// <summary>
        /// Parses a command into its tokens.
        /// </summary>
        /// <param name="command">The command to parse.</param>
        /// <returns>The tokens parsed.</returns>
        TokenPayload Tokenize(string command);
    }
}
