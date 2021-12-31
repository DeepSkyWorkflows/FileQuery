// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System;
using System.Linq;
using FileQueryDatabase.Database;
using FileQueryDatabase.Services;
using FileQueryDatabase.Tokens;

namespace FileQueryDatabase.Commands
{
    /// <summary>
    /// "LS" command to list current directory.
    /// </summary>
    public partial class ListCommand : IFileQueryCommand
    {
        private readonly ITokenizer tokenizer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListCommand"/> class.
        /// </summary>
        /// <param name="sp">Ignored service provider.</param>
        public ListCommand(IFileQueryServiceProvider sp) => tokenizer = sp.ResolveService<ITokenizer>();

        /// <summary>
        /// Gets the text of the command.
        /// </summary>
        public string CommandText => "ls";

        /// <summary>
        /// Gets a value indicating whether the command starts with the command text and has parameters.
        /// </summary>
        public bool StartsWith => true;

        /// <summary>
        /// Implementaton of the command.
        /// </summary>
        /// <param name="db">The <see cref="FileDatabase"/>.</param>
        /// <param name="command">The command.</param>
        /// <returns>A value indicating whether the program should terminate after returning.</returns>
        public bool Execute(FileDatabase db, string command)
        {
            try
            {
                var tokens = tokenizer.Tokenize(command[2..]);
                var parms = tokens.Tokens().Select(t => t.Token).ToArray();
                bool filesOnly = Array.IndexOf(parms, "-f") >= 0;
                bool recurse = Array.IndexOf(parms, "-r") >= 0;
                bool directoriesOnly = Array.IndexOf(parms, "-d") >= 0;
                string subdir = null;
                foreach (var parameter in parms)
                {
                    if (parameter.StartsWith("-"))
                    {
                        continue;
                    }

                    if (!string.IsNullOrEmpty(parameter))
                    {
                        subdir = parameter;
                    }
                }

                var dir = db.CurrentDirectory;

                if (subdir != null)
                {
                    var childDir = db.CurrentDirectory.Children.OfType<DirectoryInstance>()
                        .SingleOrDefault(d => d.Id.EndsWith(subdir, StringComparison.InvariantCultureIgnoreCase));
                    dir = childDir ?? throw new ArgumentException($"'subdir' is not a valid directory name.");
                }

                var candidates = recurse ? dir.Descendants : dir.Children;
                var filter = db.Filter == null ? candidates.AsQueryable() : candidates.AsQueryable().Where(db.Filter);
                if (filesOnly)
                {
                    filter = filter.OfType<FileInstance>();
                }

                if (directoriesOnly)
                {
                    filter = filter.OfType<DirectoryInstance>();
                }

                if (filter.Any())
                {
                    ConsoleManager.ShowMessage($"{dir}:");

                    var minLevel = filter.Min(f => f.Level);

                    foreach (var item in filter.OrderBy(f => f.Level).ThenBy(f => f.Id))
                    {
                        if (item is FileInstance fi)
                        {
                            if (fi.Level - 1 == db.CurrentDirectory.Level)
                            {
                                ConsoleManager.ShowMessage(fi.ToString());
                            }
                            else
                            {
                                ConsoleManager.ShowMessage($"\t{fi}");
                            }
                        }
                        else
                        {
                            ConsoleManager.ShowMessage(((DirectoryInstance)item).ToString());
                            continue;
                        }
                    }
                }
                else
                {
                    ConsoleManager.ShowMessage("No files or directories match the current filter. Use 'filter clear' to reset the filter.");
                }
            }
            catch (ArgumentException ae)
            {
                ConsoleManager.ShowMessage($"Error: {ae.Message}.");
            }
            catch (InvalidOperationException io)
            {
                ConsoleManager.ShowMessage($"Error: {io.Message}.");
            }

            return false;
        }
    }
}
