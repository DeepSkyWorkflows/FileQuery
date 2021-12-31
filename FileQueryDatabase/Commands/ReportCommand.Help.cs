// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System;

namespace FileQueryDatabase.Commands
{
    /// <summary>
    /// Help for report command.
    /// </summary>
    public partial class ReportCommand : IHelpText
    {
        /// <summary>
        /// Gets the name of the command for help.
        /// </summary>
        public string HelpName => "Report";

        /// <summary>
        /// Gets the description of the command.
        /// </summary>
        public string HelpDescription => "Generate a report based on the filtered items.";

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        public (string parameter, string name, string description)[] HelpParameters =>
            new[]
            {
                ("<property list>", "Property List", "The list of proprties to include. Orders by the properties by default."),
                ("-o", "Order by", "Provide a property name or names to order the result. Separate with commas."),
            };

        /// <summary>
        /// Gets the help examples.
        /// </summary>
        public (string code, string description)[] HelpExamples =>
            Array.Empty<(string code, string description)>();
    }
}
