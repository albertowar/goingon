// ****************************************************************************
// <copyright file="NewsMemoryTests.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <"author">Alberto Guerra Gonzalez</"author">
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace Storage.Tests
{
    using System;
    
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using Common.Tests;
    using MemoryStorage.Entities;
    using Model.EntitiesBll;

    [TestClass]
    public class NewsMemoryTests
    {
        private static readonly Guid newsGuid = Guid.NewGuid();

        [TestMethod]
        public void TestEqualsSucceeds()
        {
            NewsMemory news1 = new NewsMemory(newsGuid, "title", "content", "author", new DateTime(), 0);
            NewsMemory news2 = new NewsMemory(newsGuid, "title", "content", "author", new DateTime(), 0);
            NewsMemory news3 = new NewsMemory(newsGuid, "title", "otherContent", "author", new DateTime(), 0);
            NewsMemory news4 = new NewsMemory(newsGuid, "title", "content", "author", new DateTime(1), 0);
            NewsMemory news5 = new NewsMemory(newsGuid, "title", "content", "author", new DateTime(), 2);

            Assert.AreEqual(news1, news2, "Equal");
            Assert.AreEqual(news1, news3, "Equal - Different Content");
            Assert.AreEqual(news1, news4, "Equal - Different TimeSpan");
            Assert.AreEqual(news1, news5, "Equal - Different rating");
        }

        [TestMethod]
        public void TestEqualsFails()
        {
            NewsMemory news1 = new NewsMemory(Guid.NewGuid(), "title", It.IsAny<string>(), "author", It.IsAny<DateTime>(), It.IsAny<int>());
            NewsMemory news2 = new NewsMemory(Guid.NewGuid(), "another title", It.IsAny<string>(), "author", It.IsAny<DateTime>(), It.IsAny<int>());

            Assert.AreNotEqual(news1, news2, "Different GUID");
        }

        [TestMethod]
        public void TestFromNewsBll()
        {
            NewsMemory newsMemory = new NewsMemory(newsGuid, "title", It.IsAny<string>(), "author", It.IsAny<DateTime>(), It.IsAny<int>());
            
            NewsBll newsBll = new NewsBll
            {
                Id = newsGuid,
                Title = "title",
                Content =  It.IsAny<string>(),
                Author = "author",
                Date = It.IsAny<DateTime>(),
                Rating = It.IsAny<int>()
            };

            Assert.AreEqual(newsMemory, NewsMemory.FromNewsBll(newsBll));
        }

        [TestMethod]
        public void TestToNewsBll()
        {
            NewsMemory newsMemory = new NewsMemory(newsGuid, "title", It.IsAny<string>(), "author", It.IsAny<DateTime>(), It.IsAny<int>());
            NewsBll newsBll = new NewsBll
            {
                Id = newsGuid,
                Title = "title",
                Content = It.IsAny<string>(),
                Author= "author",
                Date = It.IsAny<DateTime>(),
                Rating = It.IsAny<int>()
            };

            Assert.IsTrue(new NewsBllEqualityComparer().Equals(newsBll, NewsMemory.ToNewsBll(newsMemory)));
        }

        [TestMethod]
        public void TestMerge()
        {
            NewsMemory newsMemory = new NewsMemory(newsGuid, "title", "content", "author", new DateTime(2014, 12, 25, 13, 0, 0), It.IsAny<int>());
            NewsMemory anotherNewsMemory = new NewsMemory(newsGuid, "another title", "another content", "author", new DateTime(2014, 12, 26, 13, 0, 0), It.IsAny<int>());

            newsMemory.Merge(anotherNewsMemory);

            Assert.AreEqual(newsGuid, newsMemory.Id);
            Assert.AreEqual(newsMemory.Id, anotherNewsMemory.Id);
            Assert.AreEqual(newsMemory.Title, anotherNewsMemory.Title);
            Assert.AreEqual(newsMemory.Content, anotherNewsMemory.Content);
            Assert.AreEqual(newsMemory.Date, anotherNewsMemory.Date);
        }

        [TestMethod]
        public void TestMergeNullValues()
        {
            NewsMemory newsMemory = new NewsMemory(newsGuid, "title", "content", "author", new DateTime(2014, 12, 25, 13, 0, 0), It.IsAny<int>());
            NewsMemory anotherNewsMemory = new NewsMemory(newsGuid, null, null, "author", new DateTime(2014, 12, 26, 13, 0, 0), It.IsAny<int>());

            newsMemory.Merge(anotherNewsMemory);

            Assert.AreEqual(newsGuid, newsMemory.Id);
            Assert.AreEqual(newsMemory.Id, anotherNewsMemory.Id);
            Assert.AreEqual("title", newsMemory.Title);
            Assert.AreEqual("content", newsMemory.Content);
            Assert.AreEqual(newsMemory.Date, anotherNewsMemory.Date);
        }

        [TestMethod]
        public void TestMergeDifferentNews()
        {
            NewsMemory newsMemory = new NewsMemory(Guid.NewGuid(), "title", "content", "author", new DateTime(2014, 12, 25, 13, 0, 0), It.IsAny<int>());
            NewsMemory anotherNewsMemory = new NewsMemory(Guid.NewGuid(), "another title", "another content", "author", new DateTime(2014, 12, 26, 13, 0, 0), It.IsAny<int>());

            newsMemory.Merge(anotherNewsMemory);

            Assert.AreNotEqual(newsMemory.Id, anotherNewsMemory.Id);
            Assert.AreNotEqual(newsMemory.Title, anotherNewsMemory.Title);
            Assert.AreNotEqual(newsMemory.Content, anotherNewsMemory.Content);
            Assert.AreNotEqual(newsMemory.Date, anotherNewsMemory.Date);
        }
    }
}
