// <copyright file="CyclicDependencyException.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SpreadSheetEngine
{
    using System;

    /// <summary>
    /// Exception that indicates a cyclic dependency within the spreadsheet.
    /// </summary>
    internal class CyclicDependencyException:InvalidOperationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CyclicDependencyException"/> class
        /// with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public CyclicDependencyException(string message)
            : base(message)
        {
        }
    }
}
