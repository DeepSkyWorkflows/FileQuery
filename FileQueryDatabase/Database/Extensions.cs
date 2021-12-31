// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System;
using System.Runtime.CompilerServices;

namespace FileQueryDatabase.Database
{
    /// <summary>
    /// Helper extensions.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Cast a value to an <see cref="ExtendedProperty"/>.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="tag">The property name.</param>
        /// <returns>The <see cref="ExtendedProperty"/>.</returns>
        public static ExtendedProperty AsExtendedProperty<T>(this T value, [CallerMemberName] string tag = null)
        {
            return new ()
            {
                Name = tag,
                Value = value,
            };
        }

        /// <summary>
        /// Assumes a scalar and converts to bytes.
        /// </summary>
        /// <param name="value">The <see cref="ExtendedProperty"/>.</param>
        /// <returns>The length in bytes.</returns>
        public static string ToLengthInBytes(this ExtendedProperty value)
        {
            const long Kb = 1024;
            const long Mb = Kb * Kb;
            const long Gb = Mb * Kb;
            var length = (long)value.Value;
            if (length > Gb)
            {
                var len = ((float)length / Gb).ToString("0.00");
                return $"{len} Gb";
            }

            if (length > Mb)
            {
                var len = ((float)length / Mb).ToString("0.00");
                return $"{len} Mb";
            }

            if (length > Kb)
            {
                var len = ((float)length / Kb).ToString("0.00");
                return $"{len} Kb";
            }

            return $"{length} bytes";
        }

        /// <summary>
        /// Converts a UTC date to local.
        /// </summary>
        /// <param name="prop">The <see cref="ExtendedProperty"/>.</param>
        /// <returns>The date value.</returns>
        public static string ToLocalDateAndTime(this ExtendedProperty prop)
        {
            if (prop.Value is DateTime dt)
            {
                var tz = TimeZoneInfo.Local;
                var date = TimeZoneInfo.ConvertTimeFromUtc(dt, tz);
                return $"{date.ToShortDateString()} {date.ToShortTimeString()}";
            }

            return string.Empty;
        }
    }
}
