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
    using System.Drawing;
    using System.IO;
    using System.Threading.Tasks;
    using GoingOn.Common.Tests;
    using GoingOn.Model;
    using GoingOn.Repository;
    using GoingOn.XStoreProxy.BlobStore;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class ImageRepositoryTests
    {
        private IImageRepository repository;

        private Mock<IBlobStore> mockBlobStore;

        [TestInitialize]
        public void Initialize()
        {
            this.mockBlobStore = new Mock<IBlobStore>();

            this.repository = new ImageRepository(this.mockBlobStore.Object);
        }

        [TestMethod]
        public void TestNewsImageRepositoryThrowsIfNotFoundImageAfterCreate()
        {
            this.mockBlobStore.Setup(blobStore => blobStore.ContainsBlob(It.IsAny<string>())).Returns(Task.FromResult(false));
            this.mockBlobStore.Setup(blobStore => blobStore.CreateBlob(It.IsAny<string>(), It.IsAny<Stream>())).Returns(Task.FromResult(0));

            Image image = Image.FromFile("goten.png");

            AssertExtensions.Throws<RepositoryException>(
                () =>
                    this.repository.CreateNewsImage(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>(), image).Wait());
        }
    }
}
