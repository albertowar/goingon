// ****************************************************************************
// <copyright file="GoingOnFrontendException.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// Class to encapsulate the exception originated in the Frontend side. Used also for validation exceptions.
// </summary>
// ****************************************************************************

namespace GoingOn.Frontend.Common
{
    using System;
    using System.Net;

    public class GoingOnFrontendException : Exception
    {
        public HttpStatusCode StatusCode { get; private set; }

        public GoingOnFrontendException(HttpStatusCode statusCode, string message) : base(message)
        {
            this.StatusCode = statusCode;
        }
    }
}
