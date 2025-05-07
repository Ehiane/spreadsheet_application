// <copyright file="NodeFactory.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SpreadSheetEngine
{
    using System.Collections.Generic;

    /// <summary>
    /// A class that creates nodes based on the token passed.
    /// </summary>
    internal class NodeFactory
    {
        /// <summary>
        /// Creates a node based on the token passed.
        /// </summary>
        /// <param name="token"> a string token. </param>
        /// <param name="variables"> a dictionary of variables. </param>
        /// <returns> a node based on the token passed. </returns>
        public static Node CreateNode(string token, Dictionary<string, double> variables)
        {
            OperatorNodeFactory operatorNodeFactory = new OperatorNodeFactory();

            // Check if the token is a valid constant (number)
            if (double.TryParse(token, out double constantValue))
            {
                return new ConstantNode(constantValue);
            }

            // Check if the token is a valid operator
            else if ("+-*/".Contains(token))
            {
                return operatorNodeFactory.CreateOperatorNode(token[0]);
            }

            // Check if the token is alphanumeric and valid as a variable name
            else if (char.IsLetter(token[0]))
            {
                foreach (char c in token)
                {
                    if (!char.IsLetterOrDigit(c))
                    {
                        throw new ArgumentException($"Invalid variable format: {token}");
                    }
                }

                return new VariableNode(variables) { Name = token };
            }

            // If the token is an invalid operator or contains invalid characters
            else
            {
                throw new ArgumentException($"Invalid token: {token}");
            }
        }
    }
}