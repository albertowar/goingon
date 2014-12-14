// ****************************************************************************
// <copyright file="UserTests.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Net.Http;
using System.Web.Http;

namespace EndToEndTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class UserTests
    {
        private HttpServer server;
        private HttpClient client;

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
            client = new HttpClient(server);
        }

        [TestMethod]
        public void TestCreateUser()
        {
            // transform json
            //var task = client.PostAsync("http://test.com/api/user", new StringContent(@"{"n"}"));
        }
    }
}
