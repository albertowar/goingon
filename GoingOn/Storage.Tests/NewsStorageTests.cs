// ****************************************************************************
// <copyright file="NewsMemoryTests.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

using NewsTableStorage = GoingOn.Storage.TableStorage.NewsTableStorage;

namespace GoingOn.Storage.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using GoingOn.Common.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Model.EntitiesBll;

    [TestClass]
    public class NewsStorageTests
    {
        private static readonly string city = "Malaga";
        private static readonly DateTime date = DateTime.Parse("2015-05-14");
        private static readonly Guid newsGuid = Guid.NewGuid();

        private static readonly NewsBll News = new NewsBll
        {
            Id = newsGuid,
            Title = "title",
            City = city,
            Content = "content",
            Author = "author",
            Date = date
        };

        private INewsStorage storage;

        [TestInitialize]
        public void Initialize()
        {
            string connectionString = ConfigurationManager.AppSettings["StorageConnectionString"];
            string newsTableName = ConfigurationManager.AppSettings["NewsTableName"];

            this.storage = new NewsTableStorage(connectionString, newsTableName);
        }

        [TestCleanup]
        public void Cleanup()
        {
            this.storage.DeleteAllNews(city).Wait();
        }

        [TestMethod]
        public void TestAddNews()
        {
            this.storage.AddNews(News).Wait();

            Assert.IsTrue(this.storage.ContainsNews(News).Result);
        }

        [TestMethod]
        public void TestGetNews()
        {
            this.storage.AddNews(News).Wait();

            NewsBll actualNews = this.storage.GetNews(city, date, newsGuid).Result;

            Assert.IsTrue(new NewsBllEqualityComparer().Equals(News, actualNews));
        }

        [TestMethod]
        public void TestGetNewsCityDate()
        {
            for (int i = 0; i < 10; ++i)
            {
                var news = new NewsBll
                {
                    Id = Guid.NewGuid(),
                    Title = "title" + i,
                    City = city,
                    Content = "content" + i,
                    Author = "author",
                    Date = date
                };

                this.storage.AddNews(news).Wait();
            }

            IEnumerable<NewsBll> newsList = this.storage.GetNews(city, date).Result;

            Assert.AreEqual(10, newsList.Count());
        }

        [TestMethod]
        public void TestGetNewsEmptyStorage()
        {
            AssertExtensions.Throws<AzureTableStorageException>(() => this.storage.GetNews(city, date, newsGuid).Wait());
        }

        [TestMethod]
        public void TestContainsNews()
        {
            this.storage.AddNews(News).Wait();

            Assert.IsTrue(this.storage.ContainsNews(News).Result);
        }

        [TestMethod]
        public void TestUpdateNews()
        {
            Guid guid1 = Guid.NewGuid();
            Guid guid2 = Guid.NewGuid();

            NewsBll oldNews1 = new NewsBll
            {
                Id = guid1,
                Title = "title 1",
                Content = "content",
                Author = "author",
                Date = DateTime.Parse("2014-12-24"),
                City = city
            };

            NewsBll oldNews2 = new NewsBll
            {
                Id = guid2,
                Title = "title 2",
                Content = "content",
                Author = "author",
                Date = DateTime.Parse("2014-12-24"),
                City = city
            };

            this.storage.AddNews(oldNews1).Wait();
            this.storage.AddNews(oldNews2).Wait();

            NewsBll updatedTitleNews = new NewsBll
            {
                Id = guid1,
                Title = "title 1",
                Content = "content",
                Author = "author",
                Date = DateTime.Parse("2014-12-24"),
                City = city
            };

            NewsBll updatedContentNews = new NewsBll
            {
                Id = guid2,
                Title = "title 2",
                Content = "updated content",
                Author = "author",
                Date = DateTime.Parse("2014-12-24"),
                City = city
            };

            this.storage.UpdateNews(updatedTitleNews).Wait();
            this.storage.UpdateNews(updatedContentNews).Wait();

            Assert.IsTrue(this.storage.ContainsNews(updatedTitleNews).Result);
            Assert.IsTrue(this.storage.ContainsNews(updatedContentNews).Result);
        }

        [TestMethod]
        public void TestContainsNewsReturnsFalse()
        {
            NewsBll newsDifferentTitle = new NewsBll
            {
                Id = newsGuid,
                Title = "different title",
                Content = "content",
                Author = "author",
                Date = new DateTime(2014, 12, 24, 13, 0, 0)
            };
            NewsBll newsDifferentYear = new NewsBll
            {
                Id = newsGuid,
                Title = "title",
                Content = "content",
                Author = "author",
                Date = DateTime.Parse("2014-12-24"),
                City = city
            };
            NewsBll newsDifferentMonth = new NewsBll
            {
                Id = newsGuid,
                Title = "title",
                Content = "content",
                Author = "author",
                Date = DateTime.Parse("2014-12-24"),
                City = city
            };
            NewsBll newsDifferentHour = new NewsBll
            {
                Id = newsGuid,
                Title = "title",
                Content = "content",
                Author = "author",
                Date = DateTime.Parse("2014-12-24"),
                City = city
            };

            this.storage.AddNews(News).Wait();

            Assert.IsFalse(this.storage.ContainsNews(newsDifferentTitle).Result);

            Assert.IsFalse(this.storage.ContainsNews(newsDifferentYear).Result);
            Assert.IsFalse(this.storage.ContainsNews(newsDifferentMonth).Result);
            Assert.IsFalse(this.storage.ContainsNews(newsDifferentHour).Result);
        }

        [TestMethod]
        public void TestDeleteNews()
        {
            this.storage.AddNews(News).Wait();

            this.storage.DeleteNews(city, date, newsGuid).Wait();

            Assert.IsFalse(this.storage.ContainsNews(News).Result);
        }

        [TestMethod]
        public void TestDeleteNewsDoesNotAffectOtherNews()
        {
            var guid1 = Guid.NewGuid();
            var guid2 = Guid.NewGuid();

            var news1 = new NewsBll
            {
                Id = guid1,
                Title = "title",
                Content = "content",
                Author = "author",
                Date = date,
                City = city
            };

            var news2 = new NewsBll
            {
                Id = guid2,
                Title = "other title",
                Content = "content",
                Author = "author",
                Date = date,
                City = city
            };

            this.storage.AddNews(news1).Wait();
            this.storage.AddNews(news2).Wait();

            this.storage.DeleteNews(city, date, guid1).Wait();

            Assert.IsFalse(this.storage.ContainsNews(news1).Result);
            Assert.IsTrue(this.storage.ContainsNews(news2).Result);
        }

        [TestMethod]
        public void TestDeleteAll()
        {
            this.storage.AddNews(News).Wait();

            this.storage.DeleteAllNews(News.City).Wait();

            Assert.IsFalse(this.storage.ContainsNews(News).Result);
        }

        // TODO: add CT for IsAuthorOf

        [TestMethod]
        public void TestExists()
        {
            Assert.IsTrue(this.storage.Exists("Malaga", DateTime.Parse("2015-04-18"), Guid.Parse("fedf9223-1121-46d9-b346-26f91477411f")).Result);
        }
    }
}
