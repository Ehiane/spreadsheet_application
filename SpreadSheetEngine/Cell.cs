// <copyright file="Cell.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SpreadSheetEngine
{
    using System.ComponentModel;
    using System.Data;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Represents an abstract base class for a cell in a spreadsheet.
    /// Implements the <see cref="INotifyPropertyChanged"/> interface to notify listerners of property changes.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="Cell"/> class with the specified row and column indices.
    /// </remarks>
    /// <param name="rowIndex">The row index of the cell.</param>
    /// <param name="colIndex">The column index of the cell.</param>
    public abstract class Cell(DataRow rowIndex, DataColumn colIndex)
        : INotifyPropertyChanged
    {
        /// <summary>
        /// The row index of the cell.
        /// </summary>
        protected readonly DataRow rowIndex = rowIndex;

        /// <summary>
        /// The column index of the cell.
        /// </summary>
        protected readonly DataColumn columnIndex = colIndex;

        /// <summary>
        /// The text content of the cell.
        /// </summary>
        protected string? text;

        /// <summary>
        /// The evaluated value of the cell.
        /// </summary>
        protected string? value;

        /// <summary>
        /// the background color of the cell.
        /// </summary>
        protected uint bgColor = 16777215; // default color is white

        /// <summary>
        /// Event triggered when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        /// <summary>
        /// Gets the row index of the cell.
        /// </summary>
        public DataRow RowIndex => this.rowIndex;

        /// <summary>
        /// Gets the row index of the cell.
        /// </summary>
        public DataColumn ColIndex => this.columnIndex;

        /// <summary>
        /// Gets or sets a value indicating whether the cell has been evaluated.
        /// Used to prevent re-evaluation of cells that have already been processed,
        /// particularly useful in tracking dependency calculations.
        /// </summary>
        public bool IsEvaluated { get; set; }

        public bool IsValueNotified { get; set; }

        public bool IsFormulaEvaluated { get; set; }

        /// <summary>
        /// Gets or sets the text content of the cell.
        /// Setting this property will trigger the <see cref="PropertyChanged"/> event and call <see cref="EvaluateValue"/>.
        /// </summary>
        public virtual string Text
        {
            get
            {
                return this.text ?? string.Empty; // incase the text is null return empty
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
        /// Gets or sets the evaluated value of the cell. This property is read-only to external classes.
        /// </summary>
        public virtual string Value
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
        /// </summary>
        public virtual uint BGColor
        {
            get { return this.bgColor; }
            set { this.bgColor = value; }
        }

        /// <summary>
        /// Notifies that the text content of the cell has changed.
        /// </summary>
        public void NotifyTextChanged()
        {
            this.OnPropertyChanged("Text");
        }

        /// <summary>
        /// Evaluates the value of the cell based on the text content.
        /// </summary>
        public void NotifyValueChanged()
        {
            this.OnPropertyChanged("Value");
            this.IsValueNotified = true;
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
