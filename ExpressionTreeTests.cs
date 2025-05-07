// <copyright file="ExpressionTreeTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SpreadSheetTests
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using SpreadSheetEngine;

    /// <summary>
    /// This class contains unit tests for the <see cref="ExpressionTree"/> class.
    /// </summary>
    public class ExpressionTreeTests
    {
        private ExpressionTree expressionTree;

        /// <summary>
        /// Initializes a new <see cref="ExpressionTree"/> before each test, with a default expression and sets a variable.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            // Ensure variable is set before each test
            this.expressionTree = new ExpressionTree("A1+12-3");
            this.expressionTree.SetVariable("A1", 5);
        }

        /// <summary>
        /// Tests if the <see cref="ExpressionTree"/> correctly evaluates a valid expression with a variable and constants.
        /// </summary>
        [Test]
        public void Evaluate_WithValidExpression()
        {
            this.expressionTree.SetVariable("A1", 5);
            double result = this.expressionTree.Evaluate();
            Assert.That(result, Is.EqualTo(14)); // 5 + 12 - 3 = 14
        }

        /// <summary>
        /// Tests if the <see cref="ExpressionTree"/> correctly parses and evaluates a simple expression with constants.
        /// </summary>
        [Test]
        public void ParseExpression_WithSimpleExpression()
        {
            this.expressionTree = new ExpressionTree("3+2");
            double result = this.expressionTree.Evaluate();
            Assert.That(result, Is.EqualTo(5)); // 3 + 2 = 5
        }

        /// <summary>
        /// Tests if the <see cref="ExpressionTree"/> evaluates an expression with a variable set to zero.
        /// </summary>
        [Test]
        public void Evaluate_WithZeroValue()
        {
            this.expressionTree = new ExpressionTree("A1 - 2");
            this.expressionTree.SetVariable("A1", 0);
            double result = this.expressionTree.Evaluate();
            Assert.That(result, Is.EqualTo(-2)); // 0 - 2 = -2
        }

        /// <summary>
        /// Tests if the <see cref="ExpressionTree"/> throws a <see cref="KeyNotFoundException"/> when trying to evaluate an expression with an undefined variable.
        /// </summary>
        [Test]
        public void Evaluate_WithUnknownVariable()
        {
            Assert.Throws<KeyNotFoundException>(() =>
            {
                this.expressionTree = new ExpressionTree("A1 + 5");
                this.expressionTree.Evaluate(); // A1 is not defined
            });
        }

        /// <summary>
        /// Tests if the <see cref="ExpressionTree"/> throws a <see cref="DivideByZeroException"/> when trying to divide by zero.
        /// </summary>
        [Test]
        public void Evaluate_WithDivideByZero()
        {
            this.expressionTree = new ExpressionTree("10/0");
            Assert.Throws<DivideByZeroException>(() =>
            {
                this.expressionTree.Evaluate(); // Should throw DivideByZeroException
            });
        }

        /// <summary>
        /// Tests if the <see cref="ExpressionTree"/> throws an <see cref="InvalidOperationException"/> when given an invalid expression.
        /// </summary>
        [Test]
        public void ParseExpression_WithInvalidExpression()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                this.expressionTree = new ExpressionTree("2^2");
                this.expressionTree.Evaluate();  // Should throw InvalidOperationException
            });
        }

        /// <summary>
        /// Tests if the <see cref="ExpressionTree"/> throws a <see cref="KeyNotFoundException"/> when trying to evaluate with an unset variable.
        /// </summary>
        [Test]
        public void Evaluate_ThrowsForUnsetVariable()
        {
            // Create a new expression with an unset variable
            this.expressionTree = new ExpressionTree("A1 + 12 - 3");

            // Expect an exception if we attempt to evaluate without setting the variable
            Assert.Throws<KeyNotFoundException>(() =>
            {
                double result = this.expressionTree.Evaluate();  // Should throw due to A1 not being set
            });
        }

        /// <summary>
        /// Tests if the <see cref="ExpressionTree"/> correctly evaluates an expression when the variable is set.
        /// </summary>
        [Test]
        public void Evaluate_WithSetVariable_ReturnsCorrectValue()
        {
            // Create a new expression with a variable
            this.expressionTree = new ExpressionTree("A1 + 12 - 3");

            // Set the variable
            this.expressionTree.SetVariable("A1", 5);

            // Evaluate the expression
            double result = this.expressionTree.Evaluate();

            // Assert the correct value
            Assert.That(result, Is.EqualTo(14));  // 5 + 12 - 3 = 14
        }

        [Test]
        /// <summary>
        ///  test case to ensure that the expression tree can handle negative numbers.
        /// </summary>
        public void Evaluate_WithOperatorPrecedence()
        {
            // Test case to enssure multiplication has higher precedence than addition
            this.expressionTree = new ExpressionTree("3 + 4 * 2");
            double result1 = this.expressionTree.Evaluate();

            // Test case to ensure addition has higher precedence than multiplication
            this.expressionTree = new ExpressionTree("3 * 4 + 2");
            double result2 = this.expressionTree.Evaluate();

            Assert.That(result1, !Is.EqualTo(result2));
        }

        [Test]
        public void ParseExpression_WithMismatchedParentheses_ThrowsException()
        {
           // Test case for unmatched or missing closing parenthesis
           Assert.Throws<InvalidOperationException>(() =>
           {
               this.expressionTree = new ExpressionTree("(3 + 4");
               this.expressionTree.Evaluate();  // Should throw InvalidOperationException
           });

            // Test case for unmatched or missing opening parenthesis
           Assert.Throws<InvalidOperationException>(() =>
           {
                this.expressionTree = new ExpressionTree("3 + 4)");
                this.expressionTree.Evaluate();  // Should throw InvalidOperationException
           });
        }


    }
}
