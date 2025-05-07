// <copyright file="AdditionNode.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SpreadSheetEngine
{
    /// <summary>
    /// Represents a node in the expression tree that performs an addition operation.
    /// </summary>
    internal class AdditionNode : OperatorNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AdditionNode"/> class.
        /// </summary>
        public AdditionNode()
            : base('+')
        {
        }

        /// <summary>
        /// Gets the addition operator symbol ('+').
        /// </summary>
        public static new char Operator => '+';

        /// <summary>
        /// Gets the precedence level of the addition operation.
        /// Addition has a precedence of 2, shared with subtraction.
        /// </summary>
        public static new int Precedence => 2;

        /// <summary>
        /// Gets the associativity of the addition operation.
        /// Addition is left-associative ('L'), meaning operations are evaluated from left to right.
        /// </summary>
        public static new char Associativity => 'L';

        /// <summary>
        /// Evaluates the addition operation by adding the results of the left and right nodes.
        /// </summary>
        /// <returns>The result of the addition operation.</returns>
        public override double Evaluate()
        {
            return this.Left.Evaluate() + this.Right.Evaluate();
        }
    }
}
