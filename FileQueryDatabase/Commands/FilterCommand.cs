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
    public partial class FilterCommand : IFileQueryCommand
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

                bool add = parameters.Trim().StartsWith("add", StringComparison.InvariantCultureIgnoreCase);

                if (add)
                {
                    command = command[(command.IndexOf("add") - 2) ..];
                }

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

                var expression = queryParser.Parse(command[6..], db, add);
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
