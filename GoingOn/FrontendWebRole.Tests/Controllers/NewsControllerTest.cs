// ****************************************************************************
// <copyright file="NewsControllerTest.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// NewsController tests class
// </summary>
// ****************************************************************************

namespace GoingOn.FrontendWebRole.Tests.Controllers
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Security.Principal;
    using System.Threading.Tasks;
    using System.Web.Http.Routing;
    using GoingOn.Common;
    using GoingOn.Common.Tests;
    using GoingOn.Frontend.Common;
    using GoingOn.Frontend.Entities;
    using GoingOn.Frontend.Validation;
    using GoingOn.FrontendWebRole.Controllers;
    using GoingOn.Model.EntitiesBll;
    using GoingOn.Storage;
    using GoingOn.Storage.TableStorage.Entities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Newtonsoft.Json;
    using WebApiContrib.Testing;

    [TestClass]
    public class NewsControllerTest
    {
        private Mock<INewsStorage> newsStorageMock;
        private Mock<IApiInputValidationChecks> inputValidation;
        private Mock<IApiBusinessLogicValidationChecks> businessValidation;

        private static readonly User User = new User { Nickname = "nickname", Password = "password", City = "Malaga" };
        private static readonly News News = new News { Title = "title", Content = "content" };

        private const string Scheme = "http";
        private const string Host = "test.com";
        private const int Port = 123;

        private static readonly string NewsUriRoot = string.Format("{0}://{1}:{2}", Scheme, Host, Port);

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

            this.businessValidation.Setup(validation => validation.IsValidGetNews(this.newsStorageMock.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(true));
            this.newsStorageMock.Setup(storage => storage.GetNews(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(News.ToNewsBll(guid, News, City, User.Nickname, DateTime.Parse(Date))));

            var newsController = new NewsController(this.newsStorageMock.Object, this.inputValidation.Object, this.businessValidation.Object);

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, GOUriBuilder.BuildAbsoluteNewsUri(Scheme, Host, Port, City, Date, guid.ToString()));
            request.Headers.Referrer = new Uri(GOUriBuilder.BuildAbsoluteNewsUri(Scheme, Host, Port, City, Date, guid.ToString()));

            newsController.ConfigureForTesting(request, "GetNews", new HttpRoute(GOUriBuilder.GetNewsTemplate));

            HttpResponseMessage response = newsController.Get(City, Date, guid.ToString()).Result;

            HttpContent content = response.Content;
            string jsonContent = content.ReadAsStringAsync().Result;
            var actualNews = JsonConvert.DeserializeObject<NewsREST>(jsonContent);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(new NewsCompleteEqualityComparer().Equals(News, NewsREST.ToNews(actualNews)));
            Assert.IsTrue(actualNews.Links.Any(link => string.Equals("self", link.Rel)));
            Assert.AreEqual(new Uri(GOUriBuilder.BuildAbsoluteNewsUri(Scheme, Host, Port, City, Date, guid.ToString())), actualNews.Links.First(link => string.Equals("self", link.Rel)).Href);
            Assert.IsTrue(actualNews.Links.Any(link => string.Equals("author", link.Rel)));
            Assert.AreEqual(new Uri(GOUriBuilder.BuildAbsoluteUserUri(Scheme, Host, Port, NewsControllerTest.User.Nickname)), actualNews.Links.First(link => string.Equals("author", link.Rel)).Href);
        }

        [TestMethod]
        public void TestGetNewsReturns400BadRequestWhenNewsParametersValidationFails()
        {
            Guid guid = Guid.NewGuid();

            this.inputValidation.Setup(validation => validation.ValidateNewsParameters(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Throws(new InputValidationException(HttpStatusCode.BadRequest, string.Empty));

            this.GetNewsFails(HttpStatusCode.BadRequest, guid);
        }

        [TestMethod]
        public void TestGetNewsReturns404NotFoundWhenBusinessValidationFails()
        {
            Guid guid = Guid.NewGuid();

            this.businessValidation.Setup(validation => validation.IsValidGetNews(this.newsStorageMock.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(false));
            this.newsStorageMock.Setup(storage => storage.GetNews(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(News.ToNewsBll(guid, News, City, User.Nickname, DateTime.Parse(Date))));

            this.GetNewsFails(HttpStatusCode.NotFound, guid);
        }

        [TestMethod]
        public void TestPostUserReturns200OkWhenCreatesNews()
        {
            this.inputValidation.Setup(validation => validation.IsValidNews(It.IsAny<News>())).Returns(true);
            this.businessValidation.Setup(validation => validation.IsValidCreateNews(this.newsStorageMock.Object, It.IsAny<News>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>())).Returns(Task.FromResult(true));

            var newsController = new NewsController(this.newsStorageMock.Object, this.inputValidation.Object, this.businessValidation.Object);

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, GOUriBuilder.BuildAbsoluteDiaryEntryUri(Scheme, Host, Port, City, Date));
            request.Headers.Referrer = new Uri(GOUriBuilder.BuildAbsoluteDiaryEntryUri(Scheme, Host, Port, City, Date));

            newsController.ConfigureForTesting(request, "PostNews", new HttpRoute(GOUriBuilder.PostNewsTemplate));
            newsController.User = new GenericPrincipal(new GenericIdentity(User.Nickname), null); 

            HttpResponseMessage response = newsController.Post(City, Date, News).Result;

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            Assert.IsTrue(new Uri(NewsUriRoot).IsBaseOf(response.Headers.Location));
            this.newsStorageMock.Verify(storage => storage.AddNews(It.IsAny<NewsEntity>()), Times.Once());
        }

        [TestMethod]
        public void TestPostNewsReturns400BadRequestWhenNewsParametersValidationFails()
        {
            this.inputValidation.Setup(validation => validation.ValidateNewsParameters(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Throws(new InputValidationException(HttpStatusCode.BadRequest, string.Empty));

            this.PostNewsFails(HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void TestPostNewsReturns400BadRequestWhenBusinessValidationFails()
        {
            this.businessValidation.Setup(validation => validation.IsValidCreateNews(this.newsStorageMock.Object, It.IsAny<News>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>())).Returns(Task.FromResult(false));

            this.PostNewsFails(HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void TestPatchNewsReturns200OkWhenUpdatesUser()
        {
            Guid guid = Guid.NewGuid();

            this.inputValidation.Setup(validation => validation.IsValidNews(It.IsAny<News>())).Returns(true);
            this.businessValidation.Setup(validation => validation.IsValidUpdateNews(this.newsStorageMock.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>(), It.IsAny<string>())).Returns(Task.FromResult(true));

            var newsController = new NewsController(this.newsStorageMock.Object, this.inputValidation.Object, this.businessValidation.Object);
            newsController.ConfigureForTesting(new HttpMethod("PATCH"), string.Format("{0}/{1}/{2}/{3}", NewsUriRoot, City, Date, guid));
            newsController.User = new GenericPrincipal(new GenericIdentity(User.Nickname), null);

            HttpResponseMessage response = newsController.Patch(City, Date, guid.ToString(), News).Result;

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            this.newsStorageMock.Verify(storage => storage.UpdateNews(It.IsAny<NewsBll>()), Times.Once());
        }

        [TestMethod]
        public void TestPatchNewsReturns400BadRequestWhenNewsParametersValidationFails()
        {
            Guid guid = Guid.NewGuid();

            this.inputValidation.Setup(validation => validation.ValidateNewsParameters(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Throws(new InputValidationException(HttpStatusCode.BadRequest, string.Empty));

            this.PatchNewsFails(HttpStatusCode.BadRequest, guid);
        }

        [TestMethod]
        public void TestPatchNewsReturns400BadRequestWhenNewsValidationFails()
        {
            var guid = Guid.NewGuid();

            this.inputValidation.Setup(validation => validation.IsValidNews(It.IsAny<News>())).Returns(false);
            this.businessValidation.Setup(validation => validation.IsValidUpdateNews(this.newsStorageMock.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>(), It.IsAny<string>())).Returns(Task.FromResult(true));

            this.PatchNewsFails(HttpStatusCode.BadRequest, guid);
        }

        [TestMethod]
        public void TestPatchNewsReturns404NotFoundWhenBusinessValidationFails()
        {
            Guid guid = Guid.NewGuid();

            this.inputValidation.Setup(validation => validation.IsValidNews(It.IsAny<News>())).Returns(true);
            this.businessValidation.Setup(validation => validation.IsValidUpdateNews(this.newsStorageMock.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>(), It.IsAny<string>())).Returns(Task.FromResult(false));

            this.PatchNewsFails(HttpStatusCode.NotFound, guid);
        }

        [TestMethod]
        public void TestDeleteNewsReturns204NoContentWhenDeletesUser()
        {
            Guid guid = Guid.NewGuid();

            this.businessValidation.Setup(validation => validation.IsValidDeleteNews(this.newsStorageMock.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>(), It.IsAny<string>())).Returns(Task.FromResult(true));

            var newsController = new NewsController(this.newsStorageMock.Object, this.inputValidation.Object, this.businessValidation.Object);
            newsController.ConfigureForTesting(HttpMethod.Delete, string.Format("{0}/{1}/{2}/{3}", NewsUriRoot, City, Date, guid));
            newsController.User = new GenericPrincipal(new GenericIdentity(User.Nickname), null); 

            HttpResponseMessage response = newsController.Delete(City, Date, guid.ToString()).Result;

            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
            this.newsStorageMock.Verify(storage => storage.DeleteNews(City, DateTime.Parse(Date), guid), Times.Once());
        }

        [TestMethod]
        public void TestDeleteNewsReturns400BadRequestWhenNewsParametersValidationFails()
        {
            Guid guid = Guid.NewGuid();

            this.inputValidation.Setup(validation => validation.ValidateNewsParameters(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Throws(new InputValidationException(HttpStatusCode.BadRequest, string.Empty));

            this.DeleteNewsFails(HttpStatusCode.BadRequest, guid);
        }

        [TestMethod]
        public void TestDeleteNewsReturns404NotFoundWhenBusinessValidationFails()
        {
            Guid guid = Guid.NewGuid();

            this.businessValidation.Setup(validation => validation.IsValidDeleteNews(this.newsStorageMock.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>(), It.IsAny<string>())).Returns(Task.FromResult(false));

            this.DeleteNewsFails(HttpStatusCode.NotFound, guid);
        }

        #region Test utils

        private void GetNewsFails(HttpStatusCode code, Guid guid)
        {
            var newsController = new NewsController(this.newsStorageMock.Object, this.inputValidation.Object, this.businessValidation.Object);
            newsController.ConfigureForTesting(HttpMethod.Get, string.Format("{0}/{1}/{2}/{3}", NewsUriRoot, City, Date, guid));

            var response = newsController.Get(City, Date, guid.ToString()).Result;

            Assert.AreEqual(code, response.StatusCode);
        }

        private void PostNewsFails(HttpStatusCode code)
        {
            var newsController = new NewsController(this.newsStorageMock.Object, this.inputValidation.Object, this.businessValidation.Object);
            newsController.ConfigureForTesting(HttpMethod.Post, string.Format("{0}/{1}/{2}", NewsUriRoot, City, Date));
            newsController.User = new GenericPrincipal(new GenericIdentity(User.Nickname), null);

            HttpResponseMessage response = newsController.Post(City, Date, News).Result;

            Assert.AreEqual(code, response.StatusCode);
            this.newsStorageMock.Verify(storage => storage.AddNews(It.IsAny<NewsEntity>()), Times.Never());
        }

        private void PatchNewsFails(HttpStatusCode code, Guid guid)
        {
            var newsController = new NewsController(this.newsStorageMock.Object, this.inputValidation.Object, this.businessValidation.Object);
            newsController.ConfigureForTesting(new HttpMethod("PATCH"), string.Format("{0}/{1}/{2}/{3}", NewsUriRoot, City, Date, guid));
            newsController.User = new GenericPrincipal(new GenericIdentity(User.Nickname), null);

            HttpResponseMessage response = newsController.Patch(City, Date, guid.ToString(), News).Result;

            Assert.AreEqual(code, response.StatusCode);
            this.newsStorageMock.Verify(storage => storage.UpdateNews(News.ToNewsBll(guid, News, City, User.Nickname, DateTime.Parse(Date))), Times.Never());
        }

        private void DeleteNewsFails(HttpStatusCode code, Guid guid)
        {
            var newsController = new NewsController(this.newsStorageMock.Object, this.inputValidation.Object, this.businessValidation.Object);
            newsController.ConfigureForTesting(HttpMethod.Delete, string.Format("{0}/{1}/{2}/{3}", NewsUriRoot, City, Date, guid));
            newsController.User = new GenericPrincipal(new GenericIdentity(User.Nickname), null);

            HttpResponseMessage response = newsController.Delete(City, Date, guid.ToString()).Result;

            Assert.AreEqual(code, response.StatusCode);
            this.newsStorageMock.Verify(storage => storage.DeleteNews(City, DateTime.Parse(Date), guid), Times.Never());
        }

        #endregion
    }
}
