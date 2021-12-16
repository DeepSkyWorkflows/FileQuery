// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System;
using System.Linq;
using FileQueryDatabase.Database;
using FileQueryDatabase.Query;
using FileQueryDatabase.Services;

namespace FileQueryDatabase.Commands
{
    /// <summary>
    /// Allows queries to filter to a subset of files.
    /// </summary>
    public class FilterCommand : IFileQueryCommand, IHelpText
    {
        private readonly IQueryParser queryParser;

        private readonly FilterExpressionVisitor visitor = new ();

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterCommand"/> class.
        /// </summary>
        /// <param name="serviceProvider">Service for dependency resolution.</param>
        public FilterCommand(IFileQueryServiceProvider serviceProvider)
        {
            queryParser = serviceProvider.ResolveService<IQueryParser>();
        }

        /// <summary>
        /// Gets the text for the command to activate.
        /// </summary>
        public string CommandText => "filter";

        /// <summary>
        /// Gets a value indicating whether the command is standalone, or a list starts with
        /// the command and has additional arguments.
        /// </summary>
        public bool StartsWith => true;

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

        /// <summary>
        /// Method to run to start the filter.
        /// </summary>
        /// <param name="db">The current <see cref="FileDatabase"/>.
        /// </param>
        /// <param name="command">The user command.</param>
        /// <returns>A value indicating whether the program should terminate.</returns>
        public bool Execute(FileDatabase db, string command)
        {
            try
            {
                var parameters = command[6..];

                if (parameters.Trim().StartsWith("clear", StringComparison.InvariantCultureIgnoreCase))
                {
                    ConsoleManager.ShowMessage("Removed current filter.");
                    db.Filter = null;
                    return false;
                }

                if (parameters.Trim().StartsWith("show", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (db.Filter == null)
                    {
                        ConsoleManager.ShowMessage("There is no filter currently active.");
                    }
                    else
                    {
                        ConsoleManager.ShowMessage("Current filter:");
                        ConsoleManager.ShowMessage(visitor.Print(db.Filter));
                    }

                    return false;
                }

                var expression = queryParser.Parse(command[6..], db);
                var fn = expression.Compile();
                var query = db.Root.Descendants.Where(fn);
                if (!query.Any(f => f is FileInstance))
                {
                    var fileQuery = query.OfType<DirectoryInstance>().SelectMany(d => d.Children);
                    query = fileQuery;
                }

                ConsoleManager.ShowMessage($"Filtering: {visitor.Print(expression)}...");
                var result = query.Select(f => new { File = f.Id, Directory = f.ParentId }).
                    OrderBy(f => f.Directory).ThenBy(f => f.File).ToList();
                if (result.Count < 1)
                {
                    ConsoleManager.ShowMessage("No items found.");
                    return false;
                }

                var dir = string.Empty;
                foreach (var item in result)
                {
                    if (dir != item.Directory)
                    {
                        dir = item.Directory;
                        ConsoleManager.ShowMessage(dir);
                    }

                    ConsoleManager.ShowMessage($"\t{item.File}");
                }

                db.Filter = expression;
            }
            catch (Exception ex)
            {
                ConsoleManager.ShowMessage($"Error processing filter: {ex.Message}");
            }

            return false;
        }
    }
}
