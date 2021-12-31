// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FileQueryDatabase.Database;
using MetadataExtractor;

namespace FileQueryDatabase.Decoders
{
    /// <summary>
    /// Metadata (EXIF) decoder.
    /// </summary>
    public class ExifDecoder : IFileDecoder
    {
        /// <summary>
        /// Cache for extensions that don't have metadata.
        /// </summary>
        private static readonly HashSet<string> NoMeta = new ();

        /// <inheritdoc/>
        public void DecodeFile(
            FileInfo info,
            Action<string, ExtendedProperty> addMetadata,
            Func<string, ExtendedProperty> getMetadata,
            Action<Action> executeInMonitor)
        {
            IReadOnlyList<MetadataExtractor.Directory> meta = null;

            var ext = info.Extension.Trim().ToLowerInvariant();

            if (!NoMeta.Contains(ext) && !Globals.FitsExt.Any(f => f == ext))
            {
                try
                {
                    meta = ImageMetadataReader.ReadMetadata(info.FullName);
                }
                catch (ImageProcessingException)
                {
                }
                catch (BadImageFormatException)
                {
                    if (!NoMeta.Contains(ext))
                    {
                        executeInMonitor(() =>
                        {
                            if (!NoMeta.Contains(ext))
                            {
                                NoMeta.Add(ext);
                            }
                        });
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
                        var test = getMetadata(key);
                        if (test.IsNull)
                        {
                            var srcobj = dir.GetObject(tag.Type);

                            // conversions
                            var obj = srcobj switch
                            {
                                Rational rational => rational.ToDecimal(),
                                StringValue stringValue =>
                                    (stringValue.Encoding ?? Encoding.Default)
                                    .GetString(stringValue.Bytes),
                                _ => srcobj,
                            };

                            addMetadata(key, obj.AsExtendedProperty(tag.Name));
                        }
                    }
                }
            }
        }
    }
}
