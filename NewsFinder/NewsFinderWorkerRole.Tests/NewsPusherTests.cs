// ****************************************************************************
// <copyright file="NewsPusherTests.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace NewsFinderWorkerRole.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using GoingOn.Client;
    using GoingOn.Client.Entities;
    using GoingOn.NewsFinderWorkerRole;
    using GoingOn.NewsFinderWorkerRole.Entities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class NewsPusherTests
    {
        private NewsPusher newsPusher;

        private GOClient client;

        [TestInitialize]
        public void Initialize()
        {
            string username = ConfigurationManager.AppSettings["NewsFinderWorkerRoleUsername"];
            string password = ConfigurationManager.AppSettings["NewsFinderWorkerRolePassword"];

            this.client = new GOClient(
                ConfigurationManager.AppSettings["GoingOnEndpoint"],
                username,
                password);

            this.client.CreateUser(new UserClient { Nickname = username, Password = password, City = "Malaga", Name = "Alberto", Email = "alberto@gmail.com" }).Wait();

            this.newsPusher = new NewsPusher();
        }

        /*[TestMethod]
        public void TestPushSingleNews()
        {
            var articles = new List<GuardianSingleItem>
            {
                new GuardianSingleItem()
                {
                    Title = "Title",
                    Summary = "Summary",
                    PublishDate = DateTime.Parse("2015-12-05")
                }
            };

            this.newsPusher.PushNews(articles).Wait();
        }*/

        [TestMethod]
        public void TestPusAllhNews()
        {
            var articles = NewsFinder.FindNews().Result;

            this.newsPusher.PushNews(articles).Wait();
        }
    }
}
