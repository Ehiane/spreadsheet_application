// <copyright file="ConcreteCell.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SpreadSheetEngine
{
    using System;
    using System.Data;

    /// <summary>
    /// Represents a concrete implementation of a cell in the spreadsheet.
    /// Inherits from the abstract <see cref="Cell"/> class and provides implementations for properties and behaviors.
    /// </summary>
    internal class ConcreteCell : Cell
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConcreteCell"/> class.
        /// </summary>
        /// <param name="rowIndex">The DataRow that represents the row of this cell.</param>
        /// <param name="colIndex">The DataColumn that represents the column of this cell.</param>
        public ConcreteCell(DataRow rowIndex, DataColumn colIndex)
            : base(rowIndex, colIndex)
        {
        }

        /// <summary>
        /// Gets or sets the text content of the cell.
        /// This property triggers the <see cref="OnPropertyChanged"/> event when its value changes.
        /// </summary>
        public override string Text
        {
            get
            {
                return this.text ?? string.Empty;
            }

            set
            {
                if (this.text != value)
                {
                    this.text = value;
                    this.OnPropertyChanged("Text");
                }
            }
        }

        /// <summary>
        /// Gets or sets the evaluated value of the cell.
        /// This property represents the computed value of the cell and triggers the <see cref="OnPropertyChanged"/> event when it is changed.
        /// </summary>
        public new string Value
        {
            get
            {
                return this.value ?? string.Empty;
            }

            set
            {
                this.value = value;
            }
        }

        /// <summary>
        /// Gets or sets the background color of the cell.
        /// Setting the color triggers the PropertyChanged event.
        /// </summary>
        public override uint BGColor
        {
            get
            {
                return this.bgColor;
            }

            set
            {
                if (this.bgColor != value)
                {
                    this.bgColor = value;
                    this.OnPropertyChanged("BGColor"); // Notify any UI or listeners of the color change.
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the cell has been evaluated.
        /// Used to prevent re-evaluation of cells that have already been processed,
        /// particularly useful in tracking dependency calculations.
        /// </summary>
        public new bool IsEvaluated { get; set; }
    }
}