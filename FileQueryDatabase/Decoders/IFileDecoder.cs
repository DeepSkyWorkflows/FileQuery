// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System;
using System.IO;
using FileQueryDatabase.Database;

namespace FileQueryDatabase.Decoders
{
    /// <summary>
    /// Interface for code that extracts metadata for a file.
    /// </summary>
    public interface IFileDecoder
    {
        /// <summary>
        /// Decodes the file.
        /// </summary>
        /// <param name="file">The <see cref="FileInfo"/>.</param>
        /// <param name="addMetadata">Callback to add metadata.</param>
        /// <param name="retrieveMetadata">Callback to retrieve metadata.</param>
        /// <param name="executeInMonitor">Callback to execute single-threaded.</param>
        void DecodeFile(
            FileInfo file,
            Action<string, ExtendedProperty> addMetadata,
            Func<string, ExtendedProperty> retrieveMetadata,
            Action<Action> executeInMonitor);
    }
}
