// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FileQueryDatabase.Database;
using FileQueryDatabase.Services;
using FileQueryDatabase.Tokens;

namespace FileQueryDatabase.Commands
{
    /// <summary>
    /// The report command.
    /// </summary>
    public partial class ReportCommand : IFileQueryCommand
    {
        private readonly ITokenizer tokenizer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportCommand"/> class.
        /// </summary>
        /// <param name="sp">The <see cref="IServiceProvider"/> instance.</param>
        public ReportCommand(IFileQueryServiceProvider sp) => tokenizer = sp.ResolveService<ITokenizer>();

        /// <summary>
        /// Gets the command text.
        /// </summary>
        public string CommandText => "report";

        /// <summary>
        /// Gets a value indicating whether the command starts with the command text and contains parameters.
        /// </summary>
        public bool StartsWith => true;

        /// <summary>
        /// Main implementation.
        /// </summary>
        /// <param name="db">The <see cref="FileDatabase"/>.</param>
        /// <param name="command">The user command.</param>
        /// <returns>A value indicating whether the program should terminate.</returns>
        public bool Execute(FileDatabase db, string command)
        {
            var camera = new[] { "name", "focal length", "exposure time", "iso", "creationtimeutc" };
            var sort = new (string prop, bool ascending)[]
            {
                ("focal length", true),
                ("exposure time", false),
                ("creationtimeutc", true),
            };
            var select = camera;
            var nodes = db.CurrentDirectory.Descendants.AsQueryable();
            if (db.Filter != null)
            {
                nodes = nodes.Where(db.Filter);
            }

            var matrix = new List<(FileNode fn, string[] cols)>();

            var colNames = select.Select(s => db.Columns[s].Name).ToArray();

            foreach (var item in nodes)
            {
                matrix.Add((item, select.Select(s => item[db.Columns[s].Name].ValueToString).ToArray()));
            }

            var query = matrix.AsQueryable();
            foreach (var (prop, ascending) in sort)
            {
                if (ascending)
                {
                    query = query.OrderBy(fn => fn.fn[prop].Value);
                }
                else
                {
                    query = query.OrderByDescending(fn => fn.fn[prop].Value);
                }
            }

            ConsoleManager.ShowMessage(string.Join('\t', colNames));
            foreach (var item in query)
            {
                var cols = string.Join('\t', item.cols);
                ConsoleManager.ShowMessage(cols);
            }

            return false;
        }
    }
}
