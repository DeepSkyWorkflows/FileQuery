// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FileQueryDatabase.Database
{
    /// <summary>
    /// Metadata about a directory entry.
    /// </summary>
    public class DirectoryInstance : FileNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryInstance"/> class.
        /// </summary>
        /// <param name="directory">Full path of the directory.</param>
        /// <param name="level">Level from root.</param>
        public DirectoryInstance(string directory, int level = 0)
        {
            if (string.IsNullOrWhiteSpace(directory))
            {
                throw new ArgumentNullException(directory);
            }

            Level = level;
            Id = directory.ToLowerInvariant();
        }

        /// <summary>
        /// Gets or sets the list of immediate chlidren.
        /// </summary>
        public List<FileNode> Children { get; protected set; } =
            new List<FileNode>();

        /// <summary>
        /// Gets or sets the hierarchy of all descendants.
        /// </summary>
        public List<FileNode> Descendants { get; protected set; } =
            new List<FileNode>();

        /// <summary>
        /// Parses the information.
        /// </summary>
        /// <exception cref="DirectoryNotFoundException">Thrown when the directory does not exist.</exception>
        public void ParseDirectory()
        {
            var dirInfo = new DirectoryInfo(Id);
            if (!dirInfo.Exists)
            {
                throw new DirectoryNotFoundException(Id);
            }

            var fileProperties = new Dictionary<string, ExtendedProperty>
            {
                { nameof(FileInfo.Extension), dirInfo.Extension.AsExtendedProperty(nameof(FileInfo.Extension)) },
                { nameof(FileInfo.CreationTimeUtc), dirInfo.CreationTimeUtc.AsExtendedProperty(nameof(FileInfo.CreationTimeUtc)) },
                { nameof(FileInfo.LastAccessTimeUtc), dirInfo.LastAccessTimeUtc.AsExtendedProperty(nameof(FileInfo.LastAccessTimeUtc)) },
                { nameof(FileInfo.LastWriteTimeUtc), dirInfo.LastWriteTimeUtc.AsExtendedProperty(nameof(FileInfo.LastWriteTimeUtc)) },
                { Globals.ChildCount, dirInfo.EnumerateFileSystemInfos().Count().AsExtendedProperty(Globals.ChildCount) },
                { Globals.DirectoryCount, dirInfo.EnumerateDirectories().Count().AsExtendedProperty(Globals.DirectoryCount) },
                { Globals.FileCount, dirInfo.EnumerateFiles().Count().AsExtendedProperty(Globals.FileCount) },
            };

            if (dirInfo.Parent != null)
            {
                fileProperties.Add(nameof(FileInfo.DirectoryName), dirInfo.Parent.FullName.AsExtendedProperty(nameof(FileInfo.DirectoryName)));
                ParentId = dirInfo.Parent.FullName;
            }

            foreach (var (key, value) in fileProperties)
            {
                Add(key, value);
            }

            ConsoleManager.ShowInfo(this);

            var children = new ConcurrentBag<FileNode>();
            Parallel.ForEach(dirInfo.EnumerateDirectories(), info =>
            {
                var childDirectory = new DirectoryInstance(info.FullName, Level + 1);
                children.Add(childDirectory);
                childDirectory.ParseDirectory();
            });

            Parallel.ForEach(dirInfo.EnumerateFiles(), file =>
            {
                if (Array.IndexOf(Globals.SupportedExtensions, file.Extension) >= 0)
                {
                    var child = new FileInstance(file.FullName, Level + 1);
                    children.Add(child);
                    ConsoleManager.ShowInfo(child);
                }
            });

            foreach (var child in children)
            {
                Children.Add(child);
                Descendants.Add(child);
                if (child is DirectoryInstance childDir)
                {
                    Descendants.AddRange(childDir.Descendants);
                }
            }
        }
    }
}
