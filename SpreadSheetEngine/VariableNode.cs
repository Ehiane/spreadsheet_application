// <copyright file="VariableNode.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SpreadSheetEngine
{
    /// <summary>
    /// A Node that represent the name of a value in an expression.
    /// </summary>
    internal class VariableNode : Node
    {
        /// <summary>
        /// A dictionary that holds the name of the variable and its value.
        /// </summary>
        private Dictionary<string, double> variableReference = new Dictionary<string, double>();

        /// <summary>
        /// Initializes a new instance of the <see cref="VariableNode"/> class.
        /// </summary>
        /// <param name="reference"> external dictionary that holds variable info.</param>
        public VariableNode(Dictionary<string, double> reference)
        {
            this.variableReference = reference;
        }

        /// <summary>
        /// Gets or Sets the name of the variable node.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///  returns the value of the variable node.
        /// </summary>
        /// <returns> the value of the variable node.</returns>
        public override double Evaluate()
        {
            if (!this.variableReference.ContainsKey(this.Name) || double.IsNaN(this.variableReference[this.Name]))
            {
                throw new KeyNotFoundException($"Variable '{this.Name}' has not been set or does not exist.");
            }

            return this.variableReference[this.Name];
        }
    }
}
