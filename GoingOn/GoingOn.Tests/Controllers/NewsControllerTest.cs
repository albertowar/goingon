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
    using System.Security.Principal;
    using System.Threading.Tasks;
    using System.Web.Http.Routing;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Common.Tests;
    using GoingOn.Controllers;
    using GoingOn.Entities;
    using GoingOn.Validation;
    using Model.EntitiesBll;

    using Moq;
    using Newtonsoft.Json;
    using WebApiContrib.Testing;

    [TestClass]
    public class NewsControllerTest
    {
        private Mock<INewsStorage> newsStorageMock;
        private Mock<IApiInputValidationChecks> inputValidation;
        private Mock<IApiBusinessLogicValidationChecks> businessValidation;

        private static readonly User user = new User("nickname", "password");
        private static readonly News news = new News("title", "content");

        [TestInitialize]
        public void Initizalize()
        {
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

            var newsController = new NewsController(newsStorageMock.Object, inputValidation.Object, businessValidation.Object);
            newsController.ConfigureForTesting(HttpMethod.Get, "http://test.com/api/news/" + guid, "DefaultApi", new HttpRoute("api/{controller}/{id}"));

            var response = newsController.Get(guid.ToString()).Result;

            var content = response.Content;
            var jsonContent = content.ReadAsStringAsync().Result;
            var actualNews = JsonConvert.DeserializeObject<News>(jsonContent);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(new NewsCompleteEqualityComparer().Equals(news, actualNews));
            Assert.IsTrue(actualNews.Links.Any(link => string.Equals("self", link.Rel)));
            Assert.AreEqual(new Uri("http://test.com/api/news/" + guid), actualNews.Links.First(link => string.Equals("self", link.Rel)).Href);
            Assert.IsTrue(actualNews.Links.Any(link => string.Equals("author", link.Rel)));
            Assert.AreEqual(new Uri("http://test.com/api/user/nickname"), actualNews.Links.First(link => string.Equals("author", link.Rel)).Href);
        }

        [TestMethod]
        public void TestGetNewsReturns400BadRequestWhenInputValidationFails()
        {
            var guid = Guid.NewGuid();

            inputValidation.Setup(validation => validation.IsValidNewsId(It.IsAny<string>())).Returns(false);
            businessValidation.Setup(validation => validation.IsValidGetNews(newsStorageMock.Object, It.IsAny<string>())).Returns(true);
            newsStorageMock.Setup(storage => storage.GetNews(It.IsAny<Guid>())).Returns(Task.FromResult(News.ToNewsBll(news, guid, user.Nickname, DateTime.Now)));

            var newsController = new NewsController(newsStorageMock.Object, inputValidation.Object, businessValidation.Object);
            newsController.ConfigureForTesting(HttpMethod.Get, "http://test.com/api/news/" + guid, "DefaultApi", new HttpRoute("api/{controller}/{id}"));

            var response = newsController.Get(guid.ToString()).Result;

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public void TestGetNewsReturns404NotFoundWhenBusinessValidationFails()
        {
            var guid = Guid.NewGuid();

            inputValidation.Setup(validation => validation.IsValidNewsId(It.IsAny<string>())).Returns(true);
            businessValidation.Setup(validation => validation.IsValidGetNews(newsStorageMock.Object, It.IsAny<string>())).Returns(false);
            newsStorageMock.Setup(storage => storage.GetNews(It.IsAny<Guid>())).Returns(Task.FromResult(News.ToNewsBll(news, guid, user.Nickname, DateTime.Now)));

            var newsController = new NewsController(newsStorageMock.Object, inputValidation.Object, businessValidation.Object);
            newsController.ConfigureForTesting(HttpMethod.Get, "http://test.com/api/news/" + guid, "DefaultApi", new HttpRoute("api/{controller}/{id}"));

            var response = newsController.Get(guid.ToString()).Result;

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public void TestPostUserReturns200OkWhenCreatesNews()
        {
            inputValidation.Setup(validation => validation.IsValidNews(It.IsAny<News>())).Returns(true);
            businessValidation.Setup(validation => validation.IsValidCreateNews(newsStorageMock.Object, It.IsAny<News>(), It.IsAny<string>())).Returns(true);

            var newsController = new NewsController(newsStorageMock.Object, inputValidation.Object, businessValidation.Object);
            newsController.ConfigureForTesting(HttpMethod.Post, "http://test.com/api/news/", "DefaultApi", new HttpRoute("api/{controller}/{id}"));
            newsController.User = new GenericPrincipal(new GenericIdentity(user.Nickname), null); 

            HttpResponseMessage response = newsController.Post(news).Result;

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            Assert.IsTrue(new Uri("http://test.com/api/news/").IsBaseOf(response.Headers.Location));
            newsStorageMock.Verify(storage => storage.AddNews(It.IsAny<NewsBll>()), Times.Once());
        }

        [TestMethod]
        public void TestPostNewsReturns400BadRequestWhenInputValidationFails()
        {
            inputValidation.Setup(validation => validation.IsValidNews(It.IsAny<News>())).Returns(false);
            businessValidation.Setup(validation => validation.IsValidCreateNews(newsStorageMock.Object, It.IsAny<News>(), It.IsAny<string>())).Returns(true);

            var newsController = new NewsController(newsStorageMock.Object, inputValidation.Object, businessValidation.Object);
            newsController.ConfigureForTesting(HttpMethod.Post, "http://test.com/api/news/", "DefaultApi", new HttpRoute("api/{controller}/{id}"));
            newsController.User = new GenericPrincipal(new GenericIdentity(user.Nickname), null); 

            HttpResponseMessage response = newsController.Post(news).Result;

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            newsStorageMock.Verify(storage => storage.AddNews(It.IsAny<NewsBll>()), Times.Never());
        }

        [TestMethod]
        public void TestPostNewsReturns400BadRequestWhenBusinessValidationFails()
        {
            inputValidation.Setup(validation => validation.IsValidNews(It.IsAny<News>())).Returns(true);
            businessValidation.Setup(validation => validation.IsValidCreateNews(newsStorageMock.Object, It.IsAny<News>(), It.IsAny<string>())).Returns(false);

            var newsController = new NewsController(newsStorageMock.Object, inputValidation.Object, businessValidation.Object);
            newsController.ConfigureForTesting(HttpMethod.Post, "http://test.com/api/news/", "DefaultApi", new HttpRoute("api/{controller}/{id}"));
            newsController.User = new GenericPrincipal(new GenericIdentity(user.Nickname), null); 

            HttpResponseMessage response = newsController.Post(news).Result;

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            newsStorageMock.Verify(storage => storage.AddNews(It.IsAny<NewsBll>()), Times.Never());
        }

        [TestMethod]
        public void TestDeleteUserReturns204NoContentWhenUpdatesUser()
        {
            var guid = Guid.NewGuid();

            inputValidation.Setup(validation => validation.IsValidNewsId(It.IsAny<string>())).Returns(true);
            businessValidation.Setup(validation => validation.IsValidDeleteNews(newsStorageMock.Object, It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            var newsController = new NewsController(newsStorageMock.Object, inputValidation.Object, businessValidation.Object);
            newsController.ConfigureForTesting(HttpMethod.Delete, "http://test.com/api/news/" + guid, "DefaultApi", new HttpRoute("api/{controller}/{id}"));

            HttpResponseMessage response = newsController.Delete(guid.ToString()).Result;

            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
            newsStorageMock.Verify(storage => storage.DeleteNews(guid), Times.Once());
        }

        [TestMethod]
        public void TestDeleteUserReturns400BadRequestWhenInputValidationFails()
        {
            var guid = Guid.NewGuid();

            inputValidation.Setup(validation => validation.IsValidNewsId(It.IsAny<string>())).Returns(false);
            businessValidation.Setup(validation => validation.IsValidDeleteNews(newsStorageMock.Object, It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            var newsController = new NewsController(newsStorageMock.Object, inputValidation.Object, businessValidation.Object);
            newsController.ConfigureForTesting(HttpMethod.Delete, "http://test.com/api/news/" + guid, "DefaultApi", new HttpRoute("api/{controller}/{id}"));

            HttpResponseMessage response = newsController.Delete(guid.ToString()).Result;

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            newsStorageMock.Verify(storage => storage.DeleteNews(guid), Times.Never());
        }

        [TestMethod]
        public void TestDeleteUserReturns404NotFoundWhenBusinessValidationFails()
        {
            var guid = Guid.NewGuid();

            inputValidation.Setup(validation => validation.IsValidNewsId(It.IsAny<string>())).Returns(true);
            businessValidation.Setup(validation => validation.IsValidDeleteNews(newsStorageMock.Object, It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            var newsController = new NewsController(newsStorageMock.Object, inputValidation.Object, businessValidation.Object);
            newsController.ConfigureForTesting(HttpMethod.Delete, "http://test.com/api/news/" + guid, "DefaultApi", new HttpRoute("api/{controller}/{id}"));

            HttpResponseMessage response = newsController.Delete(guid.ToString()).Result;

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            newsStorageMock.Verify(storage => storage.DeleteNews(guid), Times.Never());
        }
    }
}
