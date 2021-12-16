// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System.Collections.Generic;
using System.Linq;

namespace FileQueryDatabase.Database
{
    /// <summary>
    /// Base node that represents a file or a directory.
    /// </summary>
    public class FileNode
    {
        /// <summary>
        /// Property cache.
        /// </summary>
        private readonly IDictionary<string, ExtendedProperty> extendedProperties =
           new Dictionary<string, ExtendedProperty>();

        /// <summary>
        /// Gets or sets the unique id (path).
        /// </summary>
        public string Id { get; protected set; } = string.Empty;

        /// <summary>
        /// Gets or sets the level.
        /// </summary>
        public int Level { get; set; } = 0;

        /// <summary>
        /// Gets or sets the parent directory.
        /// </summary>
        public string ParentId { get; protected set; } = string.Empty;

        /// <summary>
        /// Access the property.
        /// </summary>
        /// <param name="col">The property (column) name.</param>
        /// <returns>The <see cref="ExtendedProperty"/>.</returns>
        public ExtendedProperty this[string col] =>
            extendedProperties.ContainsKey(col) ?
            extendedProperties[col] : ExtendedProperty.NULL;

        /// <summary>
        /// Get the properties available on this node.
        /// </summary>
        /// <returns>The list of properties.</returns>
        public string[] GetProperties()
        {
            return extendedProperties.Keys.ToArray();
        }

        /// <summary>
        /// Add a new entry.
        /// </summary>
        /// <param name="key">They key.</param>
        /// <param name="value">The value to add.</param>
        public void Add(string key, ExtendedProperty value) =>
            extendedProperties.Add(key, value);

        /// <summary>
        /// Gets the hash code.
        /// </summary>
        /// <returns>The hash of the path.</returns>
        public override int GetHashCode() => Id.GetHashCode();

        /// <summary>
        /// Equals implementation.
        /// </summary>
        /// <param name="obj">The object to compare to.</param>
        /// <returns>A value indicating whether the other object is a <see cref="FileNode"/> with the same path.</returns>
        public override bool Equals(object obj) =>
            obj is FileNode fn && fn.Id == Id;

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>The type and path.</returns>
        public override string ToString() => $"{GetType().Name}: {Id}";
    }
}
