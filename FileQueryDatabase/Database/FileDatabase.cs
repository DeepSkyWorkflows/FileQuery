// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FileQueryDatabase.Commands;
using FileQueryDatabase.Services;

namespace FileQueryDatabase.Database
{
    /// <summary>
    /// The instance of file database being used.
    /// </summary>
    public class FileDatabase
    {
        private readonly string dir;

        /// <summary>
        /// Columns (properties) being tracked.
        /// </summary>
        private readonly ColumnDatabase columns = new ();

        /// <summary>
        /// List of available commands.
        /// </summary>
        private readonly IFileQueryCommand[] commands;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileDatabase"/> class.
        /// </summary>
        /// <param name="dir">Directory to start with.</param>
        /// <param name="serviceProvider">Service resolver.</param>
        public FileDatabase(string dir, IFileQueryServiceProvider serviceProvider)
        {
            dir = dir.Trim().ToLowerInvariant();

            commands = GetType().Assembly.GetTypes()
                .Where(t => typeof(IFileQueryCommand).IsAssignableFrom(t) && !t.IsInterface)
                .Select(t => Activator.CreateInstance(t, new object[] { serviceProvider }))
                .OfType<IFileQueryCommand>().ToArray();

            ((HelpCommand)commands.Single(c => c is HelpCommand))
                .Register(commands.Select(c => (object)c).OfType<IHelpText>());

            if (string.IsNullOrEmpty(dir))
            {
                throw new DirectoryNotFoundException(dir);
            }

            this.dir = dir;
            Root = new DirectoryInstance(dir);
            CurrentDirectory = Root;
            Root.ParseDirectory();
            ConsoleManager.ShowInfo(null);
            ParseNodes();
        }

        /// <summary>
        /// Gets or sets the root directory scanned.
        /// </summary>
        public DirectoryInstance Root { get; set; }

        /// <summary>
        /// Gets or sets the current active directory.
        /// </summary>
        public DirectoryInstance CurrentDirectory { get; set; }

        /// <summary>
        /// Gets or sets he current filter being applied.
        /// </summary>
        public Expression<Func<FileNode, bool>> Filter { get; set; }

        /// <summary>
        /// Gets the properties database.
        /// </summary>
        public ColumnDatabase Columns => columns;

        /// <summary>
        /// Gets the count of files.
        /// </summary>
        public int Files => Root.Descendants.OfType<FileInstance>().Count();

        /// <summary>
        /// Gets the count of directories.
        /// </summary>
        public int Directories => Root.Descendants.OfType<DirectoryInstance>().Count();

        /// <summary>
        /// Main execution loop.
        /// </summary>
        public void Run()
        {
            if (string.IsNullOrEmpty(dir))
            {
                throw new ArgumentNullException(dir);
            }

            ConsoleManager.ShowMessage($"FileQuery {Assembly.GetExecutingAssembly().GetName().Version}");
            ConsoleManager.ShowMessage("'help' for help topics.");
            ConsoleManager.ShowMessage($"Engine initialized: tracking {Columns.PropertyCount} properties in {Files} files and {Directories} directories.");

            var end = false;
            while (!end)
            {
                var filterText = Filter == null ? "unfiltered" : "filtered";
                ConsoleManager.ShowMessage($"FileQuery ({CurrentDirectory.Id}:{filterText})> ", true);
                var cmd = ConsoleManager.ReadInput();
                end = Process(cmd);
            }
        }

        private void ParseNodes()
        {
            ConsoleManager.ShowMessage($"Parsing properties...");

            var properties = new[] { (FileNode)Root }.Union(
                Root.Descendants).SelectMany(fn => fn.GetProperties())
                .Distinct();
            ConsoleManager.ShowMessage($"{properties.Count()} properties found. Determining types...");

            foreach (var property in properties)
            {
                var sample = Root.Descendants.Where(fn => fn[property] != null && fn[property].Value != null)
                    .First()[property];

                if (!(sample is null) && !(sample.Value is null))
                {
                    if (property.IndexOf('.') < 0)
                    {
                        var prop = sample.Value.GetType().AsExtendedProperty();
                        prop.Name = property;
                        Columns.Add(null, property, sample.Value.GetType().AsExtendedProperty());
                    }
                    else
                    {
                        var parts = property.Split('.');
                        var prop = sample.Value.GetType().AsExtendedProperty();
                        prop.Name = property;
                        Columns.Add(parts[0], parts[1], prop);
                    }
                }
            }
        }

        private bool Process(string cmd)
        {
            var resolved = cmd.Trim().ToLowerInvariant();
            var commandText = resolved.Split(' ')[0];
            var command = commands.Where(
                c => (c.StartsWith &&
                c.CommandText.StartsWith(commandText)) ||
                (!c.StartsWith && c.CommandText == commandText)).FirstOrDefault();
            if (command == null)
            {
                Console.WriteLine($"Command {commandText} not recognized.");
                return false;
            }

            return command.Execute(this, resolved);
        }
    }
}
