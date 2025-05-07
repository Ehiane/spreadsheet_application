// <copyright file="SpreadSheetTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SpreadSheetTests
{
    using NUnit.Framework;
    using SpreadSheetEngine;
    using System.Data;

    /// <summary>
    /// Contains unit tests for the <see cref="SpreadSheet"/> class.
    /// Verifies the correct functionality of various operations on a spreadsheet.
    /// </summary>
    public class SpreadSheetTests
    {
        /// <summary>
        /// The maximum number of columns in the spreadsheet.
        /// </summary>
        private readonly int maxColumns = 26;

        /// <summary>
        /// The maximum number of rows in the spreadsheet.
        /// </summary>
        private readonly int maxRows = 50;

        /// <summary>
        /// The spreadsheet instance to be tested.
        /// </summary>
        private SpreadSheet spreadSheet;

        /// <summary>
        /// Sets up the spreadsheet instance before each test.
        /// Initializes a new spreadsheet with predefined rows and columns.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.spreadSheet = new SpreadSheet(this.maxRows, this.maxColumns);
        }

        /// <summary>
        /// Tests that every cell in the spreadsheet is initialized.
        /// Verifies that all cells in the 2D array of cells are not null.
        /// </summary>
        [Test]
        public void TestSpreadsheetArrayFilledWithCells()
        {
            for (int row = 0; row < this.maxRows; row++)
            {
                for (int col = 0; col < this.maxColumns; col++)
                {
                    // checking that every cell in the array is initialized
                    Assert.NotNull(this.spreadSheet.GetCell(row, col));
                }
            }
        }

        /// <summary>
        /// Tests that changing the text of a cell updates its value.
        /// Verifies that when the text of a cell is changed, the cell's value reflects the same.
        /// </summary>
        [Test]
        public void TestChangingCellTextChangesCellValue()
        {
            // changing the text of a cell
            Cell? cell = this.spreadSheet.GetCell(0, 0);
            cell.Text = "Hello";

            // the value should match the text
            Assert.That(cell.Value, Is.EqualTo("Hello"));
        }

        /// <summary>
        /// Tests that a cell can reference another cell's value.
        /// Verifies that when one cell refers to another (e.g., "=A1"), it correctly reflects the value of the referenced cell.
        /// </summary>
        [Test]
        public void TestCellReferencesAnotherCell()
        {
            Cell cellA1 = this.spreadSheet.GetCell(0, 0);
            Cell cellB1 = this.spreadSheet.GetCell(1, 0);

            // set value in one cell and reference it in another
            cellA1.Text = "5";
            cellB1.Text = "=A1";

            // the value of the second cell should be the same as the first
            Assert.That(cellB1.Value, Is.EqualTo("5"));
        }

        /// <summary>
        /// Tests that the <see cref="Cell.PropertyChanged"/> event is fired when the text of a cell changes.
        /// Verifies that changing the cell's text triggers the PropertyChanged event.
        /// </summary>
        [Test]
        public void PropertyChanged_Fired_WhenTextChanges()
        {
            var cell = this.spreadSheet.GetCell(0, 0); // get the first cell

            bool propertyChangedCalled = false;

            cell.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "Text")
                {
                    propertyChangedCalled = true;
                }
            };

            cell.Text = "new value"; // changing the text of the cell

            Assert.That(propertyChangedCalled, Is.True);
        }

        /// <summary>
        /// Tests that the <see cref="Cell.PropertyChanged"/> event is not fired if the text does not change.
        /// Verifies that setting the text to the same value does not trigger the PropertyChanged event.
        /// </summary>
        [Test]
        public void PrpoertyChanged_NotFired_WhenCellTextDoesNotChange()
        {
            var cell = this.spreadSheet.GetCell(0, 0); // get the first cell

            cell.Text = "initial value";

            bool propertyChangedCalled = false;

            cell.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "Value")
                {
                    propertyChangedCalled = true;
                }
            };

            cell.Text = "initial value"; // changing the text of the cell to the same value

            Assert.That(propertyChangedCalled, Is.False);
        }

        /// <summary>
        /// This test verifies that the <see cref="SpreadSheet.SaveToFile"/> and <see cref="SpreadSheet.LoadFromFile"/> methods work correctly.
        /// </summary>
        [Test]
        public void SaveAndLoad_NormalCase()
        {
            string filePath = "testSpreadsheet.xml";

            // Modify a few cells
            var cellA1 = this.spreadSheet.GetCell(0, 0) as ConcreteCell;
            cellA1.Text = "5";
            cellA1.BGColor = 255;

            var cellB2 = this.spreadSheet.GetCell(1, 1) as ConcreteCell;
            cellB2.Text = "=A1+1";
            cellB2.BGColor = 16711680;

            // Save and Load
            this.spreadSheet.SaveToFile(filePath);
            this.spreadSheet.Clear();
            this.spreadSheet.LoadImproved(filePath);

            // verify that the modified cells are still the same
            Assert.That(cellA1.Text, Is.EqualTo("5"));
            Assert.That(cellA1.BGColor, Is.EqualTo(255));
            Assert.That(cellB2.Text, Is.EqualTo("=A1+1"));
            Assert.That(cellB2.BGColor, Is.EqualTo(16711680));
        }

        /// <summary>
        /// Tests the boundary case of saving and loading a spreadsheet.
        /// </summary>
        [Test]
        public void SaveAndLoad_BoundaryCase()
        {
            string filePath = "boundaryTestSpreadsheet.xml";
            int lastRow = this.spreadSheet.RowCount - 1;
            int lastCol = this.spreadSheet.ColumnCount - 1;

            // Modify the boundary cell
            var boundaryCell = this.spreadSheet.GetCell(lastRow, lastCol) as ConcreteCell;
            boundaryCell.Text = "BoundaryTest";
            boundaryCell.BGColor = 65280; // Some color

            // Save and load
            this.spreadSheet.SaveToFile(filePath);
            this.spreadSheet.Clear();
            this.spreadSheet.LoadImproved(filePath);

            // Verify boundary cell data
            Assert.That(this.spreadSheet.GetCell(lastRow, lastCol).Text, Is.EqualTo("BoundaryTest"));
            Assert.That(this.spreadSheet.GetCell(lastRow, lastCol).BGColor, Is.EqualTo(65280));

            // Clean up
            File.Delete(filePath);
        }

        /// <summary>
        /// This test verifies that the <see cref="SpreadSheet.LoadFromFile"/> method throws an exception when the file does not exist.
        /// </summary>
        [Test]
        public void LoadFromFile_ExceptionCase()
        {
            string nonExistentFile = "nonExistentFile.xml";

            // attempt to load the file that doesn't exist
            Assert.Throws<FileNotFoundException>(() => this.spreadSheet.LoadImproved(nonExistentFile));
        }

        /// <summary>
        /// This test verifies that the <see cref="SpreadSheet.LoadFromFile"/> method throws an exception when the file is invalid.
        /// </summary>
        [Test]
        public void EvaluateFormula_WithUnassignedReference_ReturnsDefaultValue()
        {
            DataTable dataTable = new DataTable();

            dataTable.Columns.Add("A");
            dataTable.Columns.Add("B");

            // Add rows to the spreadsheet
            for (int i = 0; i < 10; i++)
            {
                dataTable.Rows.Add(dataTable.NewRow());
            }

            // Fetch Datarow and Datacolumn
            DataRow rowA1 = dataTable.Rows[0]; // First row (A1, B1, etc.)
            DataRow rowB1 = dataTable.Rows[0]; // Second row (A2, B2, etc.)
            DataColumn columnA = dataTable.Columns["A"];
            DataColumn columnB = dataTable.Columns["B"];

            this.spreadSheet.GetCell(0, 0).Text = "=B1 + 5"; // A1 references B1
            this.spreadSheet.GetCell(0, 1).Text = string.Empty; // B1 is unassigned

            double result = this.spreadSheet.GetCellValue(rowA1, columnA); // Evaluate A1

            Assert.That(result, Is.EqualTo(5)); // B1 is unassigned, so the default value is 0
        }

    }
}
