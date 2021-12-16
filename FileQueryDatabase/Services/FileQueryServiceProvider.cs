// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System;
using System.Collections.Generic;

namespace FileQueryDatabase.Services
{
    /// <summary>
    /// Basic dependency resolution.
    /// </summary>
    public class FileQueryServiceProvider : IFileQueryServiceProvider
    {
        /// <summary>
        /// Collection of registered factories for services.
        /// </summary>
        private readonly IDictionary<Type, object> factories =
            new Dictionary<Type, object>();

        /// <summary>
        /// Register a service.
        /// </summary>
        /// <typeparam name="T">The service contract.</typeparam>
        /// <param name="factory">The service factory.</param>
        public void ConfigureService<T>(Func<IFileQueryServiceProvider, T> factory)
        {
            if (factories.ContainsKey(typeof(T)))
            {
                throw new InvalidOperationException($"Type {typeof(T)} already configured.");
            }

            factories.Add(typeof(T), factory);
        }

        /// <summary>
        /// Resolve a service to its instance.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <returns>The concrete instance.</returns>
        public T ResolveService<T>()
        {
            if (!factories.ContainsKey(typeof(T)))
            {
                return default;
            }

            var factory = (Func<IFileQueryServiceProvider, T>)factories[typeof(T)];
            return factory(this);
        }
    }
}
