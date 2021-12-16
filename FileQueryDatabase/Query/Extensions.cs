// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

namespace FileQueryDatabase.Query
{
    /// <summary>
    /// Helpful extension methods.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Tags an object for reference... by reference.
        /// </summary>
        /// <typeparam name="T">Type of the objec to tag.</typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="tag">The tag.</param>
        /// <returns>The tagged object.</returns>
        public static T WithTag<T>(this T obj, string tag)
            where T : class
        {
            TagManager.TagObject(obj, tag);
            return obj;
        }
    }
}
