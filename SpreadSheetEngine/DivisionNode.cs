// <copyright file="DivisionNode.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SpreadSheetEngine
{
    using System;

    /// <summary>
    /// Represents a node that performs division in an expression tree.
    /// </summary>
    internal class DivisionNode : OperatorNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DivisionNode"/> class.
        /// </summary>
        public DivisionNode()
            : base('/')
        {
        }

        /// <summary>
        /// Gets the division operator symbol ('/').
        /// </summary>
        public static new char Operator => '/';

        /// <summary>
        /// Gets the precedence level of the division operation.
        /// Division has a precedence of 1, shared with multiplication.
        /// </summary>
        public static new int Precedence => 1;

        /// <summary>
        /// Gets the associativity of the division operation.
        /// Division is left-associative ('L'), meaning operations are evaluated from left to right.
        /// </summary>
        public static new char Associativity => 'L';

        /// <summary>
        /// Evaluates the division operation by dividing the left node's value by the right node's value.
        /// Throws a <see cref="DivideByZeroException"/> if the right node evaluates to zero.
        /// </summary>
        /// <returns>The result of the division.</returns>
        /// <exception cref="DivideByZeroException">Thrown when the right operand evaluates to zero.</exception>
        public override double Evaluate()
        {
            // Check if the right operand evaluates to zero before dividing
            double rightValue = this.Right.Evaluate();

            if (rightValue == 0)
            {
                throw new DivideByZeroException("Cannot divide by zero.");
            }

            return this.Left.Evaluate() / rightValue;
        }
    }
}
