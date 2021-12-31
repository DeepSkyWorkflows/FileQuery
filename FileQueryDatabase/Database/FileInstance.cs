// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System;
using System.IO;
using System.Threading;
using FileQueryDatabase.Decoders;
using FileQueryDatabase.Services;

namespace FileQueryDatabase.Database
{
    /// <summary>
    /// Instance of a file in the filesystem.
    /// </summary>
    public class FileInstance : FileNode
    {
        /// <summary>
        /// Lock for access to the no meta cache.
        /// </summary>
        private static readonly object Mutex = new ();

        private readonly IFileDecoder[] decoders;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileInstance"/> class.
        /// </summary>
        /// <param name="sp">Service provider.</param>
        /// <param name="parent">The parent directory.</param>
        /// <param name="file">The full path to the file.</param>
        /// <param name="level">The level in the current hierarchy.</param>
        public FileInstance(IFileQueryServiceProvider sp, DirectoryInstance parent, string file, int level = 0)
        {
            if (string.IsNullOrWhiteSpace(file))
            {
                throw new ArgumentNullException(nameof(file));
            }

            decoders = sp.ResolveService<IFileDecoder[]>();

            Parent = parent;
            Id = file;
            Level = level;
            ParseFile();
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>The type and path.</returns>
        public override string ToString()
        {
            var parts = new[]
            {
                this[nameof(FileInfo.Name)].ValueToString,
                this[nameof(FileInfo.Length)].ToLengthInBytes(),
                this[nameof(FileInfo.CreationTimeUtc)].ToLocalDateAndTime(),
                this[nameof(FileInfo.LastWriteTimeUtc)].ToLocalDateAndTime(),
            };

            return string.Join('\t', parts);
        }

        private void ParseFile()
        {
            if (!File.Exists(Id))
            {
                throw new FileNotFoundException(Id);
            }

            var info = new FileInfo(Id);

            foreach (var decoder in decoders)
            {
                decoder.DecodeFile(
                    info,
                    (key, val) => Add(key, val),
                    key => this[key],
                    action =>
                    {
                        Monitor.Enter(Mutex);
                        try
                        {
                            action();
                        }
                        finally
                        {
                            Monitor.Exit(Mutex);
                        }
                    });
            }
        }
    }
}
