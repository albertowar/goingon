// ****************************************************************************
// <copyright file="PersistenceException.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace GoingOn.Storage
{
    using System;

    [Serializable]
    public class AzureTableStorageException : Exception
    {
        public AzureTableStorageException(string message)
            : base(message)
        {
        }
    }
}