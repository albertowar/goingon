// ****************************************************************************
// <copyright file="NewsEntity.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// Tests for News entity
// </summary>
// ****************************************************************************

namespace GoingOn.Repository.Tests.Entitites
{
    using System;
    using GoingOn.Model.EntitiesBll;
    using GoingOn.Repository.Entities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class NewsEntityTests
    {
        [TestMethod]
        public void TestFromNewsBll()
        {
            var newsBll = new NewsBll
            {
                Id = Guid.NewGuid(),
                Date = DateTime.Parse("2015-05-04"),
                City = "Malaga",
                Title = "title",
                Content = "content",
                Author = "author"
            };

            NewsEntity convertedNews = NewsEntity.FromNewsBll(newsBll);
            
            Assert.AreEqual("Malaga;2015-05-04", convertedNews.PartitionKey);
            Assert.AreEqual(newsBll.Id.ToString(), convertedNews.RowKey);
            Assert.AreEqual(newsBll.Title, convertedNews.Title);
            Assert.AreEqual(newsBll.Content, convertedNews.Content);
            Assert.AreEqual(newsBll.Author, convertedNews.Author);
        }

        [TestMethod]
        public void TestToNewsBll()
        {
            var newsEntity = new NewsEntity
            {
                PartitionKey = "Malaga;2015-05-04",
                RowKey = Guid.NewGuid().ToString(),
                Title = "title",
                Content = "content",
                Author = "author"
            };

            NewsBll newsBll = NewsEntity.ToNewsBll(newsEntity);

            Assert.AreEqual(newsEntity.Title, newsBll.Title);
            Assert.AreEqual(newsEntity.Content, newsBll.Content);
            Assert.AreEqual(newsEntity.Author, newsBll.Author);
        }

        [TestMethod]
        public void TestEquals()
        {
            var referenceNews = new NewsEntity
            {
                PartitionKey = "partitionKey",
                RowKey = "rowKey",
                Title = "title",
                Content = "content",
                Author = "author"
            };

            var differentTitleNews = new NewsEntity
            {
                PartitionKey = "partitionKey",
                RowKey = "rowKey",
                Title = "title",
                Content = "content",
                Author = "author"
            };

            var differentContentNews = new NewsEntity
            {
                PartitionKey = "partitionKey",
                RowKey = "rowKey",
                Title = "title",
                Content = "content",
                Author = "author"
            };

            var differentAuthorNews = new NewsEntity
            {
                PartitionKey = "partitionKey",
                RowKey = "rowKey",
                Title = "title",
                Content = "content",
                Author = "another-author"
            };

            var differentPrimaryKeyNews = new NewsEntity
            {
                PartitionKey = "differentPartitionKey",
                RowKey = "rowKey",
                Title = "title",
                Content = "content",
                Author = "author"
            };

            var differentRowKeyNews = new NewsEntity
            {
                PartitionKey = "partitionKey",
                RowKey = "differentRowKey",
                Title = "title",
                Content = "content",
                Author = "author"
            };

            Assert.AreEqual(referenceNews, differentTitleNews);
            Assert.AreEqual(referenceNews, differentContentNews);
            Assert.AreEqual(referenceNews, differentAuthorNews);
            Assert.AreNotEqual(referenceNews, null);
            Assert.AreNotEqual(referenceNews, differentPrimaryKeyNews);
            Assert.AreNotEqual(referenceNews, differentRowKeyNews);
        }

        [TestMethod]
        public void TestMergeNews()
        {
            var newsToMerge = new NewsEntity
            {
                PartitionKey = "partitionKey",
                RowKey = "rowKey",
                Title = "title",
                Content = "content",
                Author = "author"
            };

            var newsDifferentTitle = new NewsEntity
            {
                PartitionKey = "partitionKey",
                RowKey = "rowKey",
                Title = "differentTitle",
                Content = "content",
                Author = "author"
            };

            var newsDifferentContent = new NewsEntity
            {
                PartitionKey = "partitionKey",
                RowKey = "rowKey",
                Title = "title",
                Content = "differentContent",
                Author = "author"
            };

            newsDifferentTitle.Merge(newsToMerge);
            newsDifferentContent.Merge(newsToMerge);

            Assert.AreEqual(newsDifferentTitle.Title, newsToMerge.Title);
            Assert.AreEqual(newsDifferentTitle.Content, newsToMerge.Content);
        }

        [TestMethod]
        public void TestBuildPartitionKey()
        {
            const string city = "city";
            const string date = "2015-05-21";

            Assert.AreEqual(string.Format("{0};{1}", city, date), NewsEntity.BuildPartitionkey(city, DateTime.Parse(date)));
        }

        [TestMethod]
        public void TestExtractFromPartitionKey()
        {
            const string city = "city";
            const string date = "2015-05-21";
            string partitionKey = string.Format("{0};{1}", city, date);

            Tuple<string, DateTime> values = NewsEntity.ExtractFromPartitionKey(partitionKey);
            Assert.AreEqual(city, values.Item1);
            Assert.AreEqual(DateTime.Parse(date), values.Item2);
        }
    }
}
