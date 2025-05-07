// <copyright file="ChangeBGColorCommand.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace SpreadSheetEngine
{
    /// <summary>
    /// this class represents a command to change the background color of a cell.
    /// </summary>
    public class ChangeBGColorCommand : ICommand
    {
        /// <summary>
        /// the default color of the cell.
        /// </summary>
        public uint DefaultColor = 16777215; // got this from debugging the color of the cell

        private Cell cell;
        private uint? oldColor;
        private uint? newColor;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeBGColorCommand"/> class.
        /// </summary>
        /// <param name="cell">The cell to change the text of.</param>
        /// <param name="newColor">The new Color to apply to the cell.</param>
        public ChangeBGColorCommand(Cell cell, uint? newColor)
        {
            this.cell = cell ?? throw new ArgumentNullException(nameof(cell));
            this.newColor = newColor;
            this.oldColor = cell.BGColor;
        }

        /// <summary>
        /// Gets the name of the command.
        /// </summary>
        public string Name => "Change Background Color";

        /// <summary>
        /// this method will be called to execute the command.
        /// </summary>
        public void Execute()
        {
            this.cell.BGColor = this.newColor ?? this.DefaultColor;
        }

        /// <summary>
        /// this method will be called to undo the command.
        /// </summary>
        public void UnExecute()
        {
            this.cell.BGColor = this.oldColor ?? this.DefaultColor;
        }
    }
}
