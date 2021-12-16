// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

namespace FileQueryDatabase.Commands
{
    /// <summary>
    /// Implements help information.
    /// </summary>
    public interface IHelpText
    {
        /// <summary>
        /// Gets the text of the command.
        /// </summary>
        string CommandText { get; }

        /// <summary>
        /// Gets the name of the help context.
        /// </summary>
        string HelpName { get; }

        /// <summary>
        /// Gets the description.
        /// </summary>
        string HelpDescription { get; }

        /// <summary>
        /// Gets the parameter list.
        /// </summary>
        (string parameter, string name, string description)[] HelpParameters { get; }

        /// <summary>
        /// Gets the examples.
        /// </summary>
        (string code, string description)[] HelpExamples { get; }
    }
}
