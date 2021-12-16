// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System;
using FileQueryDatabase.Database;
using FileQueryDatabase.Services;

namespace FileQueryDatabase.Commands
{
    /// <summary>
    /// When you're done, you're done.
    /// </summary>
    public class QuitCommand : IFileQueryCommand, IHelpText
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QuitCommand"/> class.
        /// </summary>
        /// <param name="ignore">Ignored service provider.</param>
        public QuitCommand(IFileQueryServiceProvider ignore)
        {
        }

        /// <summary>
        /// Gets the text of the command.
        /// </summary>
        public string CommandText => "exit";

        /// <summary>
        /// Gets a value indicating whether the command is a single word or starts with a list of parameters.
        /// </summary>
        public bool StartsWith => false;

        /// <summary>
        /// Gets the name for help.
        /// </summary>
        public string HelpName => "Exit";

        /// <summary>
        /// Gets the description for help.
        /// </summary>
        public string HelpDescription => "Terminates the session and exits.";

        /// <summary>
        /// Gets the list of parameters.
        /// </summary>
        public (string parameter, string name, string description)[] HelpParameters =>
            Array.Empty<(string parameter, string name, string description)>();

        /// <summary>
        /// Gets the examples.
        /// </summary>
        public (string code, string description)[] HelpExamples =>
            Array.Empty<(string code, string description)>();

        /// <summary>
        /// Implementation of the command.
        /// </summary>
        /// <param name="db">The <see cref="FileDatabase"/> instance.</param>
        /// <param name="command">The command.</param>
        /// <returns>A value indicating whether the program should terminate after running the
        /// command.</returns>
        public bool Execute(FileDatabase db, string command) => true;
    }
}
