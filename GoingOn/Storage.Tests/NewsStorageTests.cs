// ****************************************************************************
// <copyright file="NewsMemoryTests.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace Storage.Tests
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Common.Tests;
    using Model.EntitiesBll;
    using Storage.TableStorage;

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
            this.storage = NewsTableStorage.GetInstance();
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
        public void TestGetNewsEmptyStorage()
        {
            AssertExtensions.Throws<StorageException>(() => this.storage.GetNews(city, date, newsGuid).Wait());
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
            Guid guid3 = Guid.NewGuid();

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
            NewsBll oldNews3 = new NewsBll
            {
                Id = guid3,
                Title = "title 3",
                Content = "content",
                Author = "author",
                Date = DateTime.Parse("2014-12-24"),
                City = city
            };

            this.storage.AddNews(oldNews1).Wait();
            this.storage.AddNews(oldNews2).Wait();
            this.storage.AddNews(oldNews3).Wait();

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
            NewsBll updatedDateNews = new NewsBll
            {
                Id = guid3,
                Title = "title 3",
                Content = "content",
                Author = "author",
                Date = DateTime.Parse("2014-12-24"),
                City = city
            };

            this.storage.UpdateNews(updatedTitleNews).Wait();
            this.storage.UpdateNews(updatedContentNews).Wait();
            this.storage.UpdateNews(updatedDateNews).Wait();

            Assert.IsTrue(this.storage.ContainsNews(updatedTitleNews).Result);
            Assert.IsTrue(this.storage.ContainsNews(updatedContentNews).Result);
            Assert.IsTrue(this.storage.ContainsNews(updatedDateNews).Result);
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

            this.storage.DeleteAllNews(city).Wait();

            Assert.IsFalse(this.storage.ContainsNews(News).Result);
        }

        // TODO: add CT for IsAuthorOf
    }
}
