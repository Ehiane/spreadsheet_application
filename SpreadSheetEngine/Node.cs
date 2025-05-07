// <copyright file="Node.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SpreadSheetEngine
{
    /// <summary>
    /// A node that represents a value in an expression, this is the base class.
    /// </summary>
    internal abstract class Node
    {
        /// <summary>
        /// Evaluates the value of the node.
        /// </summary>
        /// <returns> the value of the node as a double.</returns>
        public abstract double Evaluate();
    }
}
