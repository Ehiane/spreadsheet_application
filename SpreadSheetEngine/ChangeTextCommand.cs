// <copyright file="ChangeTextCommand.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace SpreadSheetEngine
{
    /// <summary>
    /// Represents a command to change the text of a cell.
    /// </summary>
    public class ChangeTextCommand : ICommand
    {
        /// <summary>
        /// Gets or sets the cell to change the text of.
        /// </summary>
        public Cell Cell;
        private string oldText;
        private string newText;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeTextCommand"/> class.
        /// </summary>
        /// <param name="cell">The cell to change the text of.</param>
        /// <param name="newText">The new text to apply to the cell.</param>
        public ChangeTextCommand(Cell cell, string newText)
        {
            if (cell == null)
            {
                throw new ArgumentNullException(nameof(cell), "Cell cannot be null.");
            }

            this.Cell = cell;
            this.newText = newText;
            this.oldText = cell.Text;
            this.Name = "Change Text";
        }

        /// <summary>
        /// Gets property represents the name of the command.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// this method will be called to execute the command.
        /// </summary>
        public void Execute()
        {
            this.Cell.Text = this.newText;
            if (!this.Cell.Text.StartsWith("="))
            {
                this.Cell.Value = this.Cell.Text;
            }
        }

        /// <summary>
        /// this method will be called to undo the command.
        /// </summary>
        public void UnExecute()
        {
            this.Cell.Text = this.oldText;

            // Notify the spreadsheet engine to re-evaluate the cell
            this.Cell.NotifyTextChanged();
            this.Cell.IsEvaluated = false;
            this.Cell.NotifyValueChanged();
        }
    }
}