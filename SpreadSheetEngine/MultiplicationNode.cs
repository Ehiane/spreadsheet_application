// <copyright file="MultiplicationNode.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SpreadSheetEngine
{
    /// <summary>
    /// Represents a node in the expression tree that performs a multiplication operation.
    /// </summary>
    internal class MultiplicationNode : OperatorNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MultiplicationNode"/> class.
        /// </summary>
        public MultiplicationNode()
            : base('*')
        {
        }

        /// <summary>
        /// Gets the multiplication operator symbol.
        /// Represents the operator used for multiplication in expressions.
        /// </summary>
        public static new char Operator => '*';

        /// <summary>
        /// Gets the precedence level of the multiplication operation.
        /// Determines the order of operations when evaluating expressions.
        /// Multiplication has a precedence of 1.
        /// </summary>
        public static new int Precedence => 1;

        /// <summary>
        /// Gets the associativity of the multiplication operation.
        /// 'L' represents left associativity, meaning operations are evaluated from left to right.
        /// </summary>
        public static new char Associativity => 'L';

        /// <summary>
        /// Evaluates the multiplication operation by multiplying the results of the left and right nodes.
        /// </summary>
        /// <returns>The result of the multiplication operation.</returns>
        public override double Evaluate()
        {
            return this.Left.Evaluate() * this.Right.Evaluate();
        }
    }
}
