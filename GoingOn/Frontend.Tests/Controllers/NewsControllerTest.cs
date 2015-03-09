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

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Common.Tests;
    using Frontend.Controllers;
    using Frontend.Entities;
    using Frontend.Validation;
    using Model.EntitiesBll;
    using Storage;

    using Moq;
    using Newtonsoft.Json;
    using WebApiContrib.Testing;

    [TestClass]
    public class NewsControllerTest
    {
        private Mock<INewsStorage> newsStorageMock;
        private Mock<IApiInputValidationChecks> inputValidation;
        private Mock<IApiBusinessLogicValidationChecks> businessValidation;

        private static readonly User user = new User { Nickname = "nickname", Password = "password", City = "Malaga" };
        private static readonly News news = new News { Title = "title", Content = "content" };

        private const string UriRoot = "http://test.com/api/news/";
        private const string City = "Malaga";
        private const string Date = "2015-05-11";

        [TestInitialize]
        public void Initizalize()
        {
            this.newsStorageMock = new Mock<INewsStorage>();
            this.inputValidation = new Mock<IApiInputValidationChecks>();
            this.businessValidation = new Mock<IApiBusinessLogicValidationChecks>();
        }

        [TestMethod]
        public void TestGetNewsReturns200OkWhenTheNewsIsInTheDatabase()
        {
            Guid guid = Guid.NewGuid();

            this.inputValidation.Setup(validation => validation.IsValidNewsId(It.IsAny<string>())).Returns(true);
            this.businessValidation.Setup(validation => validation.IsValidGetNews(this.newsStorageMock.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(true));
            this.newsStorageMock.Setup(storage => storage.GetNews(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(News.ToNewsBll(guid, news, user.Nickname)));

            var newsController = new NewsController(this.newsStorageMock.Object, this.inputValidation.Object, this.businessValidation.Object);
            //newsController.ConfigureForTesting(HttpMethod.Get, string.Format("{0}/{1}/{2}/{3}", UriRoot, City, Date, guid), "DefaultApi", new HttpRoute("api/{controller}/{id}"));
            newsController.ConfigureForTesting(HttpMethod.Get, string.Format("{0}/{1}/{2}/{3}", UriRoot, City, Date, guid));

            var response = newsController.Get(City, Date, guid.ToString()).Result;

            var content = response.Content;
            var jsonContent = content.ReadAsStringAsync().Result;
            var actualNews = JsonConvert.DeserializeObject<NewsREST>(jsonContent);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(new NewsCompleteEqualityComparer().Equals(news, actualNews.News));
            Assert.IsTrue(actualNews.Links.Any(link => string.Equals("self", link.Rel)));
            Assert.AreEqual(new Uri("http://test.com/api/news/" + guid), actualNews.Links.First(link => string.Equals("self", link.Rel)).Href);
            Assert.IsTrue(actualNews.Links.Any(link => string.Equals("author", link.Rel)));
            Assert.AreEqual(new Uri("http://test.com/api/user/nickname"), actualNews.Links.First(link => string.Equals("author", link.Rel)).Href);
        }

        [TestMethod]
        public void TestGetNewsReturns400BadRequestWhenInputValidationFails()
        {
            var guid = Guid.NewGuid();

            this.inputValidation.Setup(validation => validation.IsValidNewsId(It.IsAny<string>())).Returns(false);
            this.businessValidation.Setup(validation => validation.IsValidGetNews(this.newsStorageMock.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(true));
            this.newsStorageMock.Setup(storage => storage.GetNews(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(News.ToNewsBll(guid, news, user.Nickname)));

            var newsController = new NewsController(this.newsStorageMock.Object, this.inputValidation.Object, this.businessValidation.Object);
            //newsController.ConfigureForTesting(HttpMethod.Get, string.Format("{0}/{1}/{2}/{3}", UriRoot, City, Date, guid), "DefaultApi", new HttpRoute("api/{controller}/{id}"));
            newsController.ConfigureForTesting(HttpMethod.Get, string.Format("{0}/{1}/{2}/{3}", UriRoot, City, Date, guid));

            var response = newsController.Get(City, Date, guid.ToString()).Result;

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public void TestGetNewsReturns404NotFoundWhenBusinessValidationFails()
        {
            var guid = Guid.NewGuid();

            this.inputValidation.Setup(validation => validation.IsValidNewsId(It.IsAny<string>())).Returns(true);
            this.businessValidation.Setup(validation => validation.IsValidGetNews(this.newsStorageMock.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(false));
            this.newsStorageMock.Setup(storage => storage.GetNews(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(News.ToNewsBll(guid, news, user.Nickname)));

            var newsController = new NewsController(this.newsStorageMock.Object, this.inputValidation.Object, this.businessValidation.Object);
            //newsController.ConfigureForTesting(HttpMethod.Get, string.Format("{0}/{1}/{2}/{3}", UriRoot, City, Date, guid), "DefaultApi", new HttpRoute("api/{controller}/{id}"));
            newsController.ConfigureForTesting(HttpMethod.Get, string.Format("{0}/{1}/{2}/{3}", UriRoot, City, Date, guid));

            var response = newsController.Get(City, Date, guid.ToString()).Result;

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public void TestPostUserReturns200OkWhenCreatesNews()
        {
            this.inputValidation.Setup(validation => validation.IsValidNews(It.IsAny<News>())).Returns(true);
            this.businessValidation.Setup(validation => validation.IsValidCreateNews(this.newsStorageMock.Object, It.IsAny<News>(), It.IsAny<string>())).Returns(true);

            var newsController = new NewsController(this.newsStorageMock.Object, this.inputValidation.Object, this.businessValidation.Object);
            //newsController.ConfigureForTesting(HttpMethod.Post, "http://test.com/api/news/", "DefaultApi", new HttpRoute("api/{controller}/{id}"));
            newsController.ConfigureForTesting(HttpMethod.Post, string.Format("{0}/{1}/{2}/{3}", UriRoot, City, Date));
            newsController.User = new GenericPrincipal(new GenericIdentity(user.Nickname), null); 

            HttpResponseMessage response = newsController.Post(City, Date, news).Result;

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            Assert.IsTrue(new Uri(UriRoot).IsBaseOf(response.Headers.Location));
            this.newsStorageMock.Verify(storage => storage.AddNews(It.IsAny<NewsBll>()), Times.Once());
        }

        [TestMethod]
        public void TestPostNewsReturns400BadRequestWhenInputValidationFails()
        {
            this.inputValidation.Setup(validation => validation.IsValidNews(It.IsAny<News>())).Returns(false);
            this.businessValidation.Setup(validation => validation.IsValidCreateNews(this.newsStorageMock.Object, It.IsAny<News>(), It.IsAny<string>())).Returns(true);

            var newsController = new NewsController(this.newsStorageMock.Object, this.inputValidation.Object, this.businessValidation.Object);
            //newsController.ConfigureForTesting(HttpMethod.Post, "http://test.com/api/news/", "DefaultApi", new HttpRoute("api/{controller}/{id}"));
            newsController.ConfigureForTesting(HttpMethod.Post, string.Format("{0}/{1}/{2}/{3}", UriRoot, City, Date));
            newsController.User = new GenericPrincipal(new GenericIdentity(user.Nickname), null);

            HttpResponseMessage response = newsController.Post(City, Date, news).Result;

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            this.newsStorageMock.Verify(storage => storage.AddNews(It.IsAny<NewsBll>()), Times.Never());
        }

        [TestMethod]
        public void TestPostNewsReturns400BadRequestWhenBusinessValidationFails()
        {
            this.inputValidation.Setup(validation => validation.IsValidNews(It.IsAny<News>())).Returns(true);
            this.businessValidation.Setup(validation => validation.IsValidCreateNews(newsStorageMock.Object, It.IsAny<News>(), It.IsAny<string>())).Returns(false);

            var newsController = new NewsController(this.newsStorageMock.Object, this.inputValidation.Object, this.businessValidation.Object);
            //newsController.ConfigureForTesting(HttpMethod.Post, "http://test.com/api/news/", "DefaultApi", new HttpRoute("api/{controller}/{id}"));
            newsController.ConfigureForTesting(HttpMethod.Post, string.Format("{0}/{1}/{2}/{3}", UriRoot, City, Date));
            newsController.User = new GenericPrincipal(new GenericIdentity(user.Nickname), null);

            HttpResponseMessage response = newsController.Post(City, Date, news).Result;

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            this.newsStorageMock.Verify(storage => storage.AddNews(It.IsAny<NewsBll>()), Times.Never());
        }

        [TestMethod]
        public void TestDeleteNewsReturns204NoContentWhenUpdatesUser()
        {
            var guid = Guid.NewGuid();

            this.inputValidation.Setup(validation => validation.IsValidNewsId(It.IsAny<string>())).Returns(true);
            this.businessValidation.Setup(validation => validation.IsValidDeleteNews(this.newsStorageMock.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>(), It.IsAny<string>())).Returns(Task.FromResult(true));

            var newsController = new NewsController(this.newsStorageMock.Object, this.inputValidation.Object, this.businessValidation.Object);
            //newsController.ConfigureForTesting(HttpMethod.Delete, "http://test.com/api/news/" + guid, "DefaultApi", new HttpRoute("api/{controller}/{id}"));
            newsController.ConfigureForTesting(HttpMethod.Delete, string.Format("{0}/{1}/{2}/{3}", UriRoot, City, Date, guid));
            newsController.User = new GenericPrincipal(new GenericIdentity(user.Nickname), null); 

            HttpResponseMessage response = newsController.Delete(City, Date, guid.ToString()).Result;

            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
            this.newsStorageMock.Verify(storage => storage.DeleteNews(City, DateTime.Parse(Date), guid), Times.Once());
        }

        [TestMethod]
        public void TestDeleteNewsReturns400BadRequestWhenInputValidationFails()
        {
            var guid = Guid.NewGuid();

            this.inputValidation.Setup(validation => validation.IsValidCity(It.IsAny<string>())).Returns(false);
            this.inputValidation.Setup(validation => validation.IsValidNewsDate(It.IsAny<string>())).Returns(false);
            this.inputValidation.Setup(validation => validation.IsValidNewsId(It.IsAny<string>())).Returns(false);
            this.businessValidation.Setup(validation => validation.IsValidDeleteNews(this.newsStorageMock.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>(), It.IsAny<string>())).Returns(Task.FromResult(true));

            var newsController = new NewsController(this.newsStorageMock.Object, this.inputValidation.Object, this.businessValidation.Object);
            //newsController.ConfigureForTesting(HttpMethod.Delete, "http://test.com/api/news/" + guid, "DefaultApi", new HttpRoute("api/{controller}/{id}"));
            newsController.ConfigureForTesting(HttpMethod.Delete, string.Format("{0}/{1}/{2}/{3}", UriRoot, City, Date, guid));
            newsController.User = new GenericPrincipal(new GenericIdentity(user.Nickname), null);

            HttpResponseMessage response = newsController.Delete(City, Date, guid.ToString()).Result;

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            this.newsStorageMock.Verify(storage => storage.DeleteNews(City, DateTime.Parse(Date), guid), Times.Never());
        }

        [TestMethod]
        public void TestDeleteNewsReturns404NotFoundWhenBusinessValidationFails()
        {
            var guid = Guid.NewGuid();

            this.inputValidation.Setup(validation => validation.IsValidNewsId(It.IsAny<string>())).Returns(true);
            this.businessValidation.Setup(validation => validation.IsValidDeleteNews(this.newsStorageMock.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>(), It.IsAny<string>())).Returns(Task.FromResult(false));

            var newsController = new NewsController(this.newsStorageMock.Object, this.inputValidation.Object, this.businessValidation.Object);
            //newsController.ConfigureForTesting(HttpMethod.Delete, "http://test.com/api/news/" + guid, "DefaultApi", new HttpRoute("api/{controller}/{id}"));
            newsController.ConfigureForTesting(HttpMethod.Delete, string.Format("{0}/{1}/{2}/{3}", UriRoot, City, Date, guid));
            newsController.User = new GenericPrincipal(new GenericIdentity(user.Nickname), null);

            HttpResponseMessage response = newsController.Delete(City, Date, guid.ToString()).Result;

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            this.newsStorageMock.Verify(storage => storage.DeleteNews(City, DateTime.Parse(Date), guid), Times.Never());
        }
    }
}
