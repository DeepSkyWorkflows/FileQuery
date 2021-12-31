// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System;
using System.IO;
using FileQueryDatabase.Database;
using FileQueryDatabase.Decoders;
using FileQueryDatabase.Query;
using FileQueryDatabase.Services;
using FileQueryDatabase.Tokens;

namespace FileQuery
{
    /// <summary>
    /// Main application entry.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main method.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        public static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                throw new ArgumentException("Invalid argument list.");
            }

            var dir = args.Length > 0 ? args[0] : Environment.CurrentDirectory;

            if (!Directory.Exists(dir))
            {
                throw new DirectoryNotFoundException(dir);
            }

            var serviceProvider = new FileQueryServiceProvider();
            serviceProvider.ConfigureService<ITokenizer>(sp => new Tokenizer());
            serviceProvider.ConfigureService<IOperationParser>(sp => new OperationParser());
            serviceProvider.ConfigureService<IQueryParser>(sp => new QueryParser(
                sp.ResolveService<ITokenizer>(),
                sp.ResolveService<IOperationParser>()));
            var decoders = new IFileDecoder[]
            {
                new FileInfoDecoder(),
                new ExifDecoder(),
                new FitsDecoder(),
            };
            serviceProvider.ConfigureService(sp => decoders);

            var db = new FileDatabase(dir, serviceProvider);
            db.Run();
        }
    }
}
