// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System;

namespace FileQueryDatabase.Query
{
    /// <summary>
    /// Exception thrown parsing the command tokens.
    /// </summary>
    public class TokenParseException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TokenParseException"/> class.
        /// </summary>
        /// <param name="command">The command text.</param>
        /// <param name="pos">The position of the token.</param>
        /// <param name="ex">The <see cref="Exception"/>.</param>
        public TokenParseException(string command, int pos, Exception ex)
            : base(GetMessage(command, pos, ex), ex)
        {
            Command = command;
            Pos = pos;
            RefException = ex;
        }

        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        public string Command { get; set; }

        /// <summary>
        /// Gets or sets the referenced exception.
        /// </summary>
        public Exception RefException { get; set; }

        /// <summary>
        /// Gets or sets the position of the token in the command.
        /// </summary>
        public int Pos { get; set; }

        private static string GetMessage(string command, int pos, Exception ex)
        {
            var pointer = new string(' ', pos) + "^";
            return $"{ex.Message}{Environment.NewLine}{command}{Environment.NewLine}{pointer}";
        }
    }
}
