// ****************************************************************************
// <copyright file="BusinessValidationException.cs" company="Universidad de Malaga">
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

    public class BusinessValidationException : Exception
    {
        public BusinessValidationException(string message) : base(message)
        {
        }
    }
}