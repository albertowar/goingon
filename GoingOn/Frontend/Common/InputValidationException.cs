// ****************************************************************************
// <copyright file="InputValidationException.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// Exception class to encapsulate input validation issues
// </summary>
// ****************************************************************************

namespace GoingOn.Frontend.Common
{
    using System.Net;

    public class InputValidationException : GoingOnFrontendException
    {
        public InputValidationException(HttpStatusCode statusCode, string message)
            : base(statusCode, message)
        {
        }
    }
}