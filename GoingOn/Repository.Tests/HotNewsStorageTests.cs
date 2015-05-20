// ****************************************************************************
// <copyright file="HotNewsStorageTests.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// Tests for Hot News Storage
// </summary>
// ****************************************************************************

namespace GoingOn.Repository.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using GoingOn.Common.Tests;
    using GoingOn.Model.EntitiesBll;
    using GoingOn.Repository.Entities;
    using GoingOn.XStoreProxy;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

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

        private INewsRepository repository;
        private IHotNewsRepository _hotNewsRepository;

        [TestInitialize]
        public void Initialize()
        {
            string connectionString = ConfigurationManager.AppSettings["StorageConnectionString"];
            string newsTableName = ConfigurationManager.AppSettings["HotNewsTableName"];

            this.repository = new NewsTableRepository(connectionString, newsTableName);
            this._hotNewsRepository = new NewsTableRepository(connectionString, newsTableName);
        }

        [TestCleanup]
        public void Cleanup()
        {
            this.repository.DeleteAllNews(City).Wait();
        }

        [TestMethod]
        public void TestAddHotNews()
        {
            this.repository.AddNews(NewsEntity.FromNewsBll(DefaultHotNews)).Wait();

            List<NewsBll> hotNews = this._hotNewsRepository.ListNews(DefaultHotNews.City, DefaultHotNews.Date).Result.ToList();
        
            Assert.AreEqual(1, hotNews.Count);
            Assert.IsTrue(new NewsBllEqualityComparer().Equals(DefaultHotNews, hotNews.FirstOrDefault()));
        }

        [TestMethod]
        public void TestGetNonExistingHotNews()
        {
            AssertExtensions.Throws<AzureTableStorageException>(() => this._hotNewsRepository.ListNews(City, DefaultHotNews.Date).Wait());
        }
    }
}
