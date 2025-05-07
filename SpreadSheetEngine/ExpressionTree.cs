// <copyright file="ExpressionTree.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SpreadSheetEngine
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Represents an expression tree that can evaluate a string of expressions using the Shunting Yard algorithm.
    /// </summary>
    internal class ExpressionTree
    {
        private Node root;  // Root node of the expression tree.
        private Dictionary<string, double> variables;  // Holds variables and their values.

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionTree"/> class with a given expression.
        /// </summary>
        /// <param name="expression">The string expression to evaluate.</param>
        public ExpressionTree(string expression)
        {
            this.variables = new Dictionary<string, double>();

            // Use the ShuntingYard class to convert the expression to postfix and build the tree
            var postfixTokens = ShuntingYard.ConvertToPostfix(this.Tokenize(expression), this.variables);
            this.root = this.BuildTreeFromPostfix(postfixTokens);
        }

        /// <summary>
        /// Sets the value of a variable in the expression.
        /// </summary>
        /// <param name="variableName">The name of the variable.</param>
        /// <param name="variableValue">The value to assign to the variable.</param>
        public void SetVariable(string variableName, double variableValue)
        {
            if (string.IsNullOrWhiteSpace(variableName))
            {
                throw new ArgumentException("Variable name cannot be null or empty.");
            }

            // If the variable exists in the dictionary, update its value; otherwise, throw an exception
            if (this.variables.ContainsKey(variableName))
            {
                this.variables[variableName] = variableValue;
            }
            else
            {
                throw new KeyNotFoundException($"Variable '{variableName}' is not present in the expression.");
            }
        }

        /// <summary>
        /// Evaluates the expression by traversing the expression tree.
        /// </summary>
        /// <returns>The result of the evaluated expression.</returns>
        public double Evaluate()
        {
            if (this.root == null)
            {
                throw new InvalidOperationException("The expression tree is empty. No expression has been set.");
            }

            // Evaluate the root node
            return this.root.Evaluate();  // Evaluation now checks for variables not being set
        }

        /// <summary>
        /// Tokenizes the expression string into a list of meaningful tokens (numbers, variables, and operators).
        /// </summary>
        /// <param name="expression">The expression string to tokenize.</param>
        /// <returns>A list of tokens representing the components of the expression.</returns>
        private List<string> Tokenize(string expression)
        {
            var tokens = new List<string>();

            // Using regex to identify numbers, variables, and operators
            var numberPattern = @"(\d+(\.\d+)?)"; // Matches a number with or without a decimal point
            var variablePattern = @"([A-Za-z][A-Za-z0-9]*)"; // Matches a variable name
            var operatorPattern = @"[+\-*/()]"; // Matches supported operators

            // Regex pattern combining the valid patterns
            var regexPattern = $"{numberPattern}|{variablePattern}|{operatorPattern}";
            var regex = new Regex(regexPattern);

            // Iterate through the expression string and match the regex pattern
            foreach (Match match in regex.Matches(expression))
            {
                string token = match.Value;
                tokens.Add(token);

                // Check if the token is a variable (starts with a letter)
                if (Regex.IsMatch(token, variablePattern))
                {
                    // Add the variable to the dictionary if it doesn't exist, with a null or NaN value
                    if (!this.variables.ContainsKey(token))
                    {
                        this.variables[token] = double.NaN;  // Variable is added, but not yet set
                    }
                }
            }

            // Check for unsupported tokens by finding any characters that weren't matched by the regex
            string remainingExpression = regex.Replace(expression, string.Empty);
            if (!string.IsNullOrWhiteSpace(remainingExpression))
            {
                throw new InvalidOperationException($"Unrecognized operator or token: {remainingExpression}");
            }

            return tokens;
        }

        /// <summary>
        /// Builds the expression tree from a postfix list of tokens.
        /// </summary>
        /// <param name="postfixTokens">The list of tokens in postfix order.</param>
        /// <returns>The root node of the expression tree.</returns>
        private Node BuildTreeFromPostfix(Queue<string> postfixTokens)
        {
            Stack<Node> nodeStack = new Stack<Node>();
            OperatorNodeFactory operatorNodeFactory = new OperatorNodeFactory();

            while (postfixTokens.Count > 0)
            {
                string token = postfixTokens.Dequeue();

                // If it's a number, create a ConstantNode
                if (double.TryParse(token, out double value))
                {
                    nodeStack.Push(new ConstantNode(value));
                }

                // If it's a variable, create a VariableNode
                else if (this.variables.ContainsKey(token))
                {
                    nodeStack.Push(new VariableNode(this.variables) { Name = token });
                }

                // If it's an operator, pop two nodes and create an OperatorNode
                else if (ShuntingYard.IsOperator(token[0]))
                {
                    var right = nodeStack.Pop();
                    var left = nodeStack.Pop();
                    OperatorNode operatorNode = operatorNodeFactory.CreateOperatorNode(token[0]);
                    operatorNode.Left = left;
                    operatorNode.Right = right;
                    nodeStack.Push(operatorNode);
                }
                else
                {
                    throw new InvalidOperationException($"Unrecognized token: {token}");
                }
            }

            return nodeStack.Pop();
        }
    }
}
