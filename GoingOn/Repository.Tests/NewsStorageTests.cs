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
    using System.Configuration;
    using GoingOn.Model.EntitiesBll;
    using GoingOn.Repository;
    using GoingOn.Repository.Entities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    // TODO: mock Store behaviour

    [TestClass]
    public class NewsStorageTests
    {
        private const string City = "Malaga";

        private static readonly NewsBll DefaultNews = new NewsBll
        {
            Id = Guid.NewGuid(),
            Title = "title",
            City = City,
            Content = "content",
            Author = "author",
            Date = DateTime.Parse("2015-05-14")
        };

        private INewsRepository storage;

        [TestInitialize]
        public void Initialize()
        {
            string connectionString = ConfigurationManager.AppSettings["StorageConnectionString"];
            string newsTableName = ConfigurationManager.AppSettings["NewsTableName"];

            this.storage = new NewsTableRepository(connectionString, newsTableName);
        }

        [TestCleanup]
        public void Cleanup()
        {
            this.storage.DeleteAllNews(City).Wait();
        }

        [TestMethod]
        public void TestContainsNewsCheckContent()
        {
            this.storage.AddNews(NewsEntity.FromNewsBll(DefaultNews)).Wait();

            Assert.IsTrue(this.storage.ContainsNewsCheckContent(NewsEntity.FromNewsBll(DefaultNews)).Result);
        }

        
        [TestMethod]
        public void TestContainsNewsReturnsFalse()
        {
            NewsBll newsDifferentTitle = new NewsBll
            {
                Id = DefaultNews.Id,
                Title = "different title",
                Content = DefaultNews.Content,
                Author = DefaultNews.Author,
                Date = DefaultNews.Date
            };
            NewsBll newsDifferentYear = new NewsBll
            {
                Id = DefaultNews.Id,
                Title = DefaultNews.Title,
                Content = DefaultNews.Content,
                Author = DefaultNews.Author,
                Date = DefaultNews.Date.AddYears(1),
                City = DefaultNews.City
            };
            NewsBll newsDifferentMonth = new NewsBll
            {
                Id = DefaultNews.Id,
                Title = DefaultNews.Title,
                Content = DefaultNews.Content,
                Author = DefaultNews.Author,
                Date = DefaultNews.Date.AddMonths(1),
                City = DefaultNews.City
            };
            NewsBll newsDifferentDay = new NewsBll
            {
                Id = DefaultNews.Id,
                Title = DefaultNews.Title,
                Content = DefaultNews.Content,
                Author = DefaultNews.Author,
                Date = DefaultNews.Date.AddDays(1),
                City = DefaultNews.City
            };

            this.storage.AddNews(NewsEntity.FromNewsBll(DefaultNews)).Wait();

            Assert.IsFalse(this.storage.ContainsNewsCheckContent(NewsEntity.FromNewsBll(newsDifferentTitle)).Result);
            Assert.IsFalse(this.storage.ContainsNewsCheckContent(NewsEntity.FromNewsBll(newsDifferentYear)).Result);
            Assert.IsFalse(this.storage.ContainsNewsCheckContent(NewsEntity.FromNewsBll(newsDifferentMonth)).Result);
            Assert.IsFalse(this.storage.ContainsNewsCheckContent(NewsEntity.FromNewsBll(newsDifferentDay)).Result);
        }

        
        [TestMethod]
        public void TestDeleteAll()
        {
            this.storage.AddNews(NewsEntity.FromNewsBll(DefaultNews)).Wait();

            this.storage.DeleteAllNews(DefaultNews.City).Wait();

            Assert.IsFalse(this.storage.ContainsNewsCheckContent(NewsEntity.FromNewsBll(DefaultNews)).Result);
        }

        [TestMethod]
        public void TestIsAuthorOf()
        {
            this.storage.AddNews(NewsEntity.FromNewsBll(DefaultNews)).Wait();

            Assert.IsTrue(this.storage.IsAuthorOf(DefaultNews.City, DefaultNews.Date, DefaultNews.Id, DefaultNews.Author).Result);
        }

        [TestMethod]
        public void TestIsAuthorOf_WrongAuthor()
        {
            this.storage.AddNews(NewsEntity.FromNewsBll(DefaultNews)).Wait();

            Assert.IsFalse(this.storage.IsAuthorOf(DefaultNews.City, DefaultNews.Date, DefaultNews.Id, string.Format("not-the-author-{0}", DefaultNews.Author)).Result);
        }

        [TestMethod]
        public void TestContainsNews()
        {
            this.storage.AddNews(NewsEntity.FromNewsBll(DefaultNews)).Wait();

            Assert.IsTrue(this.storage.ContainsNews(DefaultNews.City, DefaultNews.Date, DefaultNews.Id).Result);
        }
    }
}
