// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using FileQueryDatabase.Database;

namespace FileQueryDatabase
{
    /// <summary>
    /// Handles console i/o.
    /// </summary>
    public static class ConsoleManager
    {
        private static readonly object MessageMutex = new ();

        private static int left = -1;
        private static int top = -1;

        private static int directories = 0;
        private static int files = 0;

        private static bool messageSwitch = false;

        /// <summary>
        /// Output the message to the console, optionally with a newline at the end.
        /// </summary>
        /// <param name="msg">The message.</param>
        /// <param name="sameLine">A value indicating whether a new line should be appended.</param>
        public static void ShowMessage(string msg, bool sameLine = false)
        {
            if (sameLine)
            {
                Console.Write(msg);
            }
            else
            {
                Console.WriteLine(msg);
            }
        }

        /// <summary>
        /// Getes input from the console.
        /// </summary>
        /// <returns>The input.</returns>
        public static string ReadInput() => Console.ReadLine();

        /// <summary>
        /// Formatted "report" on <see cref="FileNode"/>.
        /// </summary>
        /// <param name="node">The <see cref="FileNode"/> to analyze.</param>
        public static void ShowInfo(this FileNode node)
        {
            if (node == null)
            {
                var rightMargin = Console.BufferWidth - 4;
                var blankLine = new string(' ', rightMargin + 3);
                Console.SetCursorPosition(left, top);
                Console.WriteLine($"{blankLine}{Environment.NewLine}{blankLine}{Environment.NewLine}{blankLine}");
                return;
            }

            if (node is FileInstance)
            {
                Interlocked.Increment(ref files);
            }
            else
            {
                Interlocked.Increment(ref directories);
            }

            if (messageSwitch)
            {
                return;
            }

            try
            {
                Monitor.Enter(MessageMutex);
                if (messageSwitch)
                {
                    return;
                }

                messageSwitch = true;
                ShowInfoConcurrent(node);
                messageSwitch = false;
            }
            finally
            {
                Monitor.Exit(MessageMutex);
            }
        }

        /// <summary>
        /// Parse input as tab separated columns.
        /// </summary>
        /// <param name="sequenceLength">Columns per line.</param>
        /// <param name="data">The data to parse.</param>
        public static void ShowColumns(int sequenceLength, IEnumerable<string> data)
        {
            var parmIdx = 0;
            foreach (var parm in data)
            {
                ShowMessage($"{parm}\t", sameLine: true);
                parmIdx++;
                if (parmIdx == sequenceLength)
                {
                    parmIdx = 0;
                    ShowMessage(string.Empty);
                }
            }
        }

        private static void ShowInfoConcurrent(this FileNode node)
        {
            var prefix = new string('-', node.Level) + ">";
            var filePrefix = new string(' ', prefix.Length) + ":";
            string dir = string.Empty,
                file = string.Empty,
                formalDir, formalFile;
            if (node is DirectoryInstance directory)
            {
                dir = directory.Id;
                file = string.Empty;
                filePrefix = string.Empty;
            }
            else if (node is FileInstance fileEntry)
            {
                Interlocked.Increment(ref files);
                dir = fileEntry.ParentId;
                file = fileEntry[nameof(FileInfo.Name)].ValueToString;
            }

            if (left < 0)
            {
                ShowMessage("Processing filesystem...");
                (left, top) = Console.GetCursorPosition();
            }

            formalDir = $"{prefix}{dir}";
            formalFile = $"{filePrefix}{file}";

            var rightMargin = Console.BufferWidth - 4;
            var blankLine = new string(' ', rightMargin + 3);
            formalDir = formalDir.Length > rightMargin ? $"{formalDir}..." : formalDir;
            formalFile = formalFile.Length > rightMargin ? $"{formalFile}..." : formalFile;

            Console.SetCursorPosition(left, top);
            Console.WriteLine($"{blankLine}{Environment.NewLine}{blankLine}{Environment.NewLine}{blankLine}");
            Console.SetCursorPosition(left, top);

            ShowMessage($"Processed {files} files in {directories} directories...");
            ShowMessage(formalDir);
            ShowMessage(formalFile);
        }
    }
}
