// ****************************************************************************
// <copyright file="BlobStore.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// News Storage base interface
// </summary>
// ****************************************************************************

namespace GoingOn.XStoreProxy.BlobStore
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;

    public class BlobStore : IBlobStore
    {
        // Configuration info
        private readonly string blobContainerName;
        private readonly CloudStorageAccount storageAccount;

        public BlobStore(string connectionString, string blobContainerName)
        {
            this.blobContainerName = blobContainerName;

            try
            {
                this.storageAccount = CloudStorageAccount.Parse(connectionString);
            }
            catch (Exception e)
            {
                throw new AzureTableStorageException(string.Format("The repository account could not be created. Error: {0}", e.Message));
            }
        }

        public async Task CreateBlob(string blobName, Stream stream)
        {
            CloudBlobContainer container = this.GetCloudBlobContainer();

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobName);

            await blockBlob.UploadFromStreamAsync(stream);
        }

        public async Task<bool> ContainsBlob(string blobName)
        {
            CloudBlobContainer container = this.GetCloudBlobContainer();

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobName);

            return await blockBlob.ExistsAsync();
        }

        public async Task DeleteBlob(string blobName)
        {
            CloudBlobContainer container = this.GetCloudBlobContainer();

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobName);

            await blockBlob.DeleteAsync();
        }

        #region Helper methods

        private CloudBlobContainer GetCloudBlobContainer()
        {
            CloudBlobClient tableClient = this.storageAccount.CreateCloudBlobClient();

            return tableClient.GetContainerReference(this.blobContainerName);
        }

        #endregion
    }
}
