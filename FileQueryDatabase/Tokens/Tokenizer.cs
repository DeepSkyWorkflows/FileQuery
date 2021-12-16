// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System;
using System.Collections.Generic;

namespace FileQueryDatabase.Tokens
{
    /// <summary>
    /// Implementation of token parsing logic.
    /// </summary>
    public class Tokenizer : ITokenizer
    {
        /// <summary>
        /// Process a command into tokens.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>The parsed payload.</returns>
        public TokenPayload Tokenize(string command)
        {
            // normalize the input
            var cmd = command.Trim().ToLowerInvariant().AsSpan();

            // set up the result
            var result = new TokenPayload { Command = command.Trim() };

            // tracks the text of the current token
            var currentToken = new List<char>();

            // position of the token
            int tokenStart = 0;

            // a flag indicating whether a quoted item is being parsed, hence waiting for another quote
            bool inQuotes = false;

            // a flag indicating whether the parser is in an operation (<, &&, etc.)
            bool inOp = false;

            var idx = 0;
            do
            {
                var chr = cmd[idx];

                // wrap up the last token or begin a new one
                if (chr == '"')
                {
                    if (inQuotes || inOp)
                    {
                        result.Add(currentToken.AsTokenAtPos(tokenStart, inQuotes));
                        currentToken.Clear();
                        tokenStart = idx;
                    }

                    inQuotes = !inQuotes;
                    continue;
                }

                // clear tokens and start a new run
                if (chr == '(' || chr == ')')
                {
                    if (currentToken.Count > 0)
                    {
                        result.Add(currentToken.AsTokenAtPos(tokenStart));
                        currentToken.Clear();
                        tokenStart = idx;
                    }

                    continue;
                }

                if (inQuotes)
                {
                    currentToken.Add(chr);
                    continue;
                }

                if (chr == ' ')
                {
                    if (currentToken.Count > 0)
                    {
                        result.Add(currentToken.AsTokenAtPos(tokenStart));
                        tokenStart = idx;
                        currentToken.Clear();
                    }

                    continue;
                }

                var operand = "<>=&|".Contains(chr);

                if ((operand && !inOp) || (!operand && inOp))
                {
                    if (currentToken.Count > 0)
                    {
                        result.Add(currentToken.AsTokenAtPos(tokenStart));
                        tokenStart = idx;
                        currentToken.Clear();
                    }

                    inOp = operand;
                }

                currentToken.Add(chr);
            }
            while (++idx < cmd.Length);
            if (currentToken.Count > 0)
            {
                result.Add(currentToken.AsTokenAtPos(tokenStart));
            }

            return result;
        }
    }
}
