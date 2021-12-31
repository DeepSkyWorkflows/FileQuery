// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace FileQueryDatabase.Database
{
    /// <summary>
    /// Single entity property bag. One per directory or file.
    /// </summary>
    public class ColumnDatabase
    {
        /// <summary>
        /// Direct map.
        /// </summary>
        private readonly IDictionary<string, ExtendedProperty> properties = new ConcurrentDictionary<string, ExtendedProperty>();

        /// <summary>
        /// Indexed map: length => file.length etc.
        /// </summary>
        private readonly IDictionary<string, List<string>> propMap = new ConcurrentDictionary<string, List<string>>();

        /// <summary>
        /// Maps the key to the display value.
        /// </summary>
        private readonly IDictionary<string, string> caseMap = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// Gets the count of properties indexed by this instance.
        /// </summary>
        public int PropertyCount => properties.Count;

        /// <summary>
        /// Gets the <see cref="ExtendedProperty"/> with the specified name.
        /// </summary>
        /// <param name="property">The property to index by.</param>
        /// <remarks>This indexer implements a search algorithm. First, it looks for a direct match. Then, it
        /// looks for a partial match with a single result. Otherwise it returns a null wrapper.</remarks>
        /// <exception cref="KeyNotFoundException">Thrown when the property is not found.</exception>
        /// <returns>The property.</returns>
        public ExtendedProperty this[string property]
        {
            get
            {
                var key = property.ToLowerInvariant();

                // direct match
                if (properties.ContainsKey(key))
                {
                    return properties[key];
                }

                // possibly part of a directory
                if (propMap.ContainsKey(key))
                {
                    // property only exists in one directory
                    if (propMap[key].Count == 1)
                    {
                        var newKey = $"{propMap[key][0]}.{key}".ToLowerInvariant();
                        return properties.ContainsKey(newKey) ?
                            properties[newKey] :
                            ExtendedProperty.NULL;
                    }

                    throw new KeyNotFoundException($"Property '{key}' exists in multiple directories: " + string.Join(", ", propMap[key]) +
                        $"{Environment.NewLine}Please specify the desired directory like this: \"{propMap[key][0]}.{key}\" ");
                }

                // starts with the property
                var keySet = propMap.Keys.Where(k => k.StartsWith(key));

                // hit!
                if (keySet.Count() == 1)
                {
                    var candidate = keySet.First();

                    // single directory
                    if (propMap[candidate].Count == 1)
                    {
                        var newKey = $"{propMap[candidate][0]}.{candidate}".ToLowerInvariant();
                        return properties.ContainsKey(newKey) ?
                            properties[newKey] :
                            ExtendedProperty.NULL;
                    }

                    throw new KeyNotFoundException(string.Join(", ", propMap[candidate]));
                }

                if (!keySet.Any())
                {
                    return ExtendedProperty.NULL;
                }

                throw new KeyNotFoundException(keySet.Count() <= 5 ?
                    string.Join(", ", keySet) : key);
            }
        }

        /// <summary>
        /// Add a new entry.
        /// </summary>
        /// <param name="dir">The properties directory.</param>
        /// <param name="property">The property name.</param>
        /// <param name="value">The value.</param>
        public void Add(string dir, string property, ExtendedProperty value)
        {
            if (!string.IsNullOrWhiteSpace(property))
            {
                var keyName = string.IsNullOrWhiteSpace(dir) ? property : $"{dir}.{property}";
                var key = keyName.ToLowerInvariant();
                if (!properties.ContainsKey(key))
                {
                    if (properties.TryAdd(key, value))
                    {
                        value.Name = keyName;
                        if (propMap.ContainsKey(property.ToLowerInvariant()))
                        {
                            propMap[property.ToLowerInvariant()].Add(dir);
                        }
                        else
                        {
                            propMap.Add(property.ToLowerInvariant(), new List<string> { dir });
                        }

                        caseMap.Add(key, keyName);
                    }
                }
            }
        }

        /// <summary>
        /// Iterate the list of properties.
        /// </summary>
        /// <returns>Returns the column and type.</returns>
        public IEnumerable<(string column, string type)> EnumerateProperties()
        {
            foreach (var prop in properties)
            {
                yield return (caseMap[prop.Key], prop.Value.ValueToString);
            }
        }
    }
}
