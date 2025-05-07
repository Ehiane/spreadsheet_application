// <copyright file="CompositeCommand.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace SpreadSheetEngine
{
    using System.Collections.Generic;

    /// <summary>
    /// this class represents a composite command that contains multiple commands.
    /// </summary>
    public class CompositeCommand : ICommand
    {
        private readonly List<ICommand> commands;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeCommand"/> class.
        /// </summary>
        /// <param name="commands">List of grouped commands.</param>
        public CompositeCommand(IEnumerable<ICommand> commands)
        {
            this.commands = new List<ICommand>(commands);
        }

        /// <summary>
        /// Gets the name of the command.
        /// </summary>
        public string Name => "Change Background Color of Multiple Cells";

        /// <summary>
        /// this method will be called to execute the command.
        /// </summary>
        public void Execute()
        {
            foreach (var command in this.commands)
            {
                command.Execute();
            }
        }

        /// <summary>
        /// this method will be called to undo the command.
        /// </summary>
        public void UnExecute()
        {
            foreach (var command in this.commands.AsEnumerable().Reverse())
            {
                command.UnExecute();
            }
        }
    }
}
