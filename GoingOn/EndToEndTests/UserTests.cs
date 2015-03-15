// ****************************************************************************
// <copyright file="UserTests.cs" company="Universidad de Malaga">
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
    using System.Net;

    using GoingOn.Frontend;
    using GoingOn.Frontend.Entities;
    using GoingOn.Client;
    using GoingOn.Client.Entities;
    using GoingOn.Model.EntitiesBll;
    using GoingOn.Storage;
    using GoingOn.Storage.TableStorage;

    using Microsoft.Owin.Hosting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    
    using Newtonsoft.Json;

    [TestClass]
    public class UserTests
    {
        private GOClient goClient;

        private IDisposable webService;

        private IUserStorage storage;

        private static readonly UserClient userClient = new UserClient { Nickname = "Alberto", Password = "1234", City = "Malaga" };

        [TestInitialize]
        public void TestInitialize()
        {
            this.webService = WebApp.Start<Startup>("http://*:80/");
            this.storage = UserTableStorage.GetInstance();
            this.goClient = new GOClient(@"http://localhost:80/", "Alberto", "1234");
        }

        [TestCleanup]
        public void TestCleanup()
        {
            this.webService.Dispose();
            this.storage.DeleteAllUsers().Wait();
        }

        [TestMethod]
        public void TestCreateUser()
        {
            this.goClient.CreateUser(userClient).Wait();

            Assert.IsTrue(this.storage.ContainsUser(new UserBll { Nickname = "Alberto", Password = "1234", City = "Malaga" }).Result);
        }

        [TestMethod]
        public void TestGetExistingUser()
        {
            this.goClient.CreateUser(userClient).Wait();

            var response = this.goClient.GetUser("Alberto").Result;

            var content = response.Content;
            var jsonContent = content.ReadAsStringAsync().Result;
            var actualUserREST = JsonConvert.DeserializeObject<UserREST>(jsonContent);

            Assert.IsTrue(new UserClientEqualityComparer().Equals(userClient, UserClient.FromUserREST(actualUserREST)));
        }

        [TestMethod]
        public void TestUpdateUser()
        {
            this.goClient.CreateUser(userClient).Wait();

            UserClient updatedUser = new UserClient { Nickname = "Alberto", Password = "4567", City = "Malaga" };

            this.goClient.UpdateUser(updatedUser).Wait();

            var getResponse = this.goClient.GetUser("Alberto").Result;

            var content = getResponse.Content;
            var jsonContent = content.ReadAsStringAsync().Result;
            var actualUserREST = JsonConvert.DeserializeObject<UserREST>(jsonContent);

            Assert.IsTrue(new UserClientEqualityComparer().Equals(updatedUser, UserClient.FromUserREST(actualUserREST)));
        }

        [TestMethod]
        public void TestUpdateUnauthorizedUser()
        {
            var otherUser = new UserClient { Nickname = "NotAlberto", Password = "1234", City = "Malaga" };
            UserClient updatedUser = new UserClient { Nickname = "NotAlberto", Password = "4567", City = "Malaga" };

            this.goClient.CreateUser(userClient).Wait();
            this.goClient.CreateUser(otherUser).Wait();

            var response = this.goClient.UpdateUser(updatedUser).Result;

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.IsTrue(this.storage.ContainsUser(new UserBll { Nickname = "NotAlberto", Password = "1234" }).Result);
        }

        [TestMethod]
        public void TestDeleteExistingUser()
        {
            this.goClient.CreateUser(userClient).Wait();

            this.goClient.DeleteUser("Alberto").Wait();

            Assert.IsFalse(this.storage.ContainsUser(new UserBll { Nickname = "Alberto", Password = "1234" }).Result);
        }

        [TestMethod]
        public void TestDeleteUnauthorizedUser()
        {
            var otherUser = new UserClient { Nickname = "NotAlberto", Password = "1234" };

            this.goClient.CreateUser(userClient).Wait();
            this.goClient.CreateUser(otherUser).Wait();

            var response = this.goClient.DeleteUser(otherUser.Nickname).Result;

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.IsTrue(this.storage.ContainsUser(new UserBll { Nickname = "Alberto", Password = "1234" }).Result);
        }
    }
}
