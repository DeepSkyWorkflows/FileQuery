// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using FileQueryDatabase.Database;

namespace FileQueryDatabase.Decoders
{
    /// <summary>
    /// File info decoder.
    /// </summary>
    public class FileInfoDecoder : IFileDecoder
    {
        /// <summary>
        /// Decode the file.
        /// </summary>
        /// <param name="info">The <see cref="FileInfo"/>.</param>
        /// <param name="addMetadata">Callback to add metadata.</param>
        /// <param name="getMetadata">Callback to obtain metadaa.</param>
        /// <param name="executeInMonitor">Callback to execute in a single thread.</param>
        public void DecodeFile(
            FileInfo info,
            Action<string, ExtendedProperty> addMetadata,
            Func<string, ExtendedProperty> getMetadata,
            Action<Action> executeInMonitor)
        {
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
                addMetadata(key, value);
            }
        }
    }
}
