// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

namespace FileQueryDatabase.Commands
{
    /// <summary>
    /// Command that lists all properties.
    /// </summary>
    public partial class PropertiesCommand : IHelpText
    {
        /// <summary>
        /// Gets the name for help.
        /// </summary>
        public string HelpName => "Properties";

        /// <summary>
        /// Gets the description for help.
        /// </summary>
        public string HelpDescription => "Lists all properties available in the current database with their types.";

        /// <summary>
        /// Gets the list of parameters.
        /// </summary>
        public (string parameter, string name, string description)[] HelpParameters =>
            new[] { ("<filter text>", "Filter text", "Filter to only properties that contain the text.") };

        /// <summary>
        /// Gets the examples.
        /// </summary>
        public (string code, string description)[] HelpExamples =>
            new[]
            {
                ("properties", "Lists all properties in the current database."),
                ("properties image", "Lists all properties that start with, end with, or contain the text 'image.'"),
            };
    }
}
