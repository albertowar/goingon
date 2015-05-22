// ****************************************************************************
// <copyright file="AzureTableStorageException.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// Class to encapsulate error situations with Storage
// </summary>
// ****************************************************************************

namespace GoingOn.XStoreProxy
{
    using System;

    [Serializable]
    public class AzureTableStorageException : Exception
    {
        public AzureTableStorageException(string message)
            : base(message)
        {
        }

        public AzureTableStorageException()
        {
        }
    }
}