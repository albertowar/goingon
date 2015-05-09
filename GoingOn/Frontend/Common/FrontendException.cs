// ****************************************************************************
// <copyright file="FrontendException.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace GoingOn.Frontend.Common
{
    using System;
    using System.Net;

    public class FrontendException : Exception
    {
        public HttpStatusCode StatusCode { get; private set; }

        public FrontendException(HttpStatusCode statusCode, string message) : base(message)
        {
            this.StatusCode = statusCode;
        }
    }
}
