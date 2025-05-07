// <copyright file="SpreadSheet.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SpreadSheetEngine
{
    using System.ComponentModel;
    using System.Data;
    using System.Diagnostics; // Ensure this using statement is present
    using System.Globalization;
    using System.Text.RegularExpressions;
    using System.Xml;

    /// <summary>
    /// Represents a spreadsheet that contains a 2D array of cells and handles cell operations.
    /// Provides methods to manipulate and retrieve cell data.
    /// </summary>
    public class SpreadSheet
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpreadSheet"/> class with the specified number of rows and columns.
        /// </summary>
        /// <param name="rows">The number of rows in the spreadsheet.</param>
        /// <param name="columns">The number of columns in the spreadsheet.</param>
        public SpreadSheet(int rows, int columns)
        {
            // Initialize the DataTable
            this.table = new DataTable();

            // Initialize the DataTable with the specified rows and columns
            this.InitializeTable(rows, columns);

            // Initialize the 2D array of cells
            this.cells = new ConcreteCell[rows, columns];
            this.InitializeCells();
        }

        /// <summary>
        /// Event that triggers when a property of any cell in the spreadsheet changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanger = delegate { };

        /// <summary>
        /// Event that triggers when the spreadsheet is cleared.
        /// </summary>
        public event EventHandler SpreadsheetCleared;

        /// <summary>
        /// A 2D array that contains the cells in the spreadsheet.
        /// </summary>
        private ConcreteCell[,] cells;

        /// <summary>
        /// A DataTable to manage the rows and columns in the spreadsheet.
        /// </summary>
        private DataTable table;

        /// <summary>
        /// A dictionary that contains the dependencies between cells.
        /// </summary>
        private Dictionary<Cell, List<Cell>> cellDependencies = new Dictionary<Cell, List<Cell>>();

        /// <summary>
        /// Gets the DataTable that represents the spreadsheet.
        /// </summary>
        private Stack<ICommand> undoStack = new Stack<ICommand>();

        /// <summary>
        /// Gets the DataTable that represents the spreadsheet.
        /// </summary>
        private Stack<ICommand> redoStack = new Stack<ICommand>();

        /// <summary>
        /// Backing field for CanUndo property.
        /// </summary>
        private bool canUndo;

        /// <summary>
        /// Backing field for CanRedo property.
        /// </summary>
        private bool canRedo;

        private readonly HashSet<ConcreteCell> cellsBeingEvaluated = new HashSet<ConcreteCell>();

        /// <summary>
        /// Gets a value indicating whether the undo operation can be performed or sets it (privately).
        /// </summary>
        public bool CanUndo
        {
            get => this.canUndo;
            private set => this.canUndo = value;
        }

        /// <summary>
        /// Gets a value indicating whether a value indicating whether the redo operation can be performed or sets it (privately).
        /// </summary>
        public bool CanRedo
        {
            get => this.canRedo;
            private set => this.canRedo = value;
        }

        /// <summary>
        /// Gets the number of rows in the spreadsheet.
        /// </summary>
        public int RowCount => this.cells.GetLength(0);

        /// <summary>
        /// Gets the number of columns in the spreadsheet.
        /// </summary>
        public int ColumnCount => this.cells.GetLength(1);

        /// <summary>
        /// Converts a DataRow and DataColumn to a spreadsheet-style cell name (e.g., A1, B2).
        /// </summary>
        /// <param name="row">The DataRow containing the cell.</param>
        /// <param name="column">The DataColumn containing the cell.</param>
        /// <returns>The cell name as a string in the form of "A1", "B2", etc.</returns>
        public static string GetCellName(DataRow row, DataColumn column)
        {
            int rowIndex = row.Table.Rows.IndexOf(row); // Get the row index
            int columnIndex = column.Ordinal; // Get the column index
            char columnLetter = (char)('A' + columnIndex);
            return $"{columnLetter}{rowIndex + 1}";
        }

        /// <summary>
        /// Executes a command and adds it to the undo stack.
        /// Clears the redo stack, as redo history is no longer valid after a new command.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        public void PerformCommand(ICommand command)
        {
            command.Execute();
            this.undoStack.Push(command);
            this.redoStack.Clear();
            this.UpdateCanUndoRedo();
        }

        /// <summary>
        /// Undoes the last command, moving it to the redo stack.
        /// </summary>
        public void Undo()
        {
            if (this.CanUndo)
            {
                ICommand command = this.undoStack.Pop();
                command.UnExecute();
                this.redoStack.Push(command);
                this.UpdateCanUndoRedo();
            }
        }

        /// <summary>
        /// Redoes the last undone command, moving it back to the undo stack.
        /// </summary>
        public void Redo()
        {
            if (this.CanRedo)
            {
                ICommand command = this.redoStack.Pop();
                command.Execute();
                this.undoStack.Push(command);
                this.UpdateCanUndoRedo();
            }
        }

        /// <summary>
        /// this method will return the name of the last undo action.
        /// </summary>
        /// <returns> The name of the last command.</returns>
        public string PeekUndoActionName()
        {
            if (this.undoStack.Count > 0)
            {
                return this.undoStack.Peek().Name;
            }

            return string.Empty;
        }

        /// <summary>
        /// this method will return the name of the last redo action.
        /// </summary>
        /// <returns> The name of the current command.</returns>
        public string PeekRedoActionName()
        {
            if (this.redoStack.Count > 0)
            {
                return this.redoStack.Peek().Name;
            }

            return string.Empty;
        }

        /// <summary>
        /// Handles changes in cell properties by updating the cell's value and triggering the <see cref="PropertyChanger"/> event.
        /// If the cell contains a formula (starts with "="), the value is retrieved from the referenced cell.
        /// </summary>
        /// <param name="sender">The cell that triggered the event.</param>
        /// <param name="e">The event arguments containing the property name that changed.</param>
        public void CellPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ConcreteCell cell = (ConcreteCell)sender;

            if (e.PropertyName == "Text" && cell.Text.StartsWith("="))
            {
                // Extract the referemces from the formula and update the dependencies
                this.UpdateDependencies(cell);

                // Evaluate the cell based on the formula
                this.EvaluateFormula(cell);

                // Notify all the dependent cells to update their values
                if (this.ContainsDependencies(cell.Text))
                {
                    this.NotifyDependentCells(cell);
                }
            }
            else if (e.PropertyName == "Value" && cell.Value == "!ERROR")
            {
                this.EvaluateFormula(cell);

                // Notify all the dependent cells to update their values
                if (this.ContainsDependencies(cell.Text))
                {
                    this.NotifyDependentCells(cell);
                }
            }
            else
            {
                this.NotifyDependentCells(cell);

                if (!cell.IsEvaluated)
                {
                    // Set the value to the text if it's not a formula
                    cell.Value = cell.Text;
                    this.PropertyChanger(this, new PropertyChangedEventArgs("Value"));
                }

                if (!cell.Text.StartsWith("=") && (cell.Text == string.Empty & cell.Value != string.Empty))
                {
                    cell.Value = cell.Text;
                }

                this.PropertyChanger(this, new PropertyChangedEventArgs("Value"));
            }
        }

        /// <summary>
        /// Retrieves the cell located at the specified row and column.
        /// </summary>
        /// <param name="row">The row index of the cell.</param>
        /// <param name="column">The column index of the cell.</param>
        /// <returns>The <see cref="Cell"/> at the specified row and column.</returns>
        public Cell GetCell(int row, int column)
        {
            return this.cells[row, column];
        }

        /// <summary>
        /// notifies all dependent cells of the cell at the specified row and column index.
        /// </summary>
        /// <param name="rowIndex"> Index to represent the row.</param>
        /// <param name="columnIndex"> Index to represent the column.</param>
        public void NotifyDependentCellsByIndex(int rowIndex, int columnIndex)
        {
            // Retrieve the cell based on row and column index
            var cell = this.GetCell(rowIndex, columnIndex) as ConcreteCell;

            // Ensure the cell is valid and call the private NotifyDependentCells method
            if (cell != null)
            {
                this.NotifyDependentCells(cell);
            }
        }

        /// <summary>
        ///  Method to save the spreadsheet to an XML file.
        /// </summary>
        /// <param name="filePath">the file path to be saved in.</param>
        public void SaveToFile(string filePath)
        {
            XmlDocument document = new XmlDocument();
            XmlElement root = document.CreateElement("Spreadsheet");
            document.AppendChild(root);

            var modifiedCells = this.GetModifiedCells();
            foreach (var cell in modifiedCells)
            {
                XmlElement cellElement = document.CreateElement("Cell");

                // Add the cell's row as a child element
                XmlElement rowElement = document.CreateElement("Row");
                rowElement.InnerText = cell.RowIndex is DataRow dataRow ? dataRow.Table.Rows.IndexOf(dataRow).ToString() : cell.RowIndex?.ToString() ?? string.Empty;
                cellElement.AppendChild(rowElement);

                // Add the cell's column as a child element
                XmlElement columnElement = document.CreateElement("Column");
                columnElement.InnerText = cell.ColIndex is DataColumn dataColumn ? dataColumn.ColumnName : cell.ColIndex?.ToString() ?? string.Empty;
                cellElement.AppendChild(columnElement);

                // Add the cell's text as a child element
                XmlElement textElement = document.CreateElement("Text");
                textElement.InnerText = cell.Text;
                cellElement.AppendChild(textElement);

                // Add the cell's value as a child element
                XmlElement valueElement = document.CreateElement("Value");
                valueElement.InnerText = cell.Value;
                cellElement.AppendChild(valueElement);

                // Add the cell's background color as a child element
                XmlElement colorElement = document.CreateElement("BGColor");
                colorElement.InnerText = cell.BGColor.ToString();
                cellElement.AppendChild(colorElement);

                // Add the cell element to the root
                root.AppendChild(cellElement);
            }

            document.Save(filePath);
        }

        /// <summary>
        /// Clears all cells in the spreadsheet by resetting their text, value, and background color.
        /// </summary>
        public void Clear()
        {
            for (int row = 0; row < this.RowCount; row++)
            {
                for (int col = 0; col < this.ColumnCount; col++)
                {
                    var cell = this.GetCell(row, col) as ConcreteCell;
                    if (cell != null)
                    {
                        cell.Text = string.Empty;
                        cell.Value = string.Empty;
                        cell.BGColor = 16777215; // default color
                    }
                }
            }

            // Optionally clear any undo/redo stacks if you have them
            this.undoStack.Clear();
            this.redoStack.Clear();

            // Notify any listeners if needed
            this.NotifySpreadsheetCleared();
        }

        /// <summary>
        /// Loads the spreadsheet from an existing XML file.
        /// </summary>
        /// <param name="filepath">the file path to be saved in.</param>
        public void LoadImproved(string filepath)
        {
            if (!File.Exists(filepath))
            {
                throw new FileNotFoundException("File not found", filepath);
            }

            XmlDocument document = new XmlDocument();
            document.Load(filepath);
            XmlNodeList cellNodes = document.DocumentElement?.SelectNodes("Cell");

            // Temporary storage for cells with formulas that have unresolved dependencies
            var cellsWithFormulas = new List<(ConcreteCell cell, string formula)>();

            // First pass: load all simple values and queue formulas
            foreach (XmlNode cellNode in cellNodes)
            {
                int rowIndex = int.Parse(cellNode["Row"]!.InnerXml);
                int columnIndex = this.ColumnLetterToIndex(cellNode["Column"].InnerXml);
                string text = cellNode["Text"].InnerText;
                uint bgColor = uint.Parse(cellNode["BGColor"].InnerXml);

                // retrieve or create the cell
                var cell = this.GetCell(rowIndex, columnIndex) as ConcreteCell;

                cell.BGColor = bgColor;

                if (this.IsFormulaWithReference(text))
                {
                    // Queue the cell for a second pass / evaluation for later
                    cellsWithFormulas.Add((cell, text));
                }
                else
                {
                    // simple text or numeric value, directly assign
                    cell.Text = text;
                    cell.Value = text;
                }
            }

            // Second pass: evaluate formulas with dependencies
            bool anyChanges;

            do
            {
                anyChanges = false;

                foreach (var (cell, formula) in cellsWithFormulas.ToList())
                {
                    if (this.DependenciesResolved(formula))
                    {
                        cell.Text = formula;
                        cell.Value = this.EvaluateFormula(cell).ToString(CultureInfo.InvariantCulture);
                        cellsWithFormulas.Remove((cell, formula));
                        anyChanges = true;
                    }
                }
            }
            while (anyChanges && cellsWithFormulas.Any());

            if (cellsWithFormulas.Any())
            {
                throw new InvalidOperationException("Circular dependency detected.");
            }
        }

        /// <summary>
        /// Notifies listeners that the spreadsheet has been cleared.
        /// </summary>
        protected virtual void NotifySpreadsheetCleared()
        {
            // Raise the event if there are any subscribers
            this.SpreadsheetCleared?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Loads the spreadsheet from an existing XML file.
        /// </summary>
        /// <param name="formula">The formula to be examined.</param>
        /// <returns>Indicator about if the dependency has been resolved.</returns>
        private bool DependenciesResolved(string formula)
        {
            // define a reglar expression to match cell references in the form of letters followed by numbers
            var cellReferencePattern = new Regex(@"[A-Z]+\d+");

            // find all matches for cell references in the formula
            var matches = cellReferencePattern.Matches(formula);

            foreach (Match macth in matches)
            {
                // Extract the cell reference, e.g., "A1", "B2"
                string cellReference = macth.Value;

                // convert cell reference to row and column indices
                int columnIndex = this.ColumnLetterToIndex(cellReference.Substring(0, 1)); // get the column index
                int rowIndex = this.GetRowPart(cellReference) - 1;

                // get the cell from the spreadsheet
                var referencedCell = this.GetCell(rowIndex, columnIndex) as ConcreteCell;

                // check if the referenced cell doesn't have a value
                if (string.IsNullOrEmpty(referencedCell.Value))
                {
                    return false; // dependency not resolved
                }
            }

            return true; // all dependencies are resolved
        }

        /// <summary>
        /// Gets the row part of a cell reference, e.g., "A1" returns 1.
        /// </summary>
        /// <param name="cellReference">The cell reference in the format of letters followed by numbers, e.g., "A1".</param>
        /// <returns>The row part of the cell reference as an integer.</returns>
        private int GetRowPart(string cellReference)
        {
            string rowPart = new string(cellReference.SkipWhile(char.IsLetter).ToArray());
            return int.Parse(rowPart);
        }

        /// <summary>
        /// This method converts a column letter to a zero-based index.
        /// </summary>
        /// <param name="columnLetter">The letter representing the column (e.g., "A" for the first column).</param>
        /// <returns>The zero-based index of the column.</returns>
        private int ColumnLetterToIndex(string columnLetter)
        {
            int index = 0;

            for (int i = 0; i < columnLetter.Length; i++)
            {
                index *= 26;
                index += (columnLetter[i] - 'A' + 1);
            }

            return index - 1;
        }

        /// <summary>
        /// This method checks if the formula contains dependencies.
        /// </summary>
        /// <param name="text">The text to be checked if it contains a formula with references.</param>
        /// <returns>True if the formula's dependencies are resolved; otherwise, false.</returns>
        private bool IsFormulaWithReference(string text)
        {
            // Check if it starts with "="
            if (this.IsSimpleNumberFormula(text))
            {
                return false;
            }

            // Define a regex pattern to match cell references
            // This matches patterns like =A1, =B2+C3, etc.
            Regex cellReferencePattern = new Regex(@"=.*[A-Za-z]+\d+");

            // Check if the formula contains any valid cell reference
            return cellReferencePattern.IsMatch(text);
        }

        private bool IsSimpleNumberFormula(string text)
        {
            // Return true if text matches the pattern =<number> only
            Regex simpleNumberPattern = new Regex(@"^=\d+$");
            return simpleNumberPattern.IsMatch(text);
        }

        /// <summary>
        /// Gets cells that have been modified.
        /// </summary>
        /// <returns> A list of cells that have been altered.</returns>
        private List<ConcreteCell> GetModifiedCells()
        {
            List<ConcreteCell> modifiedCells = new List<ConcreteCell>();

            // Iterate through all cells in the spreadsheet
            for (int row = 0; row < this.RowCount; row++)
            {
                for (int col = 0; col < this.ColumnCount; col++)
                {
                    var cell = this.GetCell(row, col) as ConcreteCell;

                    // Check if the cell has been modified
                    uint defaultColor = 16777215; // Default color of the cell
                    if (!string.IsNullOrEmpty(cell.Text) || cell.BGColor != defaultColor)
                    {
                        // Cell has been modified
                        modifiedCells.Add(cell);
                    }
                }
            }

            return modifiedCells;
        }

        /// <summary>
        /// this method checks if the formula contains dependencies.
        /// </summary>
        /// <param name="formula">formula to be evaluated.</param>
        /// <returns>indicator whether evaluated or not.</returns>
        private bool ContainsDependencies(string formula)
        {
            // Check if the formula is in the format "=<number>"
            if (formula.StartsWith("="))
            {
                string expression = formula.Substring(1); // Remove '='
                return !double.TryParse(expression, out _); // Returns true if it contains non-numeric characters
            }

            return false; // Not a formula
        }

        /// <summary>
        /// Updates the CanUndo and CanRedo properties based on the current state of the stacks.
        /// </summary>
        private void UpdateCanUndoRedo()
        {
            this.CanUndo = this.undoStack.Count > 0;
            this.CanRedo = this.redoStack.Count > 0;
        }

        /// <summary>
        /// Initializes the DataTable by adding the specified number of rows and columns.
        /// Column names are set as alphabetical letters (A, B, C, etc.).
        /// </summary>
        /// <param name="rows">The number of rows to add.</param>
        /// <param name="columns">The number of columns to add.</param>
        private void InitializeTable(int rows, int columns)
        {
            // Add columns to the DataTable with alphabetical column names
            for (int col = 0; col < columns; col++)
            {
                string columnName = ((char)('A' + col)).ToString();
                this.table.Columns.Add(new DataColumn(columnName));
            }

            // Add rows to the DataTable
            for (int row = 0; row < rows; row++)
            {
                this.table.Rows.Add(this.table.NewRow());
            }
        }

        /// <summary>
        /// Initializes the 2D array of cells and subscribes to their property changes.
        /// </summary>
        private void InitializeCells()
        {
            for (int row = 0; row < this.cells.GetLength(0); row++) // 0 represents the rows
            {
                for (int column = 0; column < this.cells.GetLength(1); column++) // 1 represents the columns
                {
                    DataRow datarow = this.table.Rows[row];
                    DataColumn datacolumn = this.table.Columns[column];

                    ConcreteCell newCell = new ConcreteCell(datarow, datacolumn);

                    // Subscribe to the PropertyChanged event for the new cell
                    newCell.PropertyChanged += this.CellPropertyChanged;

                    // Add the new cell to the 2D array
                    this.cells[row, column] = newCell;
                }
            }
        }

        /// <summary>
        /// Updates the dependencies for a cell with a formula by parsing its references
        /// and storing each dependent cell in the dictionary for tracking purposes.
        /// </summary>
        /// <param name="cell">The cell with a formula whose dependencies need updating.</param>
        private void UpdateDependencies(ConcreteCell cell)
        {
            // Parse the formula and find the dependencies, e.g., "=A1 + B2"
            // Add these dependencies to cellDependencies for reactive updating
            var referencedCells = this.ParseReferences(cell.Text);

            var visitedCells = new HashSet<Cell>();

            foreach (var referencedCell in referencedCells)
            {
                // detect circular dependencies
                this.DetectCircularReference(cell, visitedCells);

                // Ensure referencedCell is a key in cellDependencies
                if (!this.cellDependencies.ContainsKey(referencedCell))
                {
                    this.cellDependencies[referencedCell] = new List<Cell>();
                }

                // Now safely check and add the dependency
                if (!this.cellDependencies[referencedCell].Contains(cell))
                {
                    this.cellDependencies[referencedCell].Add(cell);
                }
            }
        }

        /// <summary>
        /// Parses a formula string in a cell's text to extract references to other cells.
        /// Returns a list of cells referenced in the formula for dependency tracking.
        /// </summary>
        /// <param name="formula">The formula string to parse for cell references.</param>
        /// <returns>A list of cells referenced in the formula.</returns>
        private List<Cell> ParseReferences(string formula)
        {
            var referencedCells = new List<Cell>();

            // Regular expression to match cell references, e.g., "A1", "B2"
            var cellReferencePattern = new Regex(@"[A-Z]+\d+");

            // Find all matches for cell references in the formula
            foreach (Match match in cellReferencePattern.Matches(formula))
            {
                string cellName = match.Value;

                // Retrieve the cell by its name
                var cell = this.GetCellByName(cellName) as ConcreteCell;

                if (cell != null)
                {
                    referencedCells.Add(cell);
                }
                else
                {
                    throw new InvalidOperationException($"Referenced cell '{cellName}' does not exist.");
                }
            }

            return referencedCells;
        }

        /// <summary>
        /// Retrieves a cell based on its name (e.g., "A1").
        /// </summary>
        /// <param name="name">The name of the cell, such as "A1" or "B2".</param>
        /// <returns>The ConcreteCell at the specified location.</returns>
        /// <exception cref="ArgumentException">Thrown if the cell name is invalid.</exception>
        private ConcreteCell GetCellByName(string name)
        {
            if (string.IsNullOrEmpty(name) || name.Length < 2)
            {
                throw new ArgumentException("Invalid cell name format.", nameof(name));
            }

            // Extract column letters and row number from the name
            int column = 0;
            int row = 0;

            int index = 0;

            // Loop through each character in the `name` string until we reach a non-letter character or the end of the string.
            // This loop is responsible for calculating the column index if the column part of the cell name is more than one letter (e.g., "AA", "BC").
            while (index < name.Length && char.IsLetter(name[index]))
            {
                // Convert the current character to uppercase to ensure case consistency ('a' or 'A' both work the same).
                // Calculate the position in the alphabet: 'A' corresponds to 1, 'B' to 2, ..., 'Z' to 26.
                // For example, 'A' - 'A' + 1 = 1, 'B' - 'A' + 1 = 2, ..., 'Z' - 'A' + 1 = 26.
                // We add this value to the column index calculation.
                // This formula treats each letter as a "digit" in a base-26 numbering system.
                // So for "AA," the calculation becomes: column = 0 * 26 + 1 (for 'A') = 1; column = 1 * 26 + 1 (for the second 'A') = 27.
                column = (column * 26) + (char.ToUpper(name[index]) - 'A' + 1);

                // Move to the next character in the string.
                index++;
            }

            if (index < name.Length && int.TryParse(name.Substring(index), out row))
            {
                // Convert from 1-based row/column to 0-based for internal array indexing
                column -= 1;
                row -= 1;

                // Use GetCell method to retrieve the cell
                return (ConcreteCell)this.GetCell(row, column);
            }
            else
            {
                throw new ArgumentException("Invalid cell name format.", nameof(name));
            }
        }

        /// <summary>
        /// Notifies all cells dependent on the specified cell, prompting them to re-evaluate
        /// based on the updated value of the referenced cell.
        /// </summary>
        /// <param name="cell">The cell whose dependents need to be updated.</param>
        private void NotifyDependentCells(ConcreteCell cell)
        {
            // check if the cell is already being evaluated to avoid infinite loops
            if (this.cellsBeingEvaluated.Contains(cell))
            {
                return;
                //throw new InvalidOperationException($"Circular reference detected in cell {GetCellName(cell.RowIndex, cell.ColIndex)}.");
            }

            this.cellsBeingEvaluated.Add(cell);
            try
            {
                // Check if cells has dependents
                if (this.cellDependencies.TryGetValue(cell, out var dependents) && cell.Value != "!Self-Reference Error") // might need to add or if the value of the key exists
                {
                    // Notify all dependent cells to update their values
                    foreach (var dependent in dependents)
                    {
                        this.EvaluateFormula((ConcreteCell)dependent);
                        if (this.cellsBeingEvaluated.Count < this.cellDependencies.Count)
                        {
                            this.NotifyDependentCells((ConcreteCell)dependent);
                        }
                    }
                }
            }
            finally
            {
                // Remove the cell from the evaluation set after processing
                this.cellsBeingEvaluated.Remove(cell);
            }
        }

        /// <summary>
        /// Evaluates the formula in the specified cell's text if it begins with "=".
        /// Parses the formula, retrieves the values of referenced cells, and calculates the result.
        /// The result is assigned to the cell's value.
        /// </summary>
        /// <param name="cell">The cell whose formula is to be evaluated.</param>
        /// <returns>The evaluated result of the formula as a double.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the formula is invalid or contains unsupported operations.
        /// </exception>
        private double EvaluateFormula(ConcreteCell cell)
        {
            try
            {
                // Clear previous error state
                if (cell.Value.Contains("ERROR"))
                {
                    cell.Value = "0"; // Reset to default value
                    cell.Text = string.Empty;
                }

                string formula = cell.Text.Substring(1); // Remove the "=" sign

                string currentCellName = GetCellName(cell.RowIndex, cell.ColIndex);

                if (formula.Contains(currentCellName))
                {
                    cell.IsEvaluated = true;
                    throw new SelfReferenceException($"Self-reference detected in cell {currentCellName}.");
                }

                // Detect circular references
                this.DetectCircularReference(cell, new HashSet<Cell>());

                // Create an expression tree from the formula
                ExpressionTree expressionTree = new ExpressionTree(formula);

                // Set the variables in the expression tree based on the dependencies
                var referencedCells = this.ParseReferences(formula);
                foreach (var referencedCell in referencedCells)
                {
                    // if the referenced cell's text is not a formula and the value is a number, set the referenced cell's value as the text.
                    if (!this.IsSimpleNumberFormula(referencedCell.Text) && !this.IsFormulaWithReference(referencedCell.Text))
                    {
                        if (referencedCell.Text != string.Empty)
                        {
                            referencedCell.Value = referencedCell.Text;
                        }
                    }

                    if (referencedCell.Value == string.Empty)
                    {
                        referencedCell.Value = "0";
                        cell.Value = "0";
                        cell.IsEvaluated = true;
                        //cell.NotifyValueChanged();
                        return 0;
                    }

                    if (referencedCell.Text == string.Empty && referencedCell.Value == "0")
                    {
                        referencedCell.Text = referencedCell.Value;
                    }

                    string cellName = GetCellName(referencedCell.RowIndex, referencedCell.ColIndex);

                    expressionTree.SetVariable(cellName, double.Parse(referencedCell.Value.ToString()));
                }

                // Evaluate the expression tree
                double result = expressionTree.Evaluate();

                // Update the cell's value with the evaluated result
                cell.Value = result.ToString();

                cell.IsEvaluated = true;

                // Trigger the PropertyChanged event to notify the UI
                if (cell.IsValueNotified == false)
                {
                    cell.NotifyValueChanged();
                }

                return result;
            }
            catch (SelfReferenceException ex)
            {
                // Set the cell's value to "!ERROR" to indicate a problem
                cell.Value = "!Self-Reference Error";
                cell.NotifyValueChanged();

                // Bubble up the exception to the UI layer
                throw new InvalidOperationException($"Error in cell {GetCellName(cell.RowIndex, cell.ColIndex)}: {ex.Message}");
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Circular reference"))
            {
                cell.Value = "!Circular Reference Error";
                throw new InvalidOperationException($"Error in cell {GetCellName(cell.RowIndex, cell.ColIndex)}: Circular reference detected.");
            }
            catch (Exception ex) // error handling
            {
                // Set the cell's value to "!ERROR" to indicate a problem
                cell.Value = "!ERROR";
                cell.NotifyValueChanged();

                // Bubble up the exception to the UI layer
                throw new InvalidOperationException($"Error in cell {GetCellName(cell.RowIndex, cell.ColIndex)}: {ex.Message}");
            }
        }

        /// <summary>
        /// This method will detect circular reference in the spreadsheet.
        /// </summary>
        /// <param name="cell">The cell to check if there's a circular reference.</param>
        /// <param name="visitedCells"> Cells that have been visited.</param>
        /// <exception cref="InvalidOperationException"> Triggers the circular reference error.</exception>
        private void DetectCircularReference(Cell cell, HashSet<Cell> visitedCells)
        {
            if (visitedCells.Contains(cell) && visitedCells.Count > 1)
            {
                throw new InvalidOperationException("Circular reference detected.");
            }
            else
            {
                return;
            }
        }
    }
}