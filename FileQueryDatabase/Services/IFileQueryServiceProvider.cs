// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

namespace FileQueryDatabase.Services
{
    /// <summary>
    /// Simple dependency resolution.
    /// </summary>
    public interface IFileQueryServiceProvider
    {
        /// <summary>
        /// Resolves the service for the contract.
        /// </summary>
        /// <typeparam name="T">The contract of the service.</typeparam>
        /// <returns>The concrete instance.</returns>
        T ResolveService<T>();
    }
}
