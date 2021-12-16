// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using MetadataExtractor;

namespace FileQueryDatabase.Database
{
    /// <summary>
    /// Instance of a file in the filesystem.
    /// </summary>
    public class FileInstance : FileNode
    {
        /// <summary>
        /// Cache for extensions that don't have metadata.
        /// </summary>
        private static readonly HashSet<string> NoMeta = new ();

        /// <summary>
        /// Lock for access to the no meta cache.
        /// </summary>
        private static readonly object Mutex = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="FileInstance"/> class.
        /// </summary>
        /// <param name="file">The full path to the file.</param>
        /// <param name="level">The level in the current hierarchy.</param>
        public FileInstance(string file, int level = 0)
        {
            if (string.IsNullOrWhiteSpace(file))
            {
                throw new ArgumentNullException(nameof(file));
            }

            Id = file;
            Level = level;
            ParseFile();
        }

        private void ParseFile()
        {
            if (!File.Exists(Id))
            {
                throw new FileNotFoundException(Id);
            }

            var info = new FileInfo(Id);

            var fileProperties = new Dictionary<string, ExtendedProperty>
            {
                { nameof(FileInfo.FullName), info.FullName.ToLowerInvariant().AsExtendedProperty(nameof(FileInfo.FullName)) },
                { nameof(FileInfo.Name),  info.Name.ToLowerInvariant().AsExtendedProperty(nameof(FileInfo.Name)) },
                { nameof(FileInfo.Length), info.Length.AsExtendedProperty(nameof(FileInfo.Length)) },
                { nameof(FileInfo.Extension), info.Extension.ToLowerInvariant().AsExtendedProperty(nameof(FileInfo.Extension)) },
                { nameof(FileInfo.CreationTimeUtc), info.CreationTimeUtc.AsExtendedProperty(nameof(FileInfo.CreationTimeUtc)) },
                { nameof(FileInfo.LastAccessTimeUtc), info.LastAccessTimeUtc.AsExtendedProperty(nameof(FileInfo.LastAccessTimeUtc)) },
                { nameof(FileInfo.LastWriteTimeUtc), info.LastWriteTimeUtc.AsExtendedProperty(nameof(FileInfo.LastWriteTimeUtc)) },
                { nameof(FileInfo.DirectoryName), info.DirectoryName.ToLowerInvariant().AsExtendedProperty(nameof(FileInfo.DirectoryName)) },
            };

            foreach (var (key, value) in fileProperties)
            {
                Add(key, value);
            }

            ParentId = info.DirectoryName;

            IReadOnlyList<MetadataExtractor.Directory> meta = null;

            var ext = info.Extension.Trim().ToLowerInvariant();

            if (!NoMeta.Contains(ext) && !Globals.FitsExt.Any(f => f == ext))
            {
                try
                {
                    meta = ImageMetadataReader.ReadMetadata(Id);
                }
                catch (ImageProcessingException)
                {
                }
                catch (BadImageFormatException)
                {
                    if (!NoMeta.Contains(ext))
                    {
                        Monitor.Enter(Mutex);
                        NoMeta.Add(ext);
                        Monitor.Exit(Mutex);
                    }
                }
            }

            if (meta != null)
            {
                foreach (var dir in meta)
                {
                    foreach (var tag in dir.Tags)
                    {
                        if (tag.Name.Contains("unknown", StringComparison.InvariantCultureIgnoreCase))
                        {
                            continue;
                        }

                        var key = $"{dir.Name}.{tag.Name}";
                        var test = this[key];
                        if (test == null || (string.IsNullOrWhiteSpace(test.Name) && test.Value == null))
                        {
                            Add(key, dir.GetObject(tag.Type).AsExtendedProperty(tag.Name));
                        }
                    }
                }
            }

            if (Globals.FitsExt.Any(f => f == ext))
            {
                var image = new JPFITS.FITSImage(Id, true);
                foreach (var name in image.Header.GetAllKeyNames())
                {
                    var value = image.Header.GetKeyValue(name);
                }
            }
        }
    }
}
