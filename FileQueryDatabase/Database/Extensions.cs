// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

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
    }
}
