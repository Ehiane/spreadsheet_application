// <copyright file="ShuntingYard.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SpreadSheetEngine
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a static class that implements the Shunting Yard algorithm to handle operator precedence and associativity.
    /// </summary>
    internal class ShuntingYard
    {
        // Dictionary to store the precedence of each operator.
        private static readonly Dictionary<string, int> Precedence = new Dictionary<string, int>
        {
            { "+", 1 },
            { "-", 1 },
            { "*", 2 },
            { "/", 2 },
        };

        /// <summary>
        /// Checks if an operator is left associative.
        /// </summary>
        /// <param name="op">The operator character to check.</param>
        /// <returns>True if the operator is left associative, otherwise false.</returns>
        public static bool IsLeftAssociative(char op)
        {
            return op == '+' || op == '-' || op == '*' || op == '/';
        }

        /// <summary>
        /// Retrieves the precedence level of an operator.
        /// </summary>
        /// <param name="op">The operator character.</param>
        /// <returns>The precedence level as an integer.</returns>
        public static int GetPrecedence(char op)
        {
            string opString = op.ToString();
            if (Precedence.ContainsKey(opString))
            {
                return Precedence[opString];
            }

            throw new InvalidOperationException("Unsupported operator: " + op);
        }

        /// <summary>
        /// Converts an infix expression (such as "3 + 4 * 2") into postfix notation (such as "3 4 2 * +").
        /// </summary>
        /// <param name="tokens">The list of tokens representing the infix expression.</param>
        /// <param name="variables">A dictionary of variables and their values.</param>
        /// <returns>A queue of tokens representing the expression in postfix notation.</returns>
        public static Queue<string> ConvertToPostfix(List<string> tokens, Dictionary<string, double> variables)
        {
            var output = new Queue<string>();
            var operators = new Stack<char>();

            foreach (var token in tokens)
            {
                if (double.TryParse(token, out _)) // Number
                {
                    output.Enqueue(token);
                }
                else if (variables.ContainsKey(token)) // Variable
                {
                    output.Enqueue(token);
                }
                else if (IsOperator(token[0])) // Operator
                {
                    while (operators.Count > 0 &&
                           IsOperator(operators.Peek()) &&
                           ((IsLeftAssociative(token[0]) && GetPrecedence(token[0]) <= GetPrecedence(operators.Peek())) ||
                            (!IsLeftAssociative(token[0]) && GetPrecedence(token[0]) < GetPrecedence(operators.Peek()))))
                    {
                        output.Enqueue(operators.Pop().ToString());
                    }

                    operators.Push(token[0]);
                }
                else if (token == "(")
                {
                    operators.Push(token[0]);
                }
                else if (token == ")")
                {
                    while (operators.Peek() != '(')
                    {
                        output.Enqueue(operators.Pop().ToString());
                    }

                    operators.Pop(); // Remove the '('
                }
                else
                {
                    throw new ArgumentException($"Invalid token detected: {token}");
                }
            }

            while (operators.Count > 0)
            {
                output.Enqueue(operators.Pop().ToString());
            }

            return output;
        }

        /// <summary>
        /// Checks if a character is a valid operator.
        /// </summary>
        /// <param name="op">The operator character.</param>
        /// <returns>True if the character is an operator, otherwise false.</returns>
        public static bool IsOperator(char op)
        {
            return "+-*/".Contains(op);
        }
    }
}
