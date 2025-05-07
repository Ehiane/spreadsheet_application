// <copyright file="ICommand.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace SpreadSheetEngine
{
    /// <summary>
    /// Interface for the Command pattern.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Gets the name of the command.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// this method will be called to execute the command.
        /// </summary>
        void Execute();

        /// <summary>
        /// this method will be called to undo the command.
        /// </summary>
        void UnExecute();
    }
}
