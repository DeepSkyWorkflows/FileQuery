// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System;
using System.Linq;
using FileQueryDatabase.Database;
using FileQueryDatabase.Services;

namespace FileQueryDatabase.Commands
{
    /// <summary>
    /// Command that lists all properties.
    /// </summary>
    public partial class PropertiesCommand : IFileQueryCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertiesCommand"/> class.
        /// </summary>
        /// <param name="ignore">Ignored provider instance.</param>
#pragma warning disable IDE0060 // Remove unused parameter
        public PropertiesCommand(IFileQueryServiceProvider ignore)
#pragma warning restore IDE0060 // Remove unused parameter
        {
        }

        /// <summary>
        /// Gets the text of the command.
        /// </summary>
        public string CommandText => "properties";

        /// <summary>
        /// Gets a value indicating whether the command is standalone, or starts with the command text and takes
        /// additional parameters.
        /// </summary>
        public bool StartsWith => true;

        /// <summary>
        /// Implementation of the command.
        /// </summary>
        /// <param name="db">The <see cref="FileDatabase"/> to use.</param>
        /// <param name="cmd">The user command.</param>
        /// <returns>A value indicating whether or not the program should terminate after running the command.</returns>
        public bool Execute(FileDatabase db, string cmd)
        {
            Func<string, bool> filter = str => true;
            if (cmd.Length > CommandText.Length)
            {
                var filterText = cmd[CommandText.Length..].Trim().ToLowerInvariant();
                filter = str => str.Contains(filterText, StringComparison.InvariantCultureIgnoreCase);
            }

            ConsoleManager.ShowMessage("Tracked properties:");
            foreach (var (column, type) in db.Columns.EnumerateProperties().OrderBy(t => t.column))
            {
                if (filter(column.ToLowerInvariant()))
                {
                    ConsoleManager.ShowMessage($"{column}\t{type}");
                }
            }

            return false;
        }
    }
}
