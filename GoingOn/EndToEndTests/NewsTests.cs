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

    using Common.Tests;
    using Client.Entities;
    using GoingOn;
    using MemoryStorage;

    [TestClass]
    public class NewsTests
    {
        private static IDisposable webService;
        private static IUserStorage userStorage;
        private static INewsStorage newsStorage;

        private HttpServer server;

        private static readonly UserClient userClient = new UserClient { Nickname = "Alberto", Password = "1234" };
        private static readonly NewsClient newsClient = new NewsClient { Title = "title", Content = "content" };

        [TestInitialize]
        public void TestInitialize()
        {
            webService = WebApp.Start<Startup>("http://*:80/");
            userStorage = UserMemoryStorage.GetInstance();
            newsStorage = NewsMemoryStorage.GetInstance();

            UserTests.CreateUser(userClient);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            webService.Dispose();
            newsStorage.DeleteAllNews();
            userStorage.DeleteAllUser();
        }

        [TestMethod]
        public void TestCreateNews()
        {
            var response = NewsTests.CreateNews(userClient, newsClient);

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var newsUri = response.Headers.Location;
            var newsId = newsUri.AbsolutePath.Split('/').Last();

            Assert.IsTrue(newsStorage.ContainsNews(Guid.Parse(newsId)).Result);
        }

        [TestMethod]
        public void TestGetNews()
        {
            var createResponse = NewsTests.CreateNews(userClient, newsClient);

            var newsUri = createResponse.Headers.Location;
            var guid = newsUri.AbsolutePath.Split('/').Last();

            var getResponse = NewsTests.GetNews(guid);

            var content = getResponse.Content;
            var jsonContent = content.ReadAsStringAsync().Result;
            var actualNewsClient = JsonConvert.DeserializeObject<NewsClient>(jsonContent);

            Assert.IsTrue(new NewsClientEqualityComparer().Equals(newsClient, actualNewsClient));
        }

        [TestMethod]
        public void TestUpdateNews()
        {
            var updatedNews = new NewsClient { Title = newsClient.Title + "something else", Content = newsClient.Content + "something else" };

            var createResponse = NewsTests.CreateNews(userClient, newsClient);

            var newsUri = createResponse.Headers.Location;
            var guid = newsUri.AbsolutePath.Split('/').Last();

            NewsTests.UpdateNews(userClient, guid, updatedNews);

            var getResponse = NewsTests.GetNews(guid);

            var content = getResponse.Content;
            var jsonContent = content.ReadAsStringAsync().Result;
            var actualNewsClient = JsonConvert.DeserializeObject<NewsClient>(jsonContent);

            Assert.IsTrue(new NewsClientEqualityComparer().Equals(updatedNews, actualNewsClient));
        }

        [TestMethod]
        public void TestUpdateNewsAnotherUser()
        {
            var anotherUser = new UserClient { Nickname = "NotAlberto", Password = "1234" };

            var updatedNews = new NewsClient { Title = newsClient.Title + "something else", Content = newsClient.Content + "something else" };

            var createResponse = NewsTests.CreateNews(userClient, newsClient);

            var newsUri = createResponse.Headers.Location;
            var guid = newsUri.AbsolutePath.Split('/').Last();

            NewsTests.UpdateNews(anotherUser, guid, updatedNews);

            var getResponse = NewsTests.GetNews(guid);

            var content = getResponse.Content;
            var jsonContent = content.ReadAsStringAsync().Result;
            var actualNewsClient = JsonConvert.DeserializeObject<NewsClient>(jsonContent);

            Assert.IsFalse(new NewsClientEqualityComparer().Equals(updatedNews, actualNewsClient));
        }

        [TestMethod]
        public void TestDeleteNews()
        {
            var createResponse = NewsTests.CreateNews(userClient, newsClient);

            var newsUri = createResponse.Headers.Location;
            var guid = newsUri.AbsolutePath.Split('/').Last();

            NewsTests.DeleteNews(userClient, guid);

            Assert.IsFalse(newsStorage.ContainsNews(Guid.Parse(guid)).Result);
        }

        [TestMethod]
        public void TestDeleteNewsFromAnotherUser()
        {
            var anotherUser = new UserClient { Nickname = "NotAlberto", Password = "1234" };
            UserTests.CreateUser(anotherUser);

            var createResponse = NewsTests.CreateNews(userClient, newsClient);

            var newsUri = createResponse.Headers.Location;
            var guid = newsUri.AbsolutePath.Split('/').Last();

            NewsTests.DeleteNews(anotherUser, guid);

            Assert.IsTrue(newsStorage.ContainsNews(Guid.Parse(guid)).Result);
        }

        #region Helper methods

        public static HttpResponseMessage CreateNews(UserClient user, NewsClient news)
        {
            var authorizationString = AuthorizationHelper.Base64Encode(string.Format("{0}:{1}", user.Nickname, user.Password));

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:80/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authorizationString);

                return client.PostAsJsonAsync("api/news", news).Result;
            }
        }

        public static HttpResponseMessage UpdateNews(UserClient user, string id, NewsClient news)
        {
            var authorizationString = AuthorizationHelper.Base64Encode(string.Format("{0}:{1}", user.Nickname, user.Password));

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:80/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authorizationString);

                return client.PatchAsJsonAsync("api/news/" + id, news).Result;
            }
        }

        private static HttpResponseMessage GetNews(string guid)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:80/");

                return client.GetAsync(string.Format("api/news/{0}", guid)).Result;
            }
        }

        private static HttpResponseMessage DeleteNews(UserClient user, string guid)
        {
            var authorizationString = AuthorizationHelper.Base64Encode(string.Format("{0}:{1}", user.Nickname, user.Password));

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:80/");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authorizationString);

                return client.DeleteAsync(string.Format("api/news/{0}", guid)).Result;
            }
        }

        #endregion
    }
}