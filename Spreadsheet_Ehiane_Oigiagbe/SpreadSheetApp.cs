// <copyright file="SpreadSheetApp.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Spreadsheet_Ehiane_Oigiagbe
{
    using System.ComponentModel;
    using SpreadSheetEngine;

    /// <summary>
    /// The main form for the spreadsheet application.
    /// Handles the UI interactions and manages the underlying spreadsheet logic.
    /// </summary>
    public partial class SpreadSheetApp : Form
    {
        /// <summary>
        /// The spreadsheet object which contains the logic and data for the cells.
        /// </summary>
        private SpreadSheet spreadSheet;
        private bool isUndoInProgress = false;
        private bool isRedoInProgress = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpreadSheetApp"/> class.
        /// Sets up the form and initializes the spreadsheet and data grid.
        /// </summary>
        public SpreadSheetApp()
        {
            this.InitializeComponent();

            // Set up the event handlers for the menu items
            this.undoToolStripMenuItem.Click += this.UndoToolStripMenuItem_Click;
            this.undoToolStripMenuItem.Click -= this.UndoToolStripMenuItem_Click;
            this.redoToolStripMenuItem.Click += this.RedoToolStripMenuItem_Click;
            this.redoToolStripMenuItem.Click -= this.RedoToolStripMenuItem_Click;

            // Initialize the spreadsheet with 50 rows and 26 columns (A-Z)
            this.spreadSheet = new SpreadSheet(50, 26);

            // Subscribe to the CellPropertyChanged event
            this.spreadSheet.PropertyChanger += this.SpreadSheetPropertyChanged;

            this.spreadSheet.SpreadsheetCleared += this.OnSpreadsheetCleared;

            this.InitializeDataGridView();

            this.dataGridView1.CellBeginEdit += this.DataGridView1_CellBeginEdit;
            this.dataGridView1.CellEndEdit += this.DataGridView1_CellEndEdit;
        }

        /// <summary>
        ///  This method will be called when the spreadsheet is cleared.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An EventArgs that contains the event data.</param>
        private void OnSpreadsheetCleared(object sender, EventArgs e)
        {
            for (int row = 0; row < this.spreadSheet.RowCount; row++)
            {
                for (int col = 0; col < this.spreadSheet.ColumnCount; col++)
                {
                    this.UpdateCellAppearance(row, col);
                }
            }
        }

        /// <summary>
        /// Handles the CellBeginEdit event of the data grid view.
        /// Sets the displayed cell content to the underlying cell's Text (formula) when editing starts.
        /// </summary>
        private void DataGridView1_CellBeginEdit(object? sender, DataGridViewCellCancelEventArgs e)
        {
            // Get the cell in the spreadsheet corresponding to the cell being edited in the DataGridView
            var spreadsheetCell = this.spreadSheet.GetCell(e.RowIndex, e.ColumnIndex);

            // Display the Text property (the raw formula or input) in the DataGridView cell while editing
            this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = spreadsheetCell.Text;
        }

        /// <summary>
        /// Handles the CellEndEdit event of the data grid view.
        /// Updates the underlying cell's Text property with the user's input and shows the evaluated Value in the cell.
        /// </summary>
        private void DataGridView1_CellEndEdit(object? sender, DataGridViewCellEventArgs e)
        {
            try
            {
                // Get the cell in the spreadsheet corresponding to the cell being edited in the DataGridView
                var spreadsheetCell = this.spreadSheet.GetCell(e.RowIndex, e.ColumnIndex);

                // Get the user's input from the DataGridView cell
                string userInput = this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString() ?? string.Empty;

                // Create a command for changing the text, passing in the cell and new text
                var changeTextCommand = new ChangeTextCommand(spreadsheetCell, userInput);

                // Execute the command (this will push it onto the undo stack as well)
                this.PerformCommand(changeTextCommand);

                // Update the DataGridView cell with the evaluated Value of the cell after the command is executed
                this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = spreadsheetCell.Value;
            }
            catch (Exception ex) // error handling
            {
                if (ex.Message.Contains("Self"))
                {
                    this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "!SELF-REFERENCE ERROR";
                }
                else if (ex.Message.Contains("Circular"))
                {

                    this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "!CIRCULAR-REFERENCE ERROR";
                }
                else
                {
                    // Set the DataGridView cell's displayed value to "!ERROR"
                    this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "!ERROR";
                }

                // Display the error message in a MessageBox
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Event handler for the form load event. Currently not used.
        /// </summary>
        /// <param name="sender">The event source.</param>
        /// <param name="e">Event data.</param>
        private void SpreadSheetApp_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Initializes the columns of the data grid view programmatically, adding columns for letters A to Z.
        /// </summary>
        private void InitializeDataGridView()
        {
            // Loop through ASCII values for letters A (65) to Z (90)
            for (char c = 'A'; c <= 'Z'; c++)
            {
                // Create a new column for each letter
                DataGridViewColumn column = new DataGridViewColumn
                {
                    Name = c.ToString(),
                    HeaderText = c.ToString(),
                    Width = 50,
                    ReadOnly = false,
                    CellTemplate = new DataGridViewTextBoxCell(), // Setting the cell template to text box
                };

                // Add the column to the data grid
                this.dataGridView1.Columns.Add(column);
            }

            // Initialize rows for the grid
            this.InitializeRowsView();
        }

        /// <summary>
        /// Updates the DataGridView by refreshing the values in the grid from the spreadsheet.
        /// </summary>
        private void UpdateDataGridView()
        {
            for (int row = 0; row < this.spreadSheet.RowCount; row++)
            {
                for (int col = 0; col < this.spreadSheet.ColumnCount; col++)
                {
                    this.dataGridView1.Rows[row].Cells[col].Value = this.spreadSheet.GetCell(row, col).Value;
                }
            }
        }

        /// <summary>
        /// Updates the appearance of a cell in the DataGridView based on the corresponding cell in the spreadsheet.
        /// </summary>
        /// <param name="row">the row in particular. </param>
        /// <param name="column">the column in particular.</param>
        private void UpdateCellAppearance(int row, int column)
        {
            var cell = this.spreadSheet.GetCell(row, column);

            // Update text display
            this.dataGridView1.Rows[row].Cells[column].Value = cell.Value;

            uint defaultColor = 16777215;

            // Update background color
            if (cell.BGColor != defaultColor) // Assuming 0 as the default "no color" state
            {
                this.dataGridView1.Rows[row].Cells[column].Style.BackColor = Color.FromArgb((int)cell.BGColor);
            }
            else
            {
                this.dataGridView1.Rows[row].Cells[column].Style.BackColor = Color.White;
            }
        }

        /// <summary>
        /// Executes a command on the spreadsheet and updates the undo/redo menu.
        /// </summary>
        /// <param name="command">The command to be executed.</param>
        private void PerformCommand(ICommand command)
        {
            // Pass the command to the spreadsheet's PerformCommand method
            this.spreadSheet.PerformCommand(command);

            // Update the undo and redo menu items to reflect the current state
            this.UpdateUndoRedoMenu();
        }

        /// <summary>
        /// Initializes the rows of the data grid view programmatically, adding rows numbered 1 to 50.
        /// </summary>
        private void InitializeRowsView()
        {
            // Loop through the numbers 1 to 50
            for (int i = 1; i <= 50; i++)
            {
                // Create a new row for each number
                DataGridViewRow row = new DataGridViewRow
                {
                    HeaderCell = new DataGridViewRowHeaderCell
                    {
                        Value = i.ToString(),
                    },
                };

                // Add the row to the data grid
                this.dataGridView1.Rows.Add(row);
            }
        }

        /// <summary>
        /// Handles the CellPropertyChanged event from the spreadsheet.
        /// Updates the data grid view to reflect changes in the underlying spreadsheet.
        /// </summary>
        /// <param name="sender">The event source (cell).</param>
        /// <param name="e">The event data (property changed).</param>
        private void SpreadSheetPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Iterate through all rows and columns to update the UI based on spreadsheet data
            for (int rows = 0; rows < this.spreadSheet.RowCount; rows++)
            {
                for (int columns = 0; columns < this.spreadSheet.ColumnCount; columns++)
                {
                    // Update each cell in the data grid with the value from the corresponding cell in the spreadsheet
                    this.dataGridView1.Rows[rows].Cells[columns].Value = this.spreadSheet.GetCell(rows, columns).Value;
                }
            }
        }

        /// <summary>
        /// Handles the click event of the Demo button.
        /// Sets values in random cells, updates column B, and sets column A to column B values.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The event arguments.</param>
        private void Button1_Click(object sender, EventArgs e)
        {
            Random random = new Random();

            // Set "Hello World" in 50 random cells
            for (int i = 0; i < 50; i++)
            {
                int row = random.Next(0, 50);
                int col = random.Next(0, 26);
                this.spreadSheet.GetCell(row, col).Text = "Hello World";
            }

            // set the text in every cell in column B to "This is cell B#"
            for (int row = 0; row < this.spreadSheet.RowCount; row++)
            {
                this.spreadSheet.GetCell(row, 1).Text = "This is cell B" + (row + 1);
            }

            // set the value in each cell in column A to the value of the corresponding cell in column B
            for (int row = 0; row < this.spreadSheet.RowCount; row++)
            {
                this.spreadSheet.GetCell(row, 0).Text = $"=B{row + 1}";
            }

            // update the datagridview to reflect the changes
            this.UpdateDataGridView();
        }

        /// <summary>
        /// Changes the text of the selected cells in the spreadsheet and updates the appearance in the DataGridView.
        /// </summary>
        /// <param name="row">The row index of the cell.</param>
        /// <param name="column">The column index of the cell.</param>
        /// <param name="newText">The new text to set in the cell.</param>
        private void ChangeCellText(int row, int column, string newText)
        {
            var cell = this.spreadSheet.GetCell(row, column);
            var command = new ChangeTextCommand(cell, newText);
            this.PerformCommand(command);
        }

        /// <summary>
        /// Changes the background color of a cell in the spreadsheet and updates the appearance in the DataGridView.
        /// </summary>
        /// <param name="row">The row index of the cell.</param>
        /// <param name="column">The column index of the cell.</param>
        /// <param name="newColor">The new background color of the cell.</param>
        private void ChangeCellBackgroundColor(int row, int column, uint? newColor)
        {
            var cell = this.spreadSheet.GetCell(row, column);
            var command = new ChangeBGColorCommand(cell, newColor);
            this.PerformCommand(command);
        }

        /// <summary>
        /// this method will be called when the user clicks the undo button.
        /// </summary>
        private void UndoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.spreadSheet.Undo();
            this.RefreshAllCellsAppearance();
            this.UpdateUndoRedoMenu();
        }

        /// <summary>
        /// this method will be called when the user clicks the redo button.
        /// </summary>
        private void RedoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.spreadSheet.Redo();
            this.RefreshAllCellsAppearance();
            this.UpdateUndoRedoMenu();
        }

        /// <summary>
        /// Refreshes the appearance of all cells in the DataGridView to match the spreadsheet data.
        /// </summary>
        private void RefreshAllCellsAppearance()
        {
            for (int row = 0; row < this.spreadSheet.RowCount; row++)
            {
                for (int column = 0; column < this.spreadSheet.ColumnCount; column++)
                {
                    this.UpdateCellAppearance(row, column);
                    this.dataGridView1.Rows[row].Cells[column].Value = this.spreadSheet.GetCell(row, column).Value;
                }
            }
        }

        /// <summary>
        /// This method is for changing the background color of the selected cells.
        /// </summary>
        private void ChangeBackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    Color selectedColor = colorDialog.Color;
                    uint newColor = (uint)((selectedColor.A << 24) | (selectedColor.R << 16) | (selectedColor.G << 8) | selectedColor.B);

                    // Create a list of commands for each selected cell
                    var commands = new List<ICommand>();

                    foreach (DataGridViewCell selectedCell in this.dataGridView1.SelectedCells)
                    {
                        int row = selectedCell.RowIndex;
                        int column = selectedCell.ColumnIndex;
                        var cell = this.spreadSheet.GetCell(row, column);
                        commands.Add(new ChangeBGColorCommand(cell, newColor));
                    }

                    // Create a composite command
                    var compositeCommand = new CompositeCommand(commands);
                    this.PerformCommand(compositeCommand);

                    // Update the appearance of all affected cells
                    foreach (DataGridViewCell selectedCell in this.dataGridView1.SelectedCells)
                    {
                        int row = selectedCell.RowIndex;
                        int column = selectedCell.ColumnIndex;
                        this.UpdateCellAppearance(row, column);
                    }
                }
            }
        }

        /// <summary>
        /// this method will update the undo and redo menu items based on the current state of the spreadsheet.
        /// </summary>
        private void UpdateUndoRedoMenu()
        {
            if (this.spreadSheet.CanUndo)
            {
                this.undoToolStripMenuItem.Text = "Undo " + this.spreadSheet.PeekUndoActionName();
                this.undoToolStripMenuItem.Enabled = true;
            }
            else
            {
                this.undoToolStripMenuItem.Text = "Undo";
                this.undoToolStripMenuItem.Enabled = false;
            }

            if (this.spreadSheet.CanRedo)
            {
                this.redoToolStripMenuItem.Text = "Redo " + this.spreadSheet.PeekRedoActionName();
                this.redoToolStripMenuItem.Enabled = true;
            }
            else
            {
                this.redoToolStripMenuItem.Text = "Redo";
                this.redoToolStripMenuItem.Enabled = false;
            }
        }

        /// <summary>
        /// Houses the logic for the file menu item.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An EventArgs that contains the event data.</param>
        private void FileToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        /*
        Spreadsheet XML format:
        <Spreadsheet>
            <Cell>
                <Row>0</Row>
                <Column>1</Column>
                <Text>=A1+5</Text>
                <Value>5</Value>
                <BGColor>16777215</BGColor>
            </Cell>
            <Cell>
                <Row>0</Row>
                <Column>2</Column>
                <Text>=A1+5</Text>
                <Value>10</Value>
                <BGColor>16777215</BGColor>
            </Cell>
            ...
         */

        /// <summary>
        /// Applies the LoadSpreadsheet method to the spreadsheet object.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An EventArgs that contains the event data.</param>
        private void LoadSpreadsheetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Create an openFiledialog to ask the user for the file path to load the spreadsheet.
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "XML Files (*.xml)|*.xml";
                openFileDialog.Title = "Load Spreadsheet";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;

                    try
                    {
                        this.spreadSheet.LoadImproved(filePath);
                        this.RefreshAllCellsAppearance();
                        MessageBox.Show("Spreadsheet loaded successfully.", "Load", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error loading spreadsheet: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        /// <summary>
        /// Applies the SaveSpreadsheet method to the spreadsheet object.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An EventArgs that contains the event data.</param>
        private void SaveSpreadsheetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Create a SaveFileDialog to ask the user for the file path to save the spreadsheet
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "XML Files (*.xml)|*.xml";
                saveFileDialog.Title = "Save Spreadsheet";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = saveFileDialog.FileName;

                    try
                    {
                        // Call the save method in your Spreadsheet class
                        this.spreadSheet.SaveToFile(filePath);
                        MessageBox.Show("Spreadsheet saved successfully!", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error saving spreadsheet: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
