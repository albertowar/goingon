// ****************************************************************************
// <copyright file="HotNewsCreator.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace GoingOn.HotNewsCreatorTests
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using GoingOn.HotNewsCreatorWorkerRole;
    using GoingOn.Model.EntitiesBll;
    using GoingOn.Storage;
    using GoingOn.Storage.TableStorage;
    using GoingOn.Storage.TableStorage.Entities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class HotNewsCreatorTests
    {
        private INewsStorage hotNewsStorage;

        private HotNewsCreator hotNewsCreator;

        [TestInitialize]
        public void Initialize()
        {
            string storageConnectionString = ConfigurationManager.AppSettings["StorageConnectionString"];

            string hotNewsTableName = ConfigurationManager.AppSettings["HotNewsTableName"];

            this.hotNewsStorage = new NewsTableStorage(storageConnectionString, hotNewsTableName);

            this.hotNewsCreator = new HotNewsCreator();
        }

        [TestCleanup]
        public void Cleanup()
        {
            this.hotNewsStorage.DeleteAllNews("Malaga");
        }

        [TestMethod]
        public void TestPushNews()
        {
            NewsBll[] hotNews = this.CreateHotNews().ToArray();

            this.hotNewsCreator.PushHotNews(hotNews);

            for (var i = 0; i < 10; ++i)
            {
                Assert.IsTrue(this.hotNewsStorage.ContainsNewsCheckContent(NewsEntity.FromNewsBll(hotNews[i])).Result);
            }
        }

        #region Helper methods

        private IEnumerable<NewsBll> CreateHotNews()
        {
            var news = new List<NewsBll>();

            for (var i = 0; i < 10; ++i)
            {
                news.Add(new NewsBll
                {
                    Id = Guid.NewGuid(),
                    Title = "title",
                    Content = "content",
                    City = "Malaga",
                    Date = DateTime.Today,
                    Author = "author"
                });
            }

            return news;
        }

        #endregion
    }
}
