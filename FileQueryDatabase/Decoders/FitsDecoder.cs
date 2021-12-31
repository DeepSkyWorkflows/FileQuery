// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System;
using System.IO;
using System.Linq;
using System.Text;
using FileQueryDatabase.Database;

namespace FileQueryDatabase.Decoders
{
    /// <summary>
    /// FITS header extractor.
    /// </summary>
    public class FitsDecoder : IFileDecoder
    {
        /// <inheritdoc/>
        public void DecodeFile(
            FileInfo info,
            Action<string, ExtendedProperty> addMetadata,
            Func<string, ExtendedProperty> getMetadata,
            Action<Action> executeInMonitor)
        {
            if (Globals.FitsExt.Any(f => f == info.Extension))
            {
                using var stream = File.OpenRead(info.FullName);

                byte[] c = new byte[2880];

                bool finished = false;

                while (!finished)
                {
                    stream.Read(c, 0, 2880);

                    for (int i = 0; i < 36; i++)
                    {
                        string line = Encoding.ASCII.GetString(c, i * 80, 80);

                        var keyValue = line.Split("=");

                        if (keyValue[0].Trim() == "END")
                        {
                            finished = true;
                            break;
                        }

                        if (keyValue[0].Trim().StartsWith("COMMENT "))
                        {
                            continue;
                        }

                        var valueComment = keyValue[1].Split("/");
                        var valueRepeat = valueComment[0].Split(":");

                        var keyStr = keyValue[0].Trim().ToLowerInvariant();
                        var valueStr = (valueRepeat[0].Trim().ToLowerInvariant() == keyStr ?
                            valueRepeat[1] : valueComment[0]).Trim();

                        var key = $"FITS.{keyStr}";
                        var test = getMetadata(keyStr);
                        if (test.IsNull)
                        {
                            addMetadata(key, valueStr.AsExtendedProperty(keyValue[0].Trim().ToLowerInvariant()));
                        }
                    }

                    if (stream.Position >= stream.Length && !finished)
                    {
                        finished = true;
                    }
                }
            }
        }
    }
}
