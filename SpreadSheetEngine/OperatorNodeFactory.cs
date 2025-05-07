// <copyright file="OperatorNodeFactory.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SpreadSheetEngine
{
    using System;
    using System.Reflection;

    /// <summary>
    /// A factory class responsible for creating instances of <see cref="OperatorNode"/> based on the operator symbol.
    /// </summary>
    internal class OperatorNodeFactory
    {
        private readonly Dictionary<char, Type> operators = [];

        /// <summary>
        /// Initializes a new instance of the <see cref="OperatorNodeFactory"/> class.
        /// Registers all available operator nodes by mapping their operator symbols to their respective types.
        /// </summary>
        public OperatorNodeFactory()
        {
            this.TraverseAvailableOperators((op, type) => this.operators.Add(op, type));
        }

        private delegate void OnOperator(char node, Type type);

        /// <summary>
        /// Creates and returns an instance of the appropriate <see cref="OperatorNode"/> subclass
        /// based on the operator symbol provided.
        /// </summary>
        /// <param name="op">The operator symbol (+, -, *, /) used in the expression.</param>
        /// <returns>An <see cref="OperatorNode"/> that corresponds to the given operator symbol.</returns>
        /// <exception cref="InvalidOperationException">Thrown when an unsupported operator symbol is passed.</exception>
        public OperatorNode CreateOperatorNode(char op)
        {
            if (this.operators.ContainsKey(op))
            {
                object? operatorNodeObject = Activator.CreateInstance(this.operators[op]);
                if (operatorNodeObject is OperatorNode)
                {
                    return (OperatorNode)operatorNodeObject;
                }
            }

            throw new Exception("Unhandled operator");
        }

        private void TraverseAvailableOperators(OnOperator onOperator)
        {
            // get the type declaration of OperatorNode
            Type operatorNodeType = typeof(OperatorNode);

            // get all types in the current assembly
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                // Get all types that inherit from our OperatorNode class using LINQ
                IEnumerable<Type> operatorTypes = assembly.GetTypes().Where(type => type.IsSubclassOf(operatorNodeType));

                // Itereate over those subclasses of OperatorNode
                foreach (var type in operatorTypes)
                {
                    // for each subclass, retrieve the Operator property
                    PropertyInfo? operatorField = type.GetProperty("Operator");
                    if (operatorField != null)
                    {
                        // Get the character value of the Operator
                        object? value = operatorField.GetValue(type);
                        if (value is char)
                        {
                            char operatorSymbol = (char)value;

                            onOperator(operatorSymbol, type);

                            // And invoke the function passed as a parameter
                            // with the operator symbol and the operator class
                        }
                    }
                }
            }
        }
    }
}