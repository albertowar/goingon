// ****************************************************************************
// <copyright file="NewsFinderTests.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace NewsFinderWorkerRole.Tests
{
    using System.Linq;
    using GoingOn.NewsFinderWorkerRole;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class NewsFinderTests
    {
        [TestMethod]
        public void TestFindNews()
        {
            var news = NewsFinder.FindNews();

            Assert.IsTrue(news.Result.Any());
        }
    }
}
