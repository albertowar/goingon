// ****************************************************************************
// <copyright file="RepositoryException.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// Class to encapsulate error situations with Storage
// </summary>
// ****************************************************************************

namespace GoingOn.Repository
{
    using System;

    [Serializable]
    public class RepositoryException : Exception
    {
        public RepositoryException(string message)
            : base(message)
        {
        }

        public RepositoryException()
        {
        }
    }
}