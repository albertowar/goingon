// ****************************************************************************
// <copyright file="NewsTests.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace GoingOn.EndToEndTests
{
    using System;
    using System.Linq;
    using System.Net;

    using GoingOn.Frontend;
    using GoingOn.Frontend.Entities;
    using GoingOn.Client;
    using GoingOn.Client.Entities;
    using GoingOn.Storage;
    using GoingOn.Storage.TableStorage;

    using Microsoft.Owin.Hosting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Newtonsoft.Json;

    [TestClass]
    public class NewsTests
    {
        private IDisposable webService;

        private IUserStorage userStorage;

        private INewsStorage newsStorage;

        private GOClient goClient;

        private static readonly UserClient userClient = new UserClient { Nickname = "Alberto", Password = "1234", City = "Malaga"};
        private static readonly NewsClient newsClient = new NewsClient { Title = "title", Content = "content" };

        private static readonly string City = "Malaga";
        private static readonly string Date = "2015-05-11";

        [TestInitialize]
        public void TestInitialize()
        {
            this.webService = WebApp.Start<Startup>("http://*:80/");
            this.goClient = new GOClient(@"http://localhost:80/", "Alberto", "1234");

            this.userStorage = UserTableStorage.GetInstance();
            this.newsStorage = NewsTableStorage.GetInstance();

            this.goClient.CreateUser(userClient).Wait();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            this.webService.Dispose();

            this.newsStorage.DeleteAllNews(City).Wait();
            this.userStorage.DeleteAllUsers().Wait();
        }

        [TestMethod]
        public void TestCreateNews()
        {
            var response = this.goClient.CreateNews(City, Date, newsClient).Result;

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var newsUri = response.Headers.Location;
            var newsId = newsUri.AbsolutePath.Split('/').Last();

            Assert.IsTrue(this.newsStorage.Exists(City, DateTime.Parse(Date), Guid.Parse(newsId)).Result);
        }

        [TestMethod]
        public void TestGetNews()
        {
            var createResponse = this.goClient.CreateNews(City, Date, newsClient).Result;

            var newsUri = createResponse.Headers.Location;
            var guid = newsUri.AbsolutePath.Split('/').Last();

            var getResponse = this.goClient.GetNews(City, Date, guid).Result;

            var content = getResponse.Content;
            var jsonContent = content.ReadAsStringAsync().Result;
            var actualNewsClient = JsonConvert.DeserializeObject<NewsREST>(jsonContent);

            Assert.IsTrue(new NewsClientEqualityComparer().Equals(newsClient, NewsClient.FromNewsREST(actualNewsClient)));
        }

        [TestMethod]
        public void TestUpdateNews()
        {
            var updatedNews = new NewsClient { Title = newsClient.Title + "something else", Content = newsClient.Content + "something else" };

            var createResponse = this.goClient.CreateNews(City, Date, newsClient).Result;

            var newsUri = createResponse.Headers.Location;
            var guid = newsUri.AbsolutePath.Split('/').Last();

            this.goClient.UpdateNews(City, Date, guid, updatedNews).Wait();

            var getResponse = this.goClient.GetNews(City, Date, guid).Result;

            var content = getResponse.Content;
            var jsonContent = content.ReadAsStringAsync().Result;
            var actualNewsClient = JsonConvert.DeserializeObject<NewsREST>(jsonContent);

            Assert.IsTrue(new NewsClientEqualityComparer().Equals(updatedNews, NewsClient.FromNewsREST(actualNewsClient)));
        }

        [TestMethod]
        public void TestUpdateNewsAnotherUser()
        {
            var updatedNews = new NewsClient { Title = newsClient.Title + "something else", Content = newsClient.Content + "something else" };

            var createResponse = this.goClient.CreateNews(City, Date, newsClient).Result;

            var newsUri = createResponse.Headers.Location;
            var guid = newsUri.AbsolutePath.Split('/').Last();

            this.goClient.UpdateNews(City, Date, guid, updatedNews).Wait();

            var getResponse = this.goClient.GetNews(City, Date, guid).Result;

            var content = getResponse.Content;
            var jsonContent = content.ReadAsStringAsync().Result;
            var actualNewsClient = JsonConvert.DeserializeObject<NewsClient>(jsonContent);

            Assert.IsFalse(new NewsClientEqualityComparer().Equals(updatedNews, actualNewsClient));
        }

        [TestMethod]
        public void TestDeleteNews()
        {
            var createResponse = this.goClient.CreateNews(City, Date, newsClient).Result;

            var newsUri = createResponse.Headers.Location;
            var guid = newsUri.AbsolutePath.Split('/').Last();

            this.goClient.DeleteNews(City, Date, guid).Wait();

            Assert.IsFalse(this.newsStorage.Exists(City, DateTime.Parse(Date), Guid.Parse(guid)).Result);
        }

        [TestMethod]
        public void TestDeleteNewsFromAnotherUser()
        {
            var anotherGoClient = new GOClient(@"http://localhost:80/", "NotAlberto", "1234");
            var anotherUser = new UserClient { Nickname = "NotAlberto", Password = "1234" };
            anotherGoClient.CreateUser(anotherUser).Wait();

            var createResponse = this.goClient.CreateNews(City, Date, newsClient).Result;

            var newsUri = createResponse.Headers.Location;
            var guid = newsUri.AbsolutePath.Split('/').Last();

            anotherGoClient.DeleteNews(City, Date, guid).Wait();

            Assert.IsTrue(this.newsStorage.Exists(City, DateTime.Parse(Date), Guid.Parse(guid)).Result);
        }
    }
}