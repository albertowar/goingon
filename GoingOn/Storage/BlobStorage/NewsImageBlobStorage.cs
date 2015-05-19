// ****************************************************************************
// <copyright file="NewsImageBlobStorage.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// Class to manage the storage of images
// </summary>
// ****************************************************************************

namespace GoingOn.Storage.BlobStorage
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;

    public class NewsImageBlobStorage : IImageStorage
    {
        // Configuration info
        private readonly string blobContainerName;
        private readonly CloudStorageAccount storageAccount;

        public NewsImageBlobStorage(string connectionString, string blobContainerName)
        {
            this.blobContainerName = blobContainerName;

            try
            {
                this.storageAccount = CloudStorageAccount.Parse(connectionString);
            }
            catch (Exception e)
            {
                throw new AzureTableStorageException(string.Format("The storage account could not be created. Error: {0}", e.Message));
            }
        }

        public async Task<Image> GetNewsImage(string city, DateTime date, Guid id)
        {
            CloudBlobContainer container = this.GetCloudBlobContainer();

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(string.Format("{0};{1};{2}", city, date.ToString("yy-MM-dd"), id));

            using (var memoryStream = new MemoryStream())
            {
                await blockBlob.DownloadToStreamAsync(memoryStream);

                return Image.FromStream(memoryStream);
            }
        }

        public async Task<Image> GetNewsThumbnailImage(string city, DateTime date, Guid id)
        {
            CloudBlobContainer container = this.GetCloudBlobContainer();

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(string.Format("thumbnail;{0};{1};{2}", city, date.ToString("yy-MM-dd"), id));

            using (var memoryStream = new MemoryStream())
            {
                await blockBlob.DownloadToStreamAsync(memoryStream);

                return Image.FromStream(memoryStream);
            }
        }

        public async Task CreateNewsImage(string city, DateTime date, Guid id, Image image)
        {
            CloudBlobContainer container = this.GetCloudBlobContainer();

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(string.Format("{0};{1};{2}", city, date.ToString("yy-MM-dd"), id));

            using (var memoryStream = new MemoryStream())
            {
                image.Save(memoryStream, image.RawFormat);
                memoryStream.Position = 0;
                await blockBlob.UploadFromStreamAsync(memoryStream);
            }

            if (await blockBlob.ExistsAsync())
            {
                blockBlob = container.GetBlockBlobReference(string.Format("thumbnail;{0};{1};{2}", city, date.ToString("yy-MM-dd"), id));

                using (var memoryStream = new MemoryStream())
                {
                    Image thumbnail = image.GetThumbnailImage(40, 40, () => false, IntPtr.Zero);
                    thumbnail.Save(memoryStream, image.RawFormat);
                    memoryStream.Position = 0;
                    await blockBlob.UploadFromStreamAsync(memoryStream);
                }
            }
            else
            {
                // TODO: throw exception
            }
        }

        public async Task DeleteNewsImage(string city, DateTime date, Guid id)
        {
            CloudBlobContainer container = this.GetCloudBlobContainer();

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(string.Format("{0};{1};{2}", city, date.ToString("yy-MM-dd"), id));

            await blockBlob.DeleteAsync();
        }

        public async Task<bool> ContainsImage(string city, DateTime date, Guid id)
        {
            CloudBlobContainer container = this.GetCloudBlobContainer();

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(string.Format("{0};{1};{2}", city, date.ToString("yy-MM-dd"), id));

            return await blockBlob.ExistsAsync();
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
