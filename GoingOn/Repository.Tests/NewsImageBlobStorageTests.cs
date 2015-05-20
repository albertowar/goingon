// ****************************************************************************
// <copyright file="NewsImageBlobStorageTests.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// NewsImageBlobRepository tests class
// </summary>
// ****************************************************************************

namespace GoingOn.Repository.Tests
{
    using System;
    using System.Configuration;
    using System.Drawing;
    using GoingOn.XStoreProxy;
    using GoingOn.XStoreProxy.BlobStore;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class NewsImageBlobStorageTests
    {
        private const string City = "Malaga";
        private static readonly DateTime date = DateTime.Today;
        private static readonly Guid NewsId = Guid.NewGuid();
        private static readonly Image Image = Image.FromFile("goku.png");

        private IImageRepository repository;

        [TestInitialize]
        public void Initialize()
        {
            string connectionString = ConfigurationManager.AppSettings["StorageConnectionString"];
            string blobContainerName = ConfigurationManager.AppSettings["ImageBlobContainerName"];

            this.repository = new NewsImageBlobRepository(connectionString, blobContainerName);
        }

        [TestMethod]
        public void TestCreateImage()
        {
            this.repository.CreateNewsImage(City, date, NewsId, Image).Wait();

            Assert.IsTrue(this.repository.ContainsImage(City, date, NewsId).Result);

            this.repository.DeleteNewsImage(City, date, NewsId).Wait();
        }

        [TestMethod]
        public void TestCreateExistingImage()
        {
            this.repository.CreateNewsImage(City, date, NewsId, Image).Wait();

            this.repository.CreateNewsImage(City, date, NewsId, Image).Wait();
        }

        // TODO: implement test for all operations
    }
}
