// <copyright file="CommandTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace SpreadSheetTests
{
    using System;
    using System.Data;
    using NUnit.Framework;
    using SpreadSheetEngine;

    /// <summary>
    /// This class contains unit tests for the <see cref="ChangeTextCommand"/> and <see cref="ChangeBGColorCommand"/> classes.
    /// </summary>
    [TestFixture]
    internal class CommandTests
    {
        private ConcreteCell cell;
        private DataRow dataRow;
        private DataColumn dataColumn;
        private DataTable table;

        /// <summary>
        /// This method will be called before each test to set up the test environment.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            // Initialize a DataTable and add a column and a row for the cell
            this.table = new DataTable();
            this.dataColumn = new DataColumn("A"); // Example column name
            this.table.Columns.Add(this.dataColumn);
            this.dataRow = this.table.NewRow();
            this.table.Rows.Add(this.dataRow);

            // Initialize the ConcreteCell with the DataRow and DataColumn
            this.cell = new ConcreteCell(this.dataRow, this.dataColumn);

            // Set initial text and background color for testing
            this.cell.Text = "Old Text";
            this.cell.BGColor = 0xFFFFFF; // Initial background color (white)
        }

        /// <summary>
        /// This method will be called after each test to clean up the test environment.
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            // Clean up
            this.table.Dispose();
            this.dataColumn.Dispose();
        }

        // Tests for ChangeTextCommand

        /// <summary>
        /// This method tests the Execute and UnExecute methods of the <see cref="ChangeTextCommand"/> class.
        /// </summary>
        [Test]
        public void TestExecuteAndUnExecute_NormalCase_Text()
        {
            string initialText = this.cell.Text;
            string newText = "New Text";
            var command = new ChangeTextCommand(this.cell, newText);

            command.Execute();
            string afterExecute = this.cell.Text;

            command.UnExecute();
            string afterUnExecute = this.cell.Text;

            Assert.That(afterExecute, Is.EqualTo(newText), "Execute should update the cell text.");
            Assert.That(afterUnExecute, Is.EqualTo(initialText), "UnExecute should revert the cell text to the original value.");
        }

        /// <summary>
        /// This method tests the Execute and UnExecute methods of the <see cref="ChangeTextCommand"/> class.
        /// </summary>
        [Test]
        public void TestExecuteAndUnExecute_BoundaryCase_Text()
        {
            string initialText = this.cell.Text;
            string emptyText = string.Empty;
            var command = new ChangeTextCommand(this.cell, emptyText);

            command.Execute();
            string afterExecute = this.cell.Text;

            command.UnExecute();
            string afterUnExecute = this.cell.Text;

            Assert.That(afterExecute, Is.EqualTo(emptyText), "Execute should set the cell text to an empty string.");
            Assert.That(afterUnExecute, Is.EqualTo(initialText), "UnExecute should revert the cell text to the original value.");
        }

        /// <summary>
        /// This method tests the Execute and UnExecute methods of the <see cref="ChangeTextCommand"/> class.
        /// </summary>
        [Test]
        public void TestExecuteAndUnExecute_ExceptionalCase_Text()
        {
            ConcreteCell? nullCell = null;
            string newText = "New Text";

            Assert.Throws<ArgumentNullException>(() => new ChangeTextCommand(nullCell, newText), "Constructor should throw ArgumentNullException for null cell.");
        }

        // Tests for ChangeBGColorCommand

        /// <summary>
        /// This method tests the Execute and UnExecute methods of the <see cref="ChangeBGColorCommand"/> class.
        /// </summary>
        [Test]
        public void TestExecuteAndUnExecute_NormalCase_BGColor()
        {
            uint initialColor = 0xFFFFFF; // White
            uint newColor = 0xFF0000; // Red
            this.cell.BGColor = initialColor;
            var command = new ChangeBGColorCommand(this.cell, newColor);

            command.Execute();
            Assert.That(this.cell.BGColor, Is.EqualTo(newColor), "Execute should update the cell background color.");

            command.UnExecute();
            Assert.That(this.cell.BGColor, Is.EqualTo(initialColor), "UnExecute should revert the cell background color to the original value.");
        }

        /// <summary>
        /// This method tests the Execute and UnExecute methods of the <see cref="ChangeBGColorCommand"/> class.
        /// </summary>
        [Test]
        public void TestExecuteAndUnExecute_BoundaryCase_BGColor()
        {
            uint defaultColor = 16777215; // White
            uint? newColor = null; // Testing with null as a boundary case
            var command = new ChangeBGColorCommand(this.cell, newColor);

            command.Execute();
            Assert.That(this.cell.BGColor, Is.EqualTo(defaultColor), "Execute should set the cell background color to default when newColor is null.");

            command.UnExecute();
            Assert.That(this.cell.BGColor, Is.EqualTo(defaultColor), "UnExecute should revert the cell background color to the original value.");
        }

        /// <summary>
        /// This method tests the Execute and UnExecute methods of the <see cref="ChangeBGColorCommand"/> class.
        /// </summary>
        [Test]
        public void TestExecuteAndUnExecute_ExceptionalCase_BGColor()
        {
            ConcreteCell? nullCell = null;
            uint newColor = 0x00FF00; // Green

            Assert.Throws<ArgumentNullException>(() => new ChangeBGColorCommand(nullCell, newColor), "Constructor should throw ArgumentNullException for null cell.");
        }
    }
}
