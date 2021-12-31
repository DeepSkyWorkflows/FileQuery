// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

namespace FileQueryDatabase.Commands
{
    /// <summary>
    /// "LS" command to list current directory.
    /// </summary>
    public partial class ListCommand : IHelpText
    {
        /// <summary>
        /// Gets the name of the command for help purposes.
        /// </summary>
        public string HelpName => "List";

        /// <summary>
        /// Gets the description.
        /// </summary>
        public string HelpDescription => "Lists the current directories and files within the user's scope.";

        /// <summary>
        /// Gets the possible parameters.
        /// </summary>
        public (string parameter, string name, string description)[] HelpParameters =>
            new[]
            {
                ("<directory>", "directory", "List files in the specified directory."),
                ("-d", "directoriesOnly", "Only list the directories: skip files."),
                ("-f", "filesOnly", "Only list information about files: skip directories."),
                ("-r", "recursive", "Recurse sub-directories for results."),
            };

        /// <summary>
        /// Gets the examples for help.
        /// </summary>
        public (string code, string description)[] HelpExamples =>
            new[]
            {
                ("ls", "List the current files and directories."),
                ("ls -f -r", "Recursively list all files and ignore directories."),
            };
    }
}
