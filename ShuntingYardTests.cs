// <copyright file="ShuntingYardTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SpreadSheetTests
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using NUnit.Framework;
    using SpreadSheetEngine;

    /// <summary>
    /// This class contains unit tests for the ShuntingYard class.
    /// </summary>
    [TestFixture]
    internal class ShuntingYardTests
    {
        // ====================================
        // Normal Case
        // ====================================

        /// <summary>
        /// Tests the ShuntingYard_ConvertToPostfix method with a simple expression.
        /// </summary>
        [Test]
        public void ShuntingYard_ConvertToPostfix_SimpleExpression_ReturnsCorrectPostfix()
        {
            string expression = "3 + 4 * 2";
            var tokens = this.Tokenize(expression);
            var variables = new Dictionary<string, double>(); // Empty dictionary for this case
            var result = ShuntingYard.ConvertToPostfix(tokens, variables);

            var expected = new Queue<string>(new string[] { "3", "4", "2", "*", "+" }); // 3 4 2 * +
            Assert.That(result, Is.EqualTo(expected));
        }

        // ====================================
        // Boundary Case
        // ====================================

        /// <summary>
        /// Tests if the Shunting Yard algorithm correctly handles an empty expression.
        /// </summary>
        [Test]
        public void ConvertToPostfix_EmptyExpression_ReturnsEmptyQueue()
        {
            // Boundary case: an empty expression should result in an empty queue.
            var tokens = this.Tokenize(string.Empty);
            var variables = new Dictionary<string, double>(); // Empty dictionary for this case
            var result = ShuntingYard.ConvertToPostfix(tokens, variables);

            Assert.That(result.Count, Is.EqualTo(0)); // Ensure the output queue is empty
        }

        // ====================================
        // Exceptional Case
        // ====================================

        /// <summary>
        /// Tests if the tokenization throws an exception for invalid tokens.
        /// </summary>
        [Test]
        public void Tokenize_InvalidToken_ThrowsException()
        {
            // Exceptional case: invalid tokens like '@' should throw an exception.
            Assert.Throws<InvalidOperationException>(() => this.Tokenize("3 + @ 4"));
        }

        // ====================================
        // Helper Methods
        // ====================================

        /// <summary>
        /// Helper function to tokenize the expression string into a list of tokens.
        /// </summary>
        /// <param name="expression">The infix expression string.</param>
        /// <returns>A list of tokens representing the components of the expression.</returns>
        private List<string> Tokenize(string expression)
        {
            var tokens = new List<string>();

            // Using regex to identify numbers, variables, and operators
            var numberPattern = @"(\d+(\.\d+)?)"; // Matches a number with or without a decimal point
            var variablePattern = @"([A-Za-z][A-Za-z0-9]*)"; // Matches a variable name
            var operatorPattern = @"[+\-*/]"; // Matches supported operators

            // Regex pattern combining the valid patterns
            var regexPattern = $"{numberPattern}|{variablePattern}|{operatorPattern}";
            var regex = new Regex(regexPattern);

            // Iterate through the expression string and match the regex pattern
            foreach (Match match in regex.Matches(expression))
            {
                tokens.Add(match.Value);
            }

            // Check for unsupported tokens by finding any characters that weren't matched by the regex
            string remainingExpression = regex.Replace(expression, string.Empty);
            if (!string.IsNullOrWhiteSpace(remainingExpression))
            {
                throw new InvalidOperationException($"Unrecognized operator or token: {remainingExpression}");
            }

            return tokens;
        }
    }
}
