// ****************************************************************************
// <copyright file="UserTests.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

using MemoryStorage;
using Model.EntitiesBll;

namespace EndToEndTests
{
    using System;
    using System.Net;
    using System.Net.Http.Headers;
    using System.Net.Http;
    using System.Web.Http;

    using Microsoft.Owin.Hosting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Client.Entities;

    using GoingOn;

    [TestClass]
    public class UserTests
    {
        private static IDisposable webService;

        private HttpServer server;

        private static readonly User user = new User { Nickname = "Alberto", Password = "1234" };

        [TestInitialize]
        public void TestInitialize()
        {
            webService = WebApp.Start<Startup>("http://*:80/");
        }

        [TestCleanup]
        public void TestCleanup()
        {
            webService.Dispose();
        }

        [TestMethod]
        public void TestCreateUser()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:80/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var task = client.PostAsJsonAsync("api/user", user);
                task.Wait();
                var response = task.Result;

                Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

                IUserStorage storage = UserMemoryStorage.GetInstance();
                Assert.IsTrue(storage.ContainsUser(new UserBll("Alberto", "1234")));
            }
        }

        [TestMethod]
        public void TestGetExistingUser()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:80/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var task = client.PostAsJsonAsync("api/user", user);
                task.Wait();
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:80/");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", "QWxiZXJ0bzoxMjM0");

                var task = client.GetAsync("api/user?nickname=Alberto");
                task.Wait();
                var response = task.Result;

                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                // Get the user
            }
        }
    }
}
