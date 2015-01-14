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
        private static readonly Guid newsGuid = Guid.NewGuid();

        private static readonly NewsBll News = new NewsBll
        {
            Id = newsGuid,
            Title = "title",
            Content = "content",
            Author = "author",
            Date = new DateTime(2014, 12, 24, 13, 0, 0),
            Rating = 1
        };

        private INewsStorage storage;

        [TestInitialize]
        public void Initialize()
        {
            storage = NewsTableStorage.GetInstance();
        }

        [TestCleanup]
        public void Cleanup()
        {
            storage.DeleteAllNews().Wait();
        }

        [TestMethod]
        public void TestAddNews()
        {
            storage.AddNews(News).Wait();

            Assert.IsTrue(storage.ContainsNews(News).Result);
        }

        [TestMethod]
        public void TestGetNews()
        {
            storage.AddNews(News).Wait();

            NewsBll actualNews = storage.GetNews(newsGuid).Result;

            Assert.IsTrue(new NewsBllEqualityComparer().Equals(News, actualNews));
        }

        [TestMethod]
        [Ignore]
        public void TestGetNewsEmptyStorage()
        {
            Assert.IsNull(storage.GetNews(newsGuid).Result);
        }

        [TestMethod]
        public void TestContainsNews()
        {
            storage.AddNews(News).Wait();

            Assert.IsTrue(storage.ContainsNews(News).Result);
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
                Date = new DateTime(2014, 12, 24, 13, 0, 0),
                Rating = 1
            };
            NewsBll oldNews2 = new NewsBll
            {
                Id = guid2,
                Title = "title 2",
                Content = "content",
                Author = "author",
                Date = new DateTime(2014, 12, 24, 13, 0, 0),
                Rating = 1
            };
            NewsBll oldNews3 = new NewsBll
            {
                Id = guid3,
                Title = "title 3",
                Content = "content",
                Author = "author",
                Date = new DateTime(2014, 12, 24, 13, 0, 0),
                Rating = 1
            };

            storage.AddNews(oldNews1).Wait();
            storage.AddNews(oldNews2).Wait();
            storage.AddNews(oldNews3).Wait();

            NewsBll updatedTitleNews = new NewsBll
            {
                Id = guid1,
                Title = "title 1",
                Content = "content",
                Author = "author",
                Date = new DateTime(2014, 12, 24, 13, 0, 0),
                Rating = 1
            };
            NewsBll updatedContentNews = new NewsBll
            {
                Id = guid2,
                Title = "title 2",
                Content = "updated content",
                Author = "author",
                Date = new DateTime(2014, 12, 24, 13, 0, 0),
                Rating = 1
            };
            NewsBll updatedDateNews = new NewsBll
            {
                Id = guid3,
                Title = "title 3",
                Content = "content",
                Author = "author",
                Date = new DateTime(2014, 12, 25, 13, 0, 0),
                Rating = 1
            };
            ;

            storage.UpdateNews(updatedTitleNews).Wait();
            storage.UpdateNews(updatedContentNews).Wait();
            storage.UpdateNews(updatedDateNews).Wait();

            Assert.IsTrue(storage.ContainsNews(updatedTitleNews).Result);
            Assert.IsTrue(storage.ContainsNews(updatedContentNews).Result);
            Assert.IsTrue(storage.ContainsNews(updatedDateNews).Result);
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
                Date = new DateTime(2014, 12, 24, 13, 0, 0),
                Rating = 1
            };
            NewsBll newsDifferentYear = new NewsBll
            {
                Id = newsGuid,
                Title = "title",
                Content = "content",
                Author = "author",
                Date = new DateTime(2015, 12, 24, 13, 0, 0),
                Rating = 1
            };
            NewsBll newsDifferentMonth = new NewsBll
            {
                Id = newsGuid,
                Title = "title",
                Content = "content",
                Author = "author",
                Date = new DateTime(2014, 11, 24, 13, 0, 0),
                Rating = 1
            };
            NewsBll newsDifferentHour = new NewsBll
            {
                Id = newsGuid,
                Title = "title",
                Content = "content",
                Author = "author",
                Date = new DateTime(2014, 12, 24, 14, 0, 0),
                Rating = 1
            };

            storage.AddNews(News).Wait();

            Assert.IsFalse(storage.ContainsNews(newsDifferentTitle).Result);
            
            // TODO: fix de filter
            //Assert.IsFalse(storage.ContainsNews(newsDifferentYear).Result);
            //Assert.IsFalse(storage.ContainsNews(newsDifferentMonth).Result);
            //Assert.IsFalse(storage.ContainsNews(newsDifferentHour).Result);
        }

        [TestMethod]
        public void TestDeleteNews()
        {
            storage.AddNews(News).Wait();

            storage.DeleteNews(newsGuid).Wait();

            Assert.IsFalse(storage.ContainsNews(News).Result);
        }

        [TestMethod]
        public void TestDeleteNewsDoesNotAffectOtherNews()
        {
            Guid guid1 = Guid.NewGuid();
            Guid guid2 = Guid.NewGuid();
            NewsBll news1 = new NewsBll
            {
                Id = guid1,
                Title = "title",
                Content = "content",
                Author = "author",
                Date = new DateTime(2014, 12, 24, 13, 0, 0),
                Rating = 1
            };
            NewsBll news2 = new NewsBll
            {
                Id = guid2,
                Title = "other title",
                Content = "content",
                Author = "author",
                Date = new DateTime(2014, 12, 24, 13, 0, 0),
                Rating = 1
            };

            storage.AddNews(news1).Wait();
            storage.AddNews(news2).Wait();

            storage.DeleteNews(guid1).Wait();

            Assert.IsFalse(storage.ContainsNews(news1).Result);
            Assert.IsTrue(storage.ContainsNews(news2).Result);
        }

        [TestMethod]
        public void TestDeleteAll()
        {
            storage.AddNews(News).Wait();

            storage.DeleteAllNews().Wait();

            Assert.IsFalse(storage.ContainsNews(News).Result);
        }

        #region Helper methods

        #endregion
    }
}
