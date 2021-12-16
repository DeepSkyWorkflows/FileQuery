// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using FileQueryDatabase.Database;

namespace FileQueryDatabase.Commands
{
    /// <summary>
    /// Interface for a command to run.
    /// </summary>
    public interface IFileQueryCommand
    {
        /// <summary>
        /// Gets the text used for the command.
        /// </summary>
        string CommandText { get; }

        /// <summary>
        /// Gets a value indicating whether the command is standalone, or starts with the command text and
        /// has additional parameters.
        /// </summary>
        bool StartsWith { get; }

        /// <summary>
        /// Implementation of the command.
        /// </summary>
        /// <param name="db">The <see cref="FileDatabase"/>.</param>
        /// <param name="command">The user command.</param>
        /// <returns>A value indicating whether the program should terminate after executing the command.</returns>
        bool Execute(FileDatabase db, string command);
    }
}
