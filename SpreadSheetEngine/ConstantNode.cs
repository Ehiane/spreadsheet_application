// <copyright file="ConstantNode.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SpreadSheetEngine
{
    /// <summary>
    /// A Node that represents a constant numerical value.
    /// </summary>
    internal class ConstantNode(double value)
        : Node
    {
        /// <summary>
        /// Gets or sets the value of the constant node.
        /// </summary>
        public double Value { get; set; } = value;

        /// <summary>
        /// Evaluates the value of the constant node.
        /// </summary>
        /// <returns> the value of the constant node.</returns>
        public override double Evaluate()
        {
            return this.Value;
        }
    }
}
