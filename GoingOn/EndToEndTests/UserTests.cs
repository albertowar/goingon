// ****************************************************************************
// <copyright file="UserTests.cs" company="Universidad de Malaga">
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
    using System.Net;
    using System.Net.Http.Headers;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Web.Http;

    using Microsoft.Owin.Hosting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Newtonsoft.Json;

    using Client.Entities;
    using GoingOn;
    using MemoryStorage;
    using Model.EntitiesBll;

    [TestClass]
    public class UserTests
    {
        private static IDisposable webService;
        private static IUserStorage storage;

        private static readonly UserClient userClient = new UserClient { Nickname = "Alberto", Password = "1234" };

        [TestInitialize]
        public void TestInitialize()
        {
            webService = WebApp.Start<Startup>("http://*:80/");
            storage = UserMemoryStorage.GetInstance();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            webService.Dispose();
            storage.DeleteAllUser();
        }

        [TestMethod]
        public void TestCreateUser()
        {
            var response = UserTests.CreateUser(userClient);

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            Assert.IsTrue(storage.ContainsUser(new UserBll{ Nickname = "Alberto", Password = "1234" }).Result);
        }

        [TestMethod]
        public void TestGetExistingUser()
        {
            UserTests.CreateUser(userClient);

            var response = UserTests.GetUser("Alberto");

            var content = response.Content;
            var jsonContent = content.ReadAsStringAsync().Result;
            UserClient actualUserClient = JsonConvert.DeserializeObject<UserClient>(jsonContent);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(new UserClientEqualityComparer().Equals(userClient, actualUserClient));
        }

        [TestMethod]
        public void TestUpdateUser()
        {
            HttpResponseMessage updateResponse, getResponse;

            UserTests.CreateUser(userClient);

            UserClient updatedUser = new UserClient { Nickname = "Alberto", Password = "4567" };

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:80/");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", "QWxiZXJ0bzoxMjM0");
                
                updateResponse = client.PutAsync("api/user/Alberto", updatedUser, new JsonMediaTypeFormatter()).Result;
            }

            getResponse = UserTests.GetUser("Alberto");

            var content = getResponse.Content;
            var jsonContent = content.ReadAsStringAsync().Result;
            UserClient actualUserClient = JsonConvert.DeserializeObject<UserClient>(jsonContent);

            Assert.AreEqual(HttpStatusCode.NoContent, updateResponse.StatusCode);
            Assert.AreEqual(HttpStatusCode.OK, getResponse.StatusCode);
            Assert.IsTrue(new UserClientEqualityComparer().Equals(updatedUser, actualUserClient));
        }

        [TestMethod]
        public void TestDeleteExistingUser()
        {
            UserTests.CreateUser(userClient);

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:80/");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", "QWxiZXJ0bzoxMjM0");

                var response = client.DeleteAsync("api/user/Alberto").Result;

                Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
                Assert.IsFalse(storage.ContainsUser(new UserBll{ Nickname = "Alberto", Password = "1234" }).Result);
            }
        }

        #region Helper methods

        public static HttpResponseMessage CreateUser(UserClient user)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:80/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                return client.PostAsJsonAsync("api/user", user).Result;
            }
        }

        private static HttpResponseMessage GetUser(string nickname)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:80/");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", "QWxiZXJ0bzoxMjM0");

                return client.GetAsync(string.Format("api/user/{0}", nickname)).Result;
            }
        }

        #endregion
    }
}
