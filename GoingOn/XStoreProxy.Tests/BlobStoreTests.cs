// ****************************************************************************
// <copyright file="BlobStoreTests.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// News Storage base interface
// </summary>
// ****************************************************************************

namespace GoingOn.XStoreProxy.Tests
{
    using System;
    using System.Configuration;
    using System.IO;
    using GoingOn.Common.Tests;
    using GoingOn.XStoreProxy.BlobStore;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class BlobStoreTests
    {
        private const string BlobName = "blob";

        private IBlobStore blobStore;

        [TestInitialize]
        public void Initialize()
        {
            string connectionString = ConfigurationManager.AppSettings.Get("StorageConnectionString");
            string blobContainerName = ConfigurationManager.AppSettings.Get("ImageBlobContainerName");

            this.blobStore = new BlobStore(connectionString, blobContainerName);
        }

        [TestCleanup]
        public void Cleanup()
        {
            try
            {
                this.blobStore.DeleteBlob(BlobName).Wait();
            }
            catch (Exception)
            {
            }
        }

        [TestMethod]
        public void TestCreateBlob()
        {
            this.blobStore.CreateBlob(BlobName, new MemoryStream()).Wait();

            Assert.IsTrue(this.blobStore.ContainsBlob(BlobName).Result);
        }

        [TestMethod]
        public void TestCreateExistingBlob()
        {
            this.blobStore.CreateBlob(BlobName, new MemoryStream()).Wait();

            this.blobStore.CreateBlob(BlobName, new MemoryStream()).Wait();

            Assert.IsTrue(this.blobStore.ContainsBlob(BlobName).Result);
        }

        [TestMethod]
        public void TestGetBlob()
        {
            var expectedStream = new MemoryStream();
            this.blobStore.CreateBlob(BlobName, expectedStream).Wait();

            var actualStream = new MemoryStream();
            this.blobStore.GetBlob(BlobName, actualStream).Wait();

            Assert.AreEqual(expectedStream.Length, actualStream.Length);
        }

        [TestMethod]
        public void GetBlobThrowsException_IfNonExisting()
        {
            AssertExtensions.Throws<AzureXStoreException>(() => this.blobStore.GetBlob(BlobName, new MemoryStream()).Wait());
        }

        [TestMethod]
        public void TestDeleteBlob()
        {
            var expectedStream = new MemoryStream();
            this.blobStore.CreateBlob(BlobName, expectedStream).Wait();

            this.blobStore.DeleteBlob(BlobName).Wait();

            Assert.IsFalse(this.blobStore.ContainsBlob(BlobName).Result);
        }

        [TestMethod]
        public void DeleteBlobThrowsException_IfNonExisting()
        {
            AssertExtensions.Throws<AzureXStoreException>(() => this.blobStore.DeleteBlob(BlobName).Wait());
        }
    }
}
