// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System;
using System.Collections.Generic;

namespace FileQueryDatabase.Query
{
    /// <summary>
    /// Allows annotating, with tag, any entity.
    /// </summary>
    public static class TagManager
    {
        private static readonly List<(WeakReference tgt, string tag)> TagCache = new ();

        /// <summary>
        /// Apply a tag to a target.
        /// </summary>
        /// <param name="target">The target object to tag.</param>
        /// <param name="tag">The tag.</param>
        public static void TagObject(object target, string tag)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            var idx = IndexOf(target);
            if (idx > 0)
            {
                TagCache[idx] = (TagCache[idx].tgt, tag);
            }
            else
            {
                TagCache.Add((new WeakReference(target), tag));
            }
        }

        /// <summary>
        /// Retrieve the tag for an object.
        /// </summary>
        /// <param name="target">The object that was tagged.</param>
        /// <returns>The tag.</returns>
        public static string GetTag(object target)
        {
            var idx = IndexOf(target);
            return idx >= 0 ? TagCache[idx].tag : null;
        }

        private static int IndexOf(object obj)
        {
            for (var x = 0; x < TagCache.Count; x++)
            {
                var (weakRef, _) = TagCache[x];
                if (weakRef.IsAlive && ReferenceEquals(obj, weakRef.Target))
                {
                    return x;
                }
            }

            return -1;
        }
    }
}
