// <copyright file="OperatorNode.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SpreadSheetEngine
{
    /// <summary>
    /// Represents an abstract operator node that serves as a base class for all operator nodes in the expression tree.
    /// </summary>
    internal abstract class OperatorNode : Node
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OperatorNode"/> class with a specified operator symbol.
        /// Sets the <see cref="Operator"/> property and initializes the <see cref="Left"/> and <see cref="Right"/> child nodes to null.
        /// </summary>
        /// <param name="sign">The operator symbol (e.g., '+', '-', '*', '/') associated with this node.</param>
        public OperatorNode(char sign)
        {
            Operator = sign;
            this.Left = null;
            this.Right = null;
        }

        /// <summary>
        /// Gets or sets the operator symbol for the node (e.g., '+', '-', '*', '/').
        /// This is a static property and represents the operator of the most recently created operator node.
        /// </summary>
        public static char Operator { get; set; }

        /// <summary>
        /// Gets the precedence level of the operator.
        /// Defines the order of operations; higher values indicate higher precedence.
        /// Must be overridden in derived classes to provide the appropriate precedence.
        /// </summary>
        public static int Precedence { get; }

        /// <summary>
        /// Gets the associativity of the operator.
        /// 'L' for left associativity (evaluates from left to right),
        /// 'R' for right associativity (evaluates from right to left).
        /// Must be overridden in derived classes to provide the appropriate associativity.
        /// </summary>
        public static char Associativity { get; }

        /// <summary>
        /// Gets or sets the left operand of the operator node.
        /// </summary>
        public Node Left { get; set; }

        /// <summary>
        /// Gets or sets the right operand of the operator node.
        /// </summary>
        public Node Right { get; set; }

        /// <summary>
        /// Evaluates the operator node.
        /// Must be implemented by subclasses to perform the specific operation.
        /// </summary>
        /// <returns>The result of evaluating the operator's expression.</returns>
        public abstract override double Evaluate();
    }
}
