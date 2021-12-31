// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using FileQueryDatabase.Database;
using FileQueryDatabase.Services;

namespace FileQueryDatabase.Commands
{
    /// <summary>
    /// Help.
    /// </summary>
    public class HelpCommand : IFileQueryCommand, IHelpText
    {
        private IHelpText[] commands;

        /// <summary>
        /// Initializes a new instance of the <see cref="HelpCommand"/> class.
        /// </summary>
        /// <param name="ignore">Ignored service provider.</param>
        public HelpCommand(IFileQueryServiceProvider ignore)
        {
        }

        /// <summary>
        /// Gets the command text.
        /// </summary>
        public string CommandText => "help";

        /// <summary>
        /// Gets a value indicating whether the command starts with the command text and has additional parameters.
        /// </summary>
        public bool StartsWith => true;

        /// <summary>
        /// Gets the name for help.
        /// </summary>
        public string HelpName => "Help";

        /// <summary>
        /// Gets the description for help.
        /// </summary>
        public string HelpDescription => "Shows help for individual commands.";

        /// <summary>
        /// Gets the list of parameters.
        /// </summary>
        public (string parameter, string name, string description)[] HelpParameters =>
            commands.OrderBy(c => c.HelpName)
            .Select(c => (c.CommandText, c.HelpName, c.HelpDescription))
            .ToArray();

        /// <summary>
        /// Gets the examples.
        /// </summary>
        public (string code, string description)[] HelpExamples =>
            new[]
            {
                ("help", "Lists the commands available to get help on."),
                ("help filter", "Provides help for the filter command."),
            };

        /// <summary>
        /// Implementation of help.
        /// </summary>
        /// <param name="db">The <see cref="FileDatabase"/> instance.</param>
        /// <param name="command">The user command.</param>
        /// <returns>A value indicating whether the program should terminate after running the command.</returns>
        public bool Execute(FileDatabase db, string command)
        {
            var gap = command.IndexOf(' ');
            if (gap >= 0)
            {
                ShowHelp(command[gap..].Trim());
                return false;
            }

            ConsoleManager.ShowMessage("Type 'help <command name>' for detailed help on a command.");
            ConsoleManager.ShowMessage("Command\tDescription");
            foreach (var help in commands.OrderBy(c => c.CommandText))
            {
                ConsoleManager.ShowMessage($"{help.HelpName}\t{help.HelpDescription}");
            }

            return false;
        }

        /// <summary>
        /// Register help files.
        /// </summary>
        /// <param name="enumerable">The list of help files.</param>
        internal void Register(IEnumerable<IHelpText> enumerable)
        {
            commands = enumerable.ToArray();
        }

        /// <summary>
        /// Parses parameters into an enumerable string source.
        /// </summary>
        /// <param name="parameters">The parameters to parse.</param>
        /// <returns>A string that yields each parameter value.</returns>
        private static IEnumerable<string> ParseParameters((string parameter, string name, string description)[] parameters)
        {
            foreach (var (parameter, name, description) in parameters)
            {
                yield return parameter;
                yield return name;
                yield return description;
            }
        }

        private void ShowHelp(string command)
        {
            var commandText = commands
                .Where(c => c.HelpName.StartsWith(command, StringComparison.InvariantCultureIgnoreCase))
                .FirstOrDefault();

            if (commandText == null)
            {
                throw new ArgumentException($"Help: invalid parameter: {command}.");
            }

            ConsoleManager.ShowMessage($"{Environment.NewLine}{commandText.HelpName}");
            ConsoleManager.ShowMessage($"{Environment.NewLine}{commandText.HelpDescription}");
            if (commandText.HelpParameters.Length > 0)
            {
                ConsoleManager.ShowMessage($"{Environment.NewLine}Parameters:");
                ConsoleManager.ShowColumns(3, ParseParameters(commandText.HelpParameters));
            }

            if (commandText.HelpExamples.Length > 0)
            {
                ConsoleManager.ShowMessage($"{Environment.NewLine}Example usages:{Environment.NewLine}");
                foreach (var (code, description) in commandText.HelpExamples)
                {
                    ConsoleManager.ShowMessage($"\"{code}\"");
                    ConsoleManager.ShowMessage(description);
                    ConsoleManager.ShowMessage(string.Empty);
                }
            }
        }
    }
}
