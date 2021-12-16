// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

namespace FileQueryDatabase.Tokens
{
    /// <summary>
    /// A simple token representing a part of the command parsed for processing.
    /// </summary>
    public class TokenItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TokenItem"/> class.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="pos">The position of the token.</param>
        public TokenItem(string token, int pos)
        {
            Token = token;
            Pos = pos;
        }

        /// <summary>
        /// Gets or sets the token text.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets the position in the original command the token was parsed from.
        /// </summary>
        public int Pos { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the token was in quotes.
        /// </summary>
        public bool Quoted { get; set; }

        /// <summary>
        /// Equals implementation.
        /// </summary>
        /// <param name="obj">The other object.</param>
        /// <returns>A value indicating whether both objects are <see cref="TokenItem"/> with the same data.</returns>
        public override bool Equals(object obj) =>
            obj is TokenItem ti && ti.Pos == Pos && ti.Quoted == Quoted && ti.Token == Token;

        /// <summary>
        /// Gets the hash code.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode() =>
            $"{Pos}:{Quoted}:{Token}".GetHashCode();

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString() =>
            Quoted ? $"{Pos}:\"Token\"" : $"{Pos}:{Token}";
    }
}
