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
    using System.Net.Http.Headers;
    using System.Net.Http;
    using System.Net.Http.Formatting;

    using Microsoft.Owin.Hosting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Newtonsoft.Json;

    using Client.Entities;
    using Frontend;
    using Frontend.Entities;
    using Model.EntitiesBll;
    using Storage;
    using Storage.TableStorage;

    [TestClass]
    public class UserTests
    {
        private static IDisposable webService;
        private static IUserStorage storage;

        private static readonly UserClient userClient = new UserClient { Nickname = "Alberto", Password = "1234", City = "Malaga" };

        [TestInitialize]
        public void TestInitialize()
        {
            webService = WebApp.Start<Startup>("http://*:80/");
            storage = UserTableStorage.GetInstance();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            webService.Dispose();
            storage.DeleteAllUsers().Wait();
        }

        [TestMethod]
        public void TestCreateUser()
        {
            UserTests.CreateUser(userClient);

            Assert.IsTrue(storage.ContainsUser(new UserBll{ Nickname = "Alberto", Password = "1234", City = "Malaga" }).Result);
        }

        [TestMethod]
        public void TestGetExistingUser()
        {
            UserTests.CreateUser(userClient);

            var response = UserTests.GetUser("Alberto");

            var content = response.Content;
            var jsonContent = content.ReadAsStringAsync().Result;
            var actualUserREST = JsonConvert.DeserializeObject<UserREST>(jsonContent);

            Assert.IsTrue(new UserClientEqualityComparer().Equals(userClient, UserClient.FromUserREST(actualUserREST)));
        }

        [TestMethod]
        public void TestUpdateUser()
        {
            UserTests.CreateUser(userClient);

            UserClient updatedUser = new UserClient { Nickname = "Alberto", Password = "4567", City = "Malaga" };

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:80/");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", "QWxiZXJ0bzoxMjM0");
                
                client.PatchAsync("api/user/Alberto", updatedUser, new JsonMediaTypeFormatter()).Wait();
            }

            var getResponse = UserTests.GetUser("Alberto");

            var content = getResponse.Content;
            var jsonContent = content.ReadAsStringAsync().Result;
            var actualUserREST = JsonConvert.DeserializeObject<UserREST>(jsonContent);

            Assert.IsTrue(new UserClientEqualityComparer().Equals(updatedUser, UserClient.FromUserREST(actualUserREST)));
        }

        [TestMethod]
        public void TestUpdateUnauthorizedUser()
        {
            var otherUser = new UserClient { Nickname = "NotAlberto", Password = "1234" };
            UserClient updatedUser = new UserClient { Nickname = "NotAlberto", Password = "4567" };

            UserTests.CreateUser(userClient);
            UserTests.CreateUser(otherUser);

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:80/");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", "QWxiZXJ0bzoxMjM0");

                client.PatchAsync("api/user/" + otherUser.Nickname, updatedUser, new JsonMediaTypeFormatter()).Wait();
            }

            Assert.IsTrue(storage.ContainsUser(new UserBll { Nickname = "NotAlberto", Password = "1234" }).Result);
        }

        [TestMethod]
        public void TestDeleteExistingUser()
        {
            UserTests.CreateUser(userClient);

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:80/");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", "QWxiZXJ0bzoxMjM0");

                client.DeleteAsync("api/user/Alberto").Wait();

                Assert.IsFalse(storage.ContainsUser(new UserBll{ Nickname = "Alberto", Password = "1234" }).Result);
            }
        }

        [TestMethod]
        public void TestDeleteUnauthorizedUser()
        {
            var otherUser = new UserClient { Nickname = "NotAlberto", Password = "1234" };

            UserTests.CreateUser(userClient);
            UserTests.CreateUser(otherUser);

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:80/");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", "QWxiZXJ0bzoxMjM0");

                client.DeleteAsync("api/user/" + otherUser.Nickname).Wait();

                Assert.IsTrue(storage.ContainsUser(new UserBll { Nickname = "Alberto", Password = "1234" }).Result);
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
