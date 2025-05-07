// <copyright file="SubtractionNode.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SpreadSheetEngine
{
    /// <summary>
    /// Represents a node in the expression tree that performs a subtraction operation.
    /// </summary>
    internal class SubtractionNode : OperatorNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubtractionNode"/> class.
        /// </summary>
        public SubtractionNode()
            : base('-')
        {
        }

        /// <summary>
        /// Gets the subtraction operator symbol.
        /// Represents the operator used for multiplication in expressions.
        /// </summary>
        public static new char Operator => '-';

        /// <summary>
        /// Gets the associativity of the subtraction operation.
        /// 'L' represents left associativity, meaning operations are evaluated from left to right.
        /// </summary>
        public static new int Precedence => 2;

        /// <summary>
        /// Gets the associativity of the subtraction operation.
        /// 'L' represents left associativity, meaning operations are evaluated from left to right.
        /// </summary>
        public static new char Associativity => 'L';

        /// <summary>
        /// Evaluates the subtraction operation by subtracting the result of the right node from the left node.
        /// </summary>
        /// <returns>The result of the subtraction operation.</returns>
        public override double Evaluate()
        {
            return this.Left.Evaluate() - this.Right.Evaluate();
        }
    }
}
