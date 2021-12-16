// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System.Collections.Generic;
using FileQueryDatabase.Query;

namespace FileQueryDatabase
{
    /// <summary>
    /// Global values.
    /// </summary>
    public static class Globals
    {
        /// <summary>
        /// Extensions supported by the tool.
        /// </summary>
        public static readonly string[] SupportedExtensions =
            new[]
            {
                ".jpeg",
                ".jpg",
                ".jfif",
                ".png",
                ".webp",
                ".gif",
                ".ico",
                ".bmp",
                ".tif",
                ".tiff",
                ".psd",
                ".pcx",
                ".raw",
                ".crw",
                ".cr2",
                ".nef",
                ".orf",
                ".raf",
                ".rw2",
                ".rwl",
                ".srw",
                ".arw",
                ".dng",
                ".x3f",
                ".mov",
                ".mp4",
                ".m4v",
                ".3g2",
                ".3gp",
                ".fit",
                ".fits",
            };

        /// <summary>
        /// Extensions for FITS files.
        /// </summary>
        public static readonly string[] FitsExt = new string[] { ".fit", ".fits" };

        /// <summary>
        /// Operations map.
        /// </summary>
        public static readonly IDictionary<string, Operations> OpMap =
            new Dictionary<string, Operations>
            {
                { "&&", Operations.And },
                { "||", Operations.Or },
                { "<", Operations.LessThan },
                { "<=", Operations.LessThanOrEqual },
                { "==", Operations.Equal },
                { ">", Operations.GreaterThan },
                { ">=", Operations.GreaterThanOrEqual },
                { "!=", Operations.NotEqual },
                { "startswith", Operations.StartsWith },
                { "endswith", Operations.EndsWith },
                { "contains", Operations.Contains },
            };

        /// <summary>
        /// File count constant.
        /// </summary>
        public static readonly string FileCount = nameof(FileCount);

        /// <summary>
        /// Directory count constant.
        /// </summary>
        public static readonly string DirectoryCount = nameof(DirectoryCount);

        /// <summary>
        /// Child count constant.
        /// </summary>
        public static readonly string ChildCount = nameof(ChildCount);
    }
}
