// ****************************************************************************
// <copyright file="NewsImageBlobRepository.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// Class to manage the repository of images
// </summary>
// ****************************************************************************

namespace GoingOn.XStoreProxy.BlobStore
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Threading.Tasks;
    using GoingOn.XStoreProxy;
    using Microsoft.WindowsAzure.Storage.Blob;

    public class NewsImageBlobRepository : IImageRepository
    {
        private readonly IBlobStore blobStore;

        public NewsImageBlobRepository(string connectionString, string blobContainerName)
        {
            this.blobStore = new BlobStore(connectionString, blobContainerName);
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
            string blobName = string.Format("{0};{1};{2}", city, date.ToString("yy-MM-dd"), id);

            using (var memoryStream = new MemoryStream())
            {
                image.Save(memoryStream, image.RawFormat);
                memoryStream.Position = 0;

                await this.blobStore.CreateBlob(blobName, memoryStream);
            }

            if (await this.blobStore.ContainsBlob(blobName))
            {
                using (var memoryStream = new MemoryStream())
                {
                    Image thumbnail = image.GetThumbnailImage(40, 40, () => false, IntPtr.Zero);
                    thumbnail.Save(memoryStream, image.RawFormat);
                    memoryStream.Position = 0;

                    await this.blobStore.CreateBlob(string.Format("thumbnail;{0};", blobName), memoryStream);
                }
            }
            else
            {
                // TODO: throw exception
            }
        }

        public async Task DeleteNewsImage(string city, DateTime date, Guid id)
        {
            string blobName = string.Format("{0};{1};{2}", city, date.ToString("yy-MM-dd"), id);

            await this.blobStore.DeleteBlob(blobName);
        }

        public async Task<bool> ContainsImage(string city, DateTime date, Guid id)
        {
            string blobName = string.Format("{0};{1};{2}", city, date.ToString("yy-MM-dd"), id);

            return await this.blobStore.ContainsBlob(blobName);
        }
    }
}
