// ****************************************************************************
// <copyright file="HotNewsStorageTests.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace GoingOn.Storage.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using GoingOn.Common.Tests;
    using GoingOn.Model.EntitiesBll;
    using GoingOn.Storage;
    using GoingOn.Storage.TableStorage;
    using GoingOn.Storage.TableStorage.Entities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class HotNewsStorageTests
    {
        private const string City = "Malaga";

        private static readonly NewsBll DefaultHotNews = new NewsBll
        {
            Id = Guid.NewGuid(),
            Title = "title",
            City = City,
            Content = "content",
            Author = "author",
            Date = DateTime.Parse("2015-05-14")
        };

        private INewsStorage storage;
        private IHotNewsStorage hotNewsStorage;

        [TestInitialize]
        public void Initialize()
        {
            string connectionString = ConfigurationManager.AppSettings["StorageConnectionString"];
            string newsTableName = ConfigurationManager.AppSettings["HotNewsTableName"];

            this.storage = new NewsTableStorage(connectionString, newsTableName);
            this.hotNewsStorage = new NewsTableStorage(connectionString, newsTableName);
        }

        [TestCleanup]
        public void Cleanup()
        {
            this.storage.DeleteAllNews(City).Wait();
        }

        [TestMethod]
        public void TestAddHotNews()
        {
            this.storage.AddNews(NewsEntity.FromNewsBll(DefaultHotNews)).Wait();

            List<NewsBll> hotNews = this.hotNewsStorage.GetNews(DefaultHotNews.City, DefaultHotNews.Date).Result.ToList();
        
            Assert.AreEqual(1, hotNews.Count);
            Assert.IsTrue(new NewsBllEqualityComparer().Equals(DefaultHotNews, hotNews.FirstOrDefault()));
        }

        [TestMethod]
        public void TestGetNonExistingHotNews()
        {
            AssertExtensions.Throws<AzureTableStorageException>(() => this.hotNewsStorage.GetNews(City, DefaultHotNews.Date).Wait());
        }
    }
}
