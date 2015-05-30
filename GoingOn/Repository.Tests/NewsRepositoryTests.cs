// ****************************************************************************
// <copyright file="NewsMemoryTests.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// Tests for News Storage
// </summary>
// ****************************************************************************

namespace GoingOn.Repository.Tests
{
    using System;
    using System.Threading.Tasks;
    using GoingOn.Model.EntitiesBll;
    using GoingOn.Repository;
    using GoingOn.XStoreProxy;
    using GoingOn.XStoreProxy.Entities;
    using GoingOn.XStoreProxy.TableStore;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class NewsRepositoryTests
    {
        private static readonly NewsBll DefaultNews = new NewsBll { City = string.Empty, Date = new DateTime(), Title = string.Empty, Author = string.Empty };

        private INewsRepository repository;

        private Mock<ITableStore> mockTableStore;

        [TestInitialize]
        public void Initialize()
        {
            this.mockTableStore = new Mock<ITableStore>();

            this.repository = new NewsTableRepository(this.mockTableStore.Object);
        }

        [TestMethod]
        public void TestContainsNews()
        {
            this.mockTableStore.Setup(
                newsStore => newsStore.GetTableEntity<NewsEntity>(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(NewsEntity.FromNewsBll(DefaultNews)));

            Assert.IsTrue(this.repository.ContainsNews(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>()).Result);
        }

        
        [TestMethod]
        public void TestContainsNewsReturnsFalse()
        {
            this.mockTableStore.Setup(
                newsStore => newsStore.GetTableEntity<NewsEntity>(It.IsAny<string>(), It.IsAny<string>()))
                .Throws<AzureXStoreException>();

            Assert.IsFalse(this.repository.ContainsNews(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>()).Result);
        }
    }
}
