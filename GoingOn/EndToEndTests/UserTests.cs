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
    using System.Web.Http;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Client.Entities;

    [TestClass]
    public class UserTests
    {
        private HttpServer server;

        [TestInitialize]
        public void TestInitialize()
        {
            var configuration = new HttpConfiguration();

            configuration.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            server = new HttpServer(configuration);
        }

        [TestMethod]
        public void TestCreateUser()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:4162/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var user = new User { Nickname = "Alberto", Password = "1234" };
                var task = client.PostAsJsonAsync("api/user", user);
                task.Wait();
                var response = task.Result;

                Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            }
        }
    }
}
