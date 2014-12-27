// ****************************************************************************
// <copyright file="NewsTests.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace EndToEndTests
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Web.Http;

    using Microsoft.Owin.Hosting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Newtonsoft.Json;

    using Client.Entities;
    using GoingOn;
    using MemoryStorage;

    [TestClass]
    public class NewsTests
    {
        private static IDisposable webService;
        //private static IUserStorage userStorage;
        private static INewsStorage newsStorage;

        private HttpServer server;

        private static readonly UserClient userClient = new UserClient { Nickname = "Alberto", Password = "1234" };
        private static readonly NewsClient newsClient = new NewsClient { Title = "title", Content = "content" };

        [TestInitialize]
        public void TestInitialize()
        {
            webService = WebApp.Start<Startup>("http://*:80/");
            //userStorage = UserMemoryStorage.GetInstance();
            newsStorage = NewsMemoryStorage.GetInstance();

            UserTests.CreateUser(userClient);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            webService.Dispose();
            newsStorage.DeleteAllNews();
        }

        [TestMethod]
        public void TestCreateNews()
        {
            var response = NewsTests.CreateNews(newsClient);

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var newsUri = response.Headers.Location;
            var newsId = newsUri.AbsolutePath.Split('/').Last();
            Assert.IsTrue(newsStorage.ContainsNews(Guid.Parse(newsId)).Result);
        }

        [TestMethod]
        public void TestGetNews()
        {
            var createResponse = NewsTests.CreateNews(newsClient);

            var newsUri = createResponse.Headers.Location;
            var guid = newsUri.AbsolutePath.Split('/').Last();

            var getResponse = NewsTests.GetUser(guid);

            var content = getResponse.Content;
            var jsonContent = content.ReadAsStringAsync().Result;
            NewsClient actualNewsClient = JsonConvert.DeserializeObject<NewsClient>(jsonContent);

            Assert.AreEqual(HttpStatusCode.OK, getResponse.StatusCode);
            Assert.IsTrue(new NewsClientEqualityComparer().Equals(newsClient, actualNewsClient));
        }

        #region Helper methods

        public static HttpResponseMessage CreateNews(NewsClient news)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:80/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", "QWxiZXJ0bzoxMjM0");

                return client.PostAsJsonAsync("api/news", news).Result;
            }
        }

        private static HttpResponseMessage GetUser(string guid)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:80/");

                return client.GetAsync(string.Format("api/news/{0}", guid)).Result;
            }
        }

        #endregion
    }
}