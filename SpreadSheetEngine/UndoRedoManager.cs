// <copyright file="UndoRedoManager.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace SpreadSheetEngine
{
    using System.Collections.Generic;

    /// <summary>
    /// this class represents a manager for the undo and redo operations.
    /// </summary>
    internal class UndoRedoManager
    {
        private Stack<ICommand> undoStack = new Stack<ICommand>();
        private Stack<ICommand> redoStack = new Stack<ICommand>();

        /// <summary>
        /// this method will execute a command and push it to the undo stack.
        /// </summary>
        /// <param name="command">The command to be executed.</param>
        public void ExecuteCommand(ICommand command)
        {
            command.Execute();
            this.undoStack.Push(command);
            this.redoStack.Clear();
        }

        /// <summary>
        /// this method will undo the last command.
        /// </summary>
        public void Undo()
        {
            if (this.undoStack.Count > 0)
            {
                ICommand command = this.undoStack.Pop();
                command.UnExecute();
                this.redoStack.Push(command);
            }
        }

        /// <summary>
        /// this method will redo the last command.
        /// </summary>
        public void Redo()
        {
            if (this.redoStack.Count > 0)
            {
                ICommand command = this.redoStack.Pop();
                command.Execute();
                this.undoStack.Push(command);
            }
        }
    }
}
