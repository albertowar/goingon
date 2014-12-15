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

    using Moq;

    using Common.Tests;
    using MemoryStorage.Entities;
    using Model.EntitiesBll;

    [TestClass]
    public class NewsMemoryTests
    {
        private static readonly UserMemory author = new UserMemory("author");

        [TestMethod]
        public void TestEqualsSucceeds()
        {
            NewsMemory news1 = new NewsMemory("title", "content", author, new TimeSpan(), 0);
            NewsMemory news2 = new NewsMemory("title", "content", author, new TimeSpan(), 0);
            NewsMemory news3 = new NewsMemory("title", "otherContent", author, new TimeSpan(), 0);
            NewsMemory news4 = new NewsMemory("title", "content", author, new TimeSpan(1), 0);
            NewsMemory news5 = new NewsMemory("title", "content", author, new TimeSpan(), 2);

            Assert.AreEqual(news1, news2, "Equal");
            Assert.AreEqual(news1, news3, "Equal - Different Content");
            Assert.AreEqual(news1, news4, "Equal - Different TimeSpan");
            Assert.AreEqual(news1, news5, "Equal - Different rating");
        }

        [TestMethod]
        public void TestEqualsFails()
        {
            NewsMemory news1 = new NewsMemory("title", It.IsAny<string>(), author, It.IsAny<TimeSpan>(), It.IsAny<int>());
            NewsMemory news2 = new NewsMemory("another title", It.IsAny<string>(), author, It.IsAny<TimeSpan>(), It.IsAny<int>());
            NewsMemory news3 = new NewsMemory("title", It.IsAny<string>(), new UserMemory("another author"), It.IsAny<TimeSpan>(), It.IsAny<int>());

            Assert.AreNotEqual(news1, news2, "Different title");
            Assert.AreNotEqual(news1, news3, "Different author");
            Assert.AreNotEqual(news1, null, "Null news");
        }

        [TestMethod]
        public void TestHashCode()
        {
            NewsMemory news1 = new NewsMemory("title", "content", author, new TimeSpan(), 0);

            Assert.IsTrue(0 > news1.GetHashCode());
        }

        [TestMethod]
        public void TestFromNewsBll()
        {
            NewsMemory newsMemory = new NewsMemory("title", It.IsAny<string>(), author, It.IsAny<TimeSpan>(), It.IsAny<int>());
            NewsBll newsBll = new NewsBll("title", It.IsAny<string>(), new UserBll("author", "password"), It.IsAny<TimeSpan>(), It.IsAny<int>());

            Assert.AreEqual(newsMemory, NewsMemory.FromNewsBll(newsBll));
        }

        [TestMethod]
        public void TestToNewsBll()
        {
            NewsMemory newsMemory = new NewsMemory("title", It.IsAny<string>(), author, It.IsAny<TimeSpan>(), It.IsAny<int>());
            NewsBll newsBll = new NewsBll("title", It.IsAny<string>(), new UserBll("author", "password"), It.IsAny<TimeSpan>(), It.IsAny<int>());

            Assert.IsTrue(new NewsBllEqualityComparer().Equals(newsBll, NewsMemory.ToNewsBll(newsMemory)));
        }
    }
}
