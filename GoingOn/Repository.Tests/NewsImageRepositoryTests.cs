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

    // TODO: mock Store behaviour

    [TestClass]
    public class NewsImageRepositoryTests
    {
        private INewsImageRepository repository;

        private Mock<IBlobStore> mockBlobStore;
        private Mock<IImageManager> mockImageManager;

        [TestInitialize]
        public void Initialize()
        {
            this.mockBlobStore = new Mock<IBlobStore>();
            this.mockImageManager = new Mock<IImageManager>();

            this.repository = new NewsNewsImageRepository(this.mockBlobStore.Object, this.mockImageManager.Object);
        }

        [TestMethod]
        public void TestNewsImageRepositoryThrowsIfNotFoundImageAfterCreate()
        {
            this.mockImageManager.Setup(imageManager => imageManager.SaveToSteam(It.IsAny<Image>(), It.IsAny<Stream>()));
            this.mockBlobStore.Setup(blobStore => blobStore.ContainsBlob(It.IsAny<string>())).Returns(Task.FromResult(false));
            this.mockBlobStore.Setup(blobStore => blobStore.CreateBlob(It.IsAny<string>(), It.IsAny<Stream>())).Returns(Task.FromResult(0));

            AssertExtensions.Throws<RepositoryException>(
                () =>
                    this.repository.CreateNewsImage(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>(), It.IsAny<Image>()).Wait());
        }
    }
}
