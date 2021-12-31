// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

namespace FileQueryDatabase.Commands
{
    /// <summary>
    /// The help portion of the filter command.
    /// </summary>
    public partial class FilterCommand : IHelpText
    {
        /// <summary>
        /// Gets the name for help.
        /// </summary>
        public string HelpName => "Filter";

        /// <summary>
        /// Gets the description for help.
        /// </summary>
        public string HelpDescription => "Filters the current database using the expression provided.";

        /// <summary>
        /// Gets the list of parameters.
        /// </summary>
        public (string parameter, string name, string description)[] HelpParameters =>
            new[]
            {
                ("[expression]",
                "Filter expression",
                "Expects the format [column] [operator] [value] and complex filters are allowed. The column name can be as" +
                " many characters as needed to provide a unique name. Strings should be surrounded by quotes. Subsequent filters" +
                " are additive."),
                ("add",
                "Add filter",
                "Adds to the current filter."),
                ("clear",
                "Clear filter",
                "Removes the current filter."),
                ("show",
                "Show filter",
                "Displays the current filter."),
            };

        /// <summary>
        /// Gets the examples.
        /// </summary>
        public (string code, string description)[] HelpExamples =>
            new[]
            {
                ("filter length < 65535", "Filters to all files with a length of less than 65k."),
                ("filter \"focal len\" == 55 && filename contains \"asc\" && iso < 3200",
                 "Filters to files with a focal length of 55mm with 'asc' in the filename that had" +
                " an ISO setting of less  than 3200."),
            };
    }
}
