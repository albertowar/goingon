// ****************************************************************************
// <copyright file="NewsMemoryTests.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// Tests for News Storage
// </summary>
// ****************************************************************************

namespace GoingOn.Storage.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using GoingOn.Common.Tests;
    using GoingOn.Storage.TableStorage;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using GoingOn.Model.EntitiesBll;
    using GoingOn.Storage.TableStorage.Entities;

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
            this.storage.DeleteAllNews(City).Wait();
        }

        [TestMethod]
        public void TestAddNews()
        {
            this.storage.AddNews(NewsEntity.FromNewsBll(DefaultNews)).Wait();

            Assert.IsTrue(this.storage.ContainsNewsCheckContent(NewsEntity.FromNewsBll(DefaultNews)).Result);
        }

        [TestMethod]
        public void TestGetNews()
        {
            this.storage.AddNews(NewsEntity.FromNewsBll(DefaultNews)).Wait();

            NewsBll actualNews = this.storage.GetNews(DefaultNews.City, DefaultNews.Date, DefaultNews.Id).Result;

            Assert.IsTrue(new NewsBllEqualityComparer().Equals(DefaultNews, actualNews));
        }

        [TestMethod]
        public void TestGetNews_UsingCityDate()
        {
            for (int i = 0; i < 10; ++i)
            {
                var news = new NewsEntity
                {
                    PartitionKey = NewsEntity.BuildPartitionkey(DefaultNews.City, DefaultNews.Date),
                    RowKey = Guid.NewGuid().ToString(),
                    Title = "title" + i,
                    Content = "content" + i,
                    Author = "author"
                };

                this.storage.AddNews(news).Wait();
            }

            IEnumerable<NewsBll> newsList = this.storage.GetNews(DefaultNews.City, DefaultNews.Date).Result;

            Assert.AreEqual(10, newsList.Count());
        }

        [TestMethod]
        public void TestGetNewsEmptyStorage()
        {
            AssertExtensions.Throws<AzureTableStorageException>(() => this.storage.GetNews(DefaultNews.City, DefaultNews.Date, DefaultNews.Id).Wait());
        }

        [TestMethod]
        public void TestContainsNews()
        {
            this.storage.AddNews(NewsEntity.FromNewsBll(DefaultNews)).Wait();

            Assert.IsTrue(this.storage.ContainsNewsCheckContent(NewsEntity.FromNewsBll(DefaultNews)).Result);
        }

        [TestMethod]
        public void TestUpdateNews()
        {
            var oldNews1 = new NewsBll
            {
                Id = Guid.NewGuid(),
                Title = "title 1",
                Content = "content",
                Author = "author",
                Date = DateTime.Parse("2014-12-24"),
                City = DefaultNews.City
            };

            var oldNews2 = new NewsBll
            {
                Id = Guid.NewGuid(),
                Title = "title 2",
                Content = "content",
                Author = "author",
                Date = DefaultNews.Date,
                City = DefaultNews.City
            };

            this.storage.AddNews(NewsEntity.FromNewsBll(oldNews1)).Wait();
            this.storage.AddNews(NewsEntity.FromNewsBll(oldNews2)).Wait();

            var updatedTitleNews = new NewsBll
            {
                Id = oldNews1.Id,
                Title = "title 1",
                Content = oldNews1.Content,
                Author = oldNews1.Author,
                Date = oldNews1.Date,
                City = oldNews1.City
            };

            NewsBll updatedContentNews = new NewsBll
            {
                Id = oldNews2.Id,
                Title = oldNews2.Title,
                Content = "updated content",
                Author = oldNews2.Author,
                Date = oldNews2.Date,
                City = oldNews2.City
            };

            this.storage.UpdateNews(updatedTitleNews).Wait();
            this.storage.UpdateNews(updatedContentNews).Wait();

            Assert.IsTrue(this.storage.ContainsNewsCheckContent(NewsEntity.FromNewsBll(updatedTitleNews)).Result);
            Assert.IsTrue(this.storage.ContainsNewsCheckContent(NewsEntity.FromNewsBll(updatedContentNews)).Result);
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
        public void TestDeleteNews()
        {
            this.storage.AddNews(NewsEntity.FromNewsBll(DefaultNews)).Wait();

            this.storage.DeleteNews(DefaultNews.City, DefaultNews.Date, DefaultNews.Id).Wait();

            Assert.IsFalse(this.storage.ContainsNewsCheckContent(NewsEntity.FromNewsBll(DefaultNews)).Result);
        }

        [TestMethod]
        public void TestDeleteNewsDoesNotAffectOtherNews()
        {
            var news1 = new NewsBll
            {
                Id = Guid.NewGuid(),
                Title = "title",
                Content = "content",
                Author = "author",
                Date = DefaultNews.Date,
                City = DefaultNews.City
            };

            var news2 = new NewsBll
            {
                Id = Guid.NewGuid(),
                Title = "other title",
                Content = "content",
                Author = "author",
                Date = DefaultNews.Date,
                City = DefaultNews.City
            };

            this.storage.AddNews(NewsEntity.FromNewsBll(news1)).Wait();
            this.storage.AddNews(NewsEntity.FromNewsBll(news2)).Wait();

            this.storage.DeleteNews(news1.City, news1.Date, news1.Id).Wait();

            Assert.IsFalse(this.storage.ContainsNewsCheckContent(NewsEntity.FromNewsBll(news1)).Result);
            Assert.IsTrue(this.storage.ContainsNewsCheckContent(NewsEntity.FromNewsBll(news2)).Result);
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
        public void TestExists()
        {
            this.storage.AddNews(NewsEntity.FromNewsBll(DefaultNews)).Wait();

            Assert.IsTrue(this.storage.ContainsNews(DefaultNews.City, DefaultNews.Date, DefaultNews.Id).Result);
        }
    }
}
