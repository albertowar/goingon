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
    using GoingOn.Repository;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Newtonsoft.Json;
    using WebApiContrib.Testing;

    [TestClass]
    public class NewsControllerTest
    {
        private Mock<INewsRepository> newsStorageMock;
        private Mock<IApiInputValidationChecks> inputValidation;
        private Mock<IApiBusinessLogicValidationChecks> businessValidation;

        private static readonly User DefaultUser = new User { Nickname = "nickname", Password = "password" };
        private static readonly News DefaultNews = new News { Title = "title", Content = "content" };

        private const string Scheme = "http";
        private const string Host = "test.com";
        private const int Port = 123;

        private static readonly string NewsUriRoot = string.Format("{0}://{1}:{2}", Scheme, Host, Port);

        private const string City = "Malaga";
        private const string Date = "2015-05-11";
        private static readonly Guid Id = Guid.NewGuid();

        [TestInitialize]
        public void Initizalize()
        {
            this.newsStorageMock = new Mock<INewsRepository>();
            this.inputValidation = new Mock<IApiInputValidationChecks>();
            this.businessValidation = new Mock<IApiBusinessLogicValidationChecks>();
        }

        [TestMethod]
        public void TestGetNewsReturns200OkWhenTheNewsIsInTheDatabase()
        {
            this.businessValidation.Setup(validation => validation.IsValidGetNews(this.newsStorageMock.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(true));
            this.newsStorageMock.Setup(storage => storage.GetNews(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(News.ToNewsBll(Id, DefaultNews, City, DefaultUser.Nickname, DateTime.Parse(Date))));

            var newsController = new NewsController(this.newsStorageMock.Object, this.inputValidation.Object, this.businessValidation.Object);

            var request = new HttpRequestMessage(HttpMethod.Get, GOUriBuilder.BuildAbsoluteNewsUri(Scheme, Host, Port, City, Date, Id.ToString()));
            request.Headers.Referrer = new Uri(GOUriBuilder.BuildAbsoluteNewsUri(Scheme, Host, Port, City, Date, Id.ToString()));

            newsController.ConfigureForTesting(request, "ListNews", new HttpRoute(GOUriBuilder.GetNewsTemplate));

            HttpResponseMessage response = newsController.Get(City, Date, Id.ToString()).Result;

            HttpContent content = response.Content;
            string jsonContent = content.ReadAsStringAsync().Result;
            var actualNews = JsonConvert.DeserializeObject<NewsREST>(jsonContent);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(new NewsCompleteEqualityComparer().Equals(DefaultNews, NewsREST.ToNews(actualNews)));
            Assert.IsTrue(actualNews.Links.Any(link => string.Equals("self", link.Rel)));
            Assert.AreEqual(new Uri(GOUriBuilder.BuildAbsoluteNewsUri(Scheme, Host, Port, City, Date, Id.ToString())), actualNews.Links.First(link => string.Equals("self", link.Rel)).Href);
            Assert.IsTrue(actualNews.Links.Any(link => string.Equals("author", link.Rel)));
            Assert.AreEqual(new Uri(GOUriBuilder.BuildAbsoluteUserUri(Scheme, Host, Port, NewsControllerTest.DefaultUser.Nickname)), actualNews.Links.First(link => string.Equals("author", link.Rel)).Href);
        }

        [TestMethod]
        public void TestGetNewsReturns400BadRequestWhenNewsParametersValidationFails()
        {
            this.inputValidation.Setup(validation => validation.ValidateNewsParameters(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Throws(new InputValidationException(HttpStatusCode.BadRequest, string.Empty));

            this.GetNewsFails(HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void TestGetNewsReturns404NotFoundWhenBusinessValidationFails()
        {
            this.businessValidation.Setup(validation => validation.IsValidGetNews(this.newsStorageMock.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(false));
            this.newsStorageMock.Setup(storage => storage.GetNews(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(News.ToNewsBll(Id, DefaultNews, City, DefaultUser.Nickname, DateTime.Parse(Date))));

            this.GetNewsFails(HttpStatusCode.NotFound);
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
            newsController.User = new GenericPrincipal(new GenericIdentity(DefaultUser.Nickname), null); 

            HttpResponseMessage response = newsController.Post(City, Date, DefaultNews).Result;

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            Assert.IsTrue(new Uri(NewsUriRoot).IsBaseOf(response.Headers.Location));
            this.newsStorageMock.Verify(storage => storage.AddNews(It.IsAny<NewsBll>()), Times.Once());
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
            this.inputValidation.Setup(validation => validation.IsValidNews(It.IsAny<News>())).Returns(true);
            this.businessValidation.Setup(validation => validation.IsValidGetNews(this.newsStorageMock.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(true));
            this.businessValidation.Setup(validation => validation.IsValidModifyNews(this.newsStorageMock.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>(), It.IsAny<string>())).Returns(Task.FromResult(true));

            var newsController = new NewsController(this.newsStorageMock.Object, this.inputValidation.Object, this.businessValidation.Object);
            newsController.ConfigureForTesting(new HttpMethod("PATCH"), string.Format("{0}/{1}/{2}/{3}", NewsUriRoot, City, Date, Id));
            newsController.User = new GenericPrincipal(new GenericIdentity(DefaultUser.Nickname), null);

            HttpResponseMessage response = newsController.Patch(City, Date, Id.ToString(), DefaultNews).Result;

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            this.newsStorageMock.Verify(storage => storage.UpdateNews(It.IsAny<NewsBll>()), Times.Once());
        }

        [TestMethod]
        public void TestPatchNewsReturns400BadRequestWhenNewsParametersValidationFails()
        {
            this.inputValidation.Setup(validation => validation.ValidateNewsParameters(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Throws(new InputValidationException(HttpStatusCode.BadRequest, string.Empty));

            this.PatchNewsFails(HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void TestPatchNewsReturns400BadRequestWhenNewsValidationFails()
        {
            this.inputValidation.Setup(validation => validation.IsValidNews(It.IsAny<News>())).Returns(false);
            this.businessValidation.Setup(validation => validation.IsValidModifyNews(this.newsStorageMock.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>(), It.IsAny<string>())).Returns(Task.FromResult(true));

            this.PatchNewsFails(HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void TestPatchNewsReturns404NotFoundWhenBusinessValidationFails()
        {
            this.inputValidation.Setup(validation => validation.IsValidNews(It.IsAny<News>())).Returns(true);
            this.businessValidation.Setup(validation => validation.IsValidGetNews(this.newsStorageMock.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(false));

            this.PatchNewsFails(HttpStatusCode.NotFound);
        }

        [TestMethod]
        public void TestPatchNewsReturns401_IfUserIsNotAuthorizedToUpdateNews()
        {
            this.inputValidation.Setup(validation => validation.IsValidNews(It.IsAny<News>())).Returns(true);
            this.businessValidation.Setup(validation => validation.IsValidGetNews(this.newsStorageMock.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(true));
            this.businessValidation.Setup(validation => validation.IsValidModifyNews(this.newsStorageMock.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>(), It.IsAny<string>())).Returns(Task.FromResult(false));

            this.PatchNewsFails(HttpStatusCode.Unauthorized);
        }

        [TestMethod]
        public void TestDeleteNewsReturns204NoContentWhenDeletesUser()
        {
            this.businessValidation.Setup(validation => validation.IsValidGetNews(this.newsStorageMock.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(true));
            this.businessValidation.Setup(validation => validation.IsValidModifyNews(this.newsStorageMock.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>(), It.IsAny<string>())).Returns(Task.FromResult(true));

            var newsController = new NewsController(this.newsStorageMock.Object, this.inputValidation.Object, this.businessValidation.Object);
            newsController.ConfigureForTesting(HttpMethod.Delete, string.Format("{0}/{1}/{2}/{3}", NewsUriRoot, City, Date, Id));
            newsController.User = new GenericPrincipal(new GenericIdentity(DefaultUser.Nickname), null); 

            HttpResponseMessage response = newsController.Delete(City, Date, Id.ToString()).Result;

            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
            this.newsStorageMock.Verify(storage => storage.DeleteNews(City, DateTime.Parse(Date), Id), Times.Once());
        }

        [TestMethod]
        public void TestDeleteNewsReturns400BadRequestWhenNewsParametersValidationFails()
        {
            this.inputValidation.Setup(validation => validation.ValidateNewsParameters(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Throws(new InputValidationException(HttpStatusCode.BadRequest, string.Empty));

            this.DeleteNewsFails(HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void TestDeleteNewsReturns404_IfNewsIsNotInDatabase()
        {
            this.businessValidation.Setup(validation => validation.IsValidGetNews(this.newsStorageMock.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(false));

            this.DeleteNewsFails(HttpStatusCode.NotFound);
        }

        [TestMethod]
        public void TestDeleteNewsReturns401_IfUserIsNotAuthorized()
        {
            this.businessValidation.Setup(validation => validation.IsValidGetNews(this.newsStorageMock.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(true));
            this.businessValidation.Setup(validation => validation.IsValidModifyNews(this.newsStorageMock.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>(), It.IsAny<string>())).Returns(Task.FromResult(false));

            this.DeleteNewsFails(HttpStatusCode.Unauthorized);
        }

        #region Test utils

        private void GetNewsFails(HttpStatusCode code)
        {
            var newsController = new NewsController(this.newsStorageMock.Object, this.inputValidation.Object, this.businessValidation.Object);
            newsController.ConfigureForTesting(HttpMethod.Get, string.Format("{0}/{1}/{2}/{3}", NewsUriRoot, City, Date, Id));

            HttpResponseMessage response = newsController.Get(City, Date, Id.ToString()).Result;

            Assert.AreEqual(code, response.StatusCode);
        }

        private void PostNewsFails(HttpStatusCode code)
        {
            var newsController = new NewsController(this.newsStorageMock.Object, this.inputValidation.Object, this.businessValidation.Object);
            newsController.ConfigureForTesting(HttpMethod.Post, string.Format("{0}/{1}/{2}", NewsUriRoot, City, Date));
            newsController.User = new GenericPrincipal(new GenericIdentity(DefaultUser.Nickname), null);

            HttpResponseMessage response = newsController.Post(City, Date, DefaultNews).Result;

            Assert.AreEqual(code, response.StatusCode);
            this.newsStorageMock.Verify(storage => storage.AddNews(It.IsAny<NewsBll>()), Times.Never());
        }

        private void PatchNewsFails(HttpStatusCode code)
        {
            var newsController = new NewsController(this.newsStorageMock.Object, this.inputValidation.Object, this.businessValidation.Object);
            newsController.ConfigureForTesting(new HttpMethod("PATCH"), string.Format("{0}/{1}/{2}/{3}", NewsUriRoot, City, Date, Id));
            newsController.User = new GenericPrincipal(new GenericIdentity(DefaultUser.Nickname), null);

            HttpResponseMessage response = newsController.Patch(City, Date, Id.ToString(), DefaultNews).Result;

            Assert.AreEqual(code, response.StatusCode);
            this.newsStorageMock.Verify(storage => storage.UpdateNews(News.ToNewsBll(Id, DefaultNews, City, DefaultUser.Nickname, DateTime.Parse(Date))), Times.Never());
        }

        private void DeleteNewsFails(HttpStatusCode code)
        {
            var newsController = new NewsController(this.newsStorageMock.Object, this.inputValidation.Object, this.businessValidation.Object);
            newsController.ConfigureForTesting(HttpMethod.Delete, string.Format("{0}/{1}/{2}/{3}", NewsUriRoot, City, Date, Id));
            newsController.User = new GenericPrincipal(new GenericIdentity(DefaultUser.Nickname), null);

            HttpResponseMessage response = newsController.Delete(City, Date, Id.ToString()).Result;

            Assert.AreEqual(code, response.StatusCode);
            this.newsStorageMock.Verify(storage => storage.DeleteNews(City, DateTime.Parse(Date), Id), Times.Never());
        }

        #endregion
    }
}
