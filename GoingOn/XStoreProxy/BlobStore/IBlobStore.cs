// ****************************************************************************
// <copyright file="IBlobStore.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// News Storage base interface
// </summary>
// ****************************************************************************

namespace GoingOn.XStoreProxy.BlobStore
{
    using System.IO;
    using System.Threading.Tasks;

    public interface IBlobStore
    {
        Task CreateBlob(string blobName, Stream content);

        Task<bool> ContainsBlob(string blobName);

        Task GetBlob(string blobName, Stream target);

        Task DeleteBlob(string blobName);
    }
}
