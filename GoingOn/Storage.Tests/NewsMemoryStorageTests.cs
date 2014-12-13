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
    using System.Linq;
    
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Common.Tests;
    using MemoryStorage;
    using Model.EntitiesBll;

    [TestClass]
    public class NewsMemoryStorageTests
    {
        private static readonly UserBll Author = new UserBll(nickname: "nickname", password: "password");
        private static readonly NewsBll News = new NewsBll(title: "title", content: "content", author: Author, date: new TimeSpan(), rating: 1);

        private INewsStorage storage;

        [TestInitialize]
        public void Initialize()
        {
            storage = NewsMemoryStorage.GetInstance();
        }

        [TestCleanup]
        public void Cleanup()
        {
            storage.DeleteAllNews();
        }

        [TestMethod]
        public void TestGetNews()
        {
            storage.AddNews(News);

            NewsBll actualNews = storage.GetNews().First();

            Assert.IsTrue(new NewsBllEqualityComparer().Equals(News, actualNews));
        }

        [TestMethod]
        public void TestGetNewsEmptyStorage()
        {
            Assert.IsFalse(storage.GetNews().Any());
        }

        [TestMethod]
        public void TestAddNews()
        {
            storage.AddNews(News);

            Assert.IsTrue(storage.ContainsNews(News));
        }

        [TestMethod]
        public void TestDeleteNews()
        {
            storage.AddNews(News);

            storage.DeleteNews(News);

            Assert.IsFalse(storage.ContainsNews(News));
        }

        [TestMethod]
        public void TestDeleteAll()
        {
            this.AddNews();

            storage.DeleteAllNews();

            Assert.IsFalse(storage.GetNews().Any());
        }

        [TestMethod]
        public void TestContainsNews()
        {
            storage.AddNews(News);

            Assert.IsTrue(storage.ContainsNews(News));
        }

        #region Helper methods

        private void AddNews()
        {
            for (int i = 0; i < 10; ++i)
            {
                var news = new NewsBll(title: "title" + i, content: "content" + i, author: Author, date: new TimeSpan(), rating: i);
            }
        }

        #endregion
    }
}
