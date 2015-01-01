// ****************************************************************************
// <copyright file="NewsControllerTest.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace GoingOn.Tests.Controllers
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http.Routing;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Common.Tests;
    using GoingOn.Entities;
    using GoingOn.Controllers;
    using GoingOn.Validation;

    using Moq;
    using Newtonsoft.Json;
    using WebApiContrib.Testing;

    [TestClass]
    public class NewsControllerTest
    {
        private Mock<IUserStorage> userStorageMock;
        private Mock<INewsStorage> newsStorageMock;
        private Mock<IApiInputValidationChecks> inputValidation;
        private Mock<IApiBusinessLogicValidationChecks> businessValidation;

        private static readonly User user = new User("nickname", "password");
        private static readonly News news = new News("title", "content");

        [TestInitialize]
        public void Initizalize()
        {
            userStorageMock = new Mock<IUserStorage>();
            newsStorageMock = new Mock<INewsStorage>();
            inputValidation = new Mock<IApiInputValidationChecks>();
            businessValidation = new Mock<IApiBusinessLogicValidationChecks>();
        }

        [TestMethod]
        public void TestGetNewsReturns200OkWhenTheNewsIsInTheDatabase()
        {
            Guid guid = Guid.NewGuid();

            inputValidation.Setup(validation => validation.IsValidNewsId(It.IsAny<string>())).Returns(true);
            businessValidation.Setup(validation => validation.IsValidGetNews(newsStorageMock.Object, It.IsAny<string>())).Returns(true);
            newsStorageMock.Setup(storage => storage.GetNews(It.IsAny<Guid>())).Returns(Task.FromResult(News.ToNewsBll(news, guid, user.Nickname, DateTime.Now)));

            NewsController newsController = new NewsController(newsStorageMock.Object, inputValidation.Object, businessValidation.Object);
            newsController.ConfigureForTesting(HttpMethod.Get, "http://test.com/api/news/" + guid, "DefaultApi", new HttpRoute("api/{controller}/{id}"));

            HttpResponseMessage response = newsController.Get(guid.ToString()).Result;

            var content = response.Content;
            var jsonContent = content.ReadAsStringAsync().Result;
            News actualNews = JsonConvert.DeserializeObject<News>(jsonContent);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(new NewsCompleteEqualityComparer().Equals(news, actualNews));
            Assert.IsTrue(actualNews.Links.Any());
            Assert.AreEqual("self", actualNews.Links.First().Rel);
            Assert.AreEqual(new Uri("http://test.com/api/news/" + guid), actualNews.Links.First().Href);
        }
    }
}
