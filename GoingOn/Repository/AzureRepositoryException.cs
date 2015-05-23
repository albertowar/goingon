// ****************************************************************************
// <copyright file="AzureXStoreException.cs" company="Universidad de Malaga">
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
    public class AzureRepositoryException : Exception
    {
        public AzureRepositoryException(string message)
            : base(message)
        {
        }

        public AzureRepositoryException()
        {
        }
    }
}