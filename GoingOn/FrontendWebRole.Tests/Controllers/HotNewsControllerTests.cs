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
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http.Routing;
    using GoingOn.Common;
    using GoingOn.Frontend.Entities;
    using GoingOn.Frontend.Validation;
    using GoingOn.FrontendWebRole.Controllers;
    using GoingOn.Model.EntitiesBll;
    using GoingOn.Storage;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Newtonsoft.Json;
    using WebApiContrib.Testing;

    [TestClass]
    public class HotNewsControllerTests
    {
        private Mock<IHotNewsStorage> hotNewsStorageMock;
        private Mock<IApiInputValidationChecks> inputValidation;
        private Mock<IApiBusinessLogicValidationChecks> businessValidation;

        private const string City = "Malaga";
        private const string Date = "2015-05-11";

        private const string Scheme = "http";
        private const string Host = "test.com";
        private const int Port = 123;

        private static readonly NewsBll News = new NewsBll
        {
            Id = Guid.NewGuid(),
            Title = "title",
            Content = "content",
            Author = "author",
            City = "Malaga",
            Date = DateTime.Parse("2015-05-11")
        };

        [TestInitialize]
        public void Initizalize()
        {
            this.hotNewsStorageMock = new Mock<IHotNewsStorage>();
            this.inputValidation = new Mock<IApiInputValidationChecks>();
            this.businessValidation = new Mock<IApiBusinessLogicValidationChecks>();
        }

        [TestMethod]
        public void TestGetHotNewsReturns200Ok_WhenThereAreHotNews()
        {
            this.inputValidation.Setup(validation => validation.IsValidCity(It.IsAny<string>())).Returns(true);
            this.businessValidation.Setup(validation => validation.IsValidGetHotNews(this.hotNewsStorageMock.Object, It.IsAny<string>(), It.IsAny<DateTime>())).Returns(Task.FromResult(true));
            this.hotNewsStorageMock.Setup(storage => storage.GetNews(It.IsAny<string>(), It.IsAny<DateTime>())).Returns(Task.FromResult(this.GenerateNews()));

            var hotNewsController = new HotNewsController(this.hotNewsStorageMock.Object, this.inputValidation.Object, this.businessValidation.Object);

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, GOUriBuilder.BuildAbsoluteDiaryEntryUri(Scheme, Host, Port, City, Date));
            request.Headers.Referrer = new Uri(GOUriBuilder.BuildAbsoluteDiaryEntryUri(Scheme, Host, Port, City, Date));

            hotNewsController.ConfigureForTesting(request, "GetHotNews", new HttpRoute(GOUriBuilder.GetHotNewsTemplate));

            HttpResponseMessage response = hotNewsController.Get(City).Result;

            HttpContent content = response.Content;
            string jsonContent = content.ReadAsStringAsync().Result;
            var actualHotNews = JsonConvert.DeserializeObject<List<NewsREST>>(jsonContent);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(1, actualHotNews.Count);
            Assert.IsTrue(actualHotNews.First().Links.Any(link => string.Equals("self", link.Rel)));
            Assert.AreEqual(new Uri(GOUriBuilder.BuildAbsoluteNewsUri(Scheme, Host, Port, City, Date, News.Id.ToString())), actualHotNews.First().Links.First(link => string.Equals("self", link.Rel)).Href);
            Assert.IsTrue(actualHotNews.First().Links.Any(link => string.Equals("author", link.Rel)));
            Assert.AreEqual(new Uri(GOUriBuilder.BuildAbsoluteUserUri(Scheme, Host, Port, News.Author)), actualHotNews.First().Links.First(link => string.Equals("author", link.Rel)).Href);
        }

        [TestMethod]
        public void TestGetHotNewsReturns400BadRequest_WhenInvalidCity()
        {
            this.inputValidation.Setup(validation => validation.IsValidCity(It.IsAny<string>())).Returns(false);
            this.businessValidation.Setup(validation => validation.IsValidGetHotNews(this.hotNewsStorageMock.Object, It.IsAny<string>(), It.IsAny<DateTime>())).Returns(Task.FromResult(true));
            this.hotNewsStorageMock.Setup(storage => storage.GetNews(It.IsAny<string>(), It.IsAny<DateTime>())).Returns(Task.FromResult(new List<NewsBll>().AsEnumerable()));

            this.AssertGetFails(url: GOUriBuilder.BuildAbsoluteDiaryEntryUri(Scheme, Host, Port, City, Date), city: City, resultCode: HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void TestGetHotNewsReturns404NotFound_WhenThereAreNotHotNews()
        {
            this.inputValidation.Setup(validation => validation.IsValidCity(It.IsAny<string>())).Returns(true);
            this.businessValidation.Setup(validation => validation.IsValidGetHotNews(this.hotNewsStorageMock.Object, It.IsAny<string>(), It.IsAny<DateTime>())).Returns(Task.FromResult(false));
            this.hotNewsStorageMock.Setup(storage => storage.GetNews(It.IsAny<string>(), It.IsAny<DateTime>())).Returns(Task.FromResult(new List<NewsBll>().AsEnumerable()));

            this.AssertGetFails(url: GOUriBuilder.BuildAbsoluteDiaryEntryUri(Scheme, Host, Port, City, Date), city: City, resultCode: HttpStatusCode.NotFound);
        }

        #region Assert helper methods

        private void AssertGetFails(string url, string city, HttpStatusCode resultCode)
        {
            HotNewsController hotNewsController = new HotNewsController(this.hotNewsStorageMock.Object, this.inputValidation.Object, this.businessValidation.Object);
            hotNewsController.ConfigureForTesting(HttpMethod.Get, url, "GetHotNews", new HttpRoute(GOUriBuilder.GetUserTemplate));

            HttpResponseMessage response = hotNewsController.Get(city).Result;

            Assert.AreEqual(resultCode, response.StatusCode);
            this.hotNewsStorageMock.Verify(storage => storage.GetNews(It.IsAny<string>(), It.IsAny<DateTime>()), Times.Never());
        }

        private IEnumerable<NewsBll> GenerateNews()
        {
            return new List<NewsBll>
            {
                News
            }.AsEnumerable();
        }

        #endregion
    }
}
