// <copyright file="SelfReferenceException.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SpreadSheetEngine
{
    /// <summary>
    /// Exception that indicates a self reference within the spreadsheet.
    /// </summary>
    public class SelfReferenceException : InvalidOperationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelfReferenceException"/> class
        /// </summary>
        /// <param name="message"> The specific message to be notified to the user.</param>
        public SelfReferenceException(string message)
            : base(message)
        {
        }
    }
}
