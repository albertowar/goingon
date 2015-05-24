// ****************************************************************************
// <copyright file="NewsImageControllerTests.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// NewsImageController tests class
// </summary>
// ****************************************************************************

namespace GoingOn.FrontendWebRole.Tests.Controllers
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using System.Web.Http.Routing;
    using GoingOn.Common;
    using GoingOn.Frontend.Common;
    using GoingOn.Frontend.Validation;
    using GoingOn.FrontendWebRole.Controllers;
    using GoingOn.Repository;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using WebApiContrib.Testing;

    [TestClass]
    public class NewsImageControllerTests
    {
        private const string Scheme = "http";
        private const string Host = "test.com";
        private const int Port = 123;

        private const string City = "Malaga";
        private const string Date = "2015-05-11";

        private Mock<INewsRepository> mockNewsRepository;
        private Mock<IImageRepository> mockImageRepository;
        private Mock<IApiInputValidationChecks> inputValidation;
        private Mock<IApiBusinessLogicValidationChecks> businessValidation;
        
        [TestInitialize]
        public void Initizalize()
        {
            this.mockNewsRepository = new Mock<INewsRepository>();
            this.mockImageRepository = new Mock<IImageRepository>();
            this.inputValidation = new Mock<IApiInputValidationChecks>();
            this.businessValidation = new Mock<IApiBusinessLogicValidationChecks>();
        }

        [TestMethod]
        public void TestGetImageReturns200_WhenTheThumbnailIsInTheDatabase()
        {
            Guid guid = Guid.NewGuid();

            this.businessValidation.Setup(validation => validation.IsValidGetNews(this.mockNewsRepository.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(true));
            this.businessValidation.Setup(validation => validation.IsValidGetImageNews(this.mockImageRepository.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(true));

            this.mockImageRepository.Setup(storage => storage.GetNewsImage(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(Image.FromFile("goten.png")));

            var newsImageController = new NewsImageController(this.mockNewsRepository.Object, this.mockImageRepository.Object, this.inputValidation.Object, this.businessValidation.Object);

            var request = new HttpRequestMessage(HttpMethod.Get, GOUriBuilder.BuildAbsoluteNewsImageUri(Scheme, Host, Port, City, Date, guid.ToString()));
            request.Headers.Referrer = new Uri(GOUriBuilder.BuildAbsoluteNewsImageUri(Scheme, Host, Port, City, Date, guid.ToString()));

            newsImageController.ConfigureForTesting(request, "GetImage", new HttpRoute(GOUriBuilder.NewsImageTemplate));

            HttpResponseMessage response = newsImageController.Get(City, Date, guid.ToString()).Result;

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(new MediaTypeHeaderValue("image/png"), response.Content.Headers.ContentType);
        }

        [TestMethod]
        public void TestGetImageReturns400_WhenInputValidationFails()
        {
            Guid guid = Guid.NewGuid();

            this.inputValidation.Setup(validation => validation.ValidateNewsParameters(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Throws(new InputValidationException(HttpStatusCode.BadRequest, string.Empty));

            var newsImageController = new NewsImageController(this.mockNewsRepository.Object, this.mockImageRepository.Object, this.inputValidation.Object, this.businessValidation.Object);

            var request = new HttpRequestMessage(HttpMethod.Get, GOUriBuilder.BuildAbsoluteNewsImageUri(Scheme, Host, Port, City, Date, guid.ToString()));
            request.Headers.Referrer = new Uri(GOUriBuilder.BuildAbsoluteNewsImageUri(Scheme, Host, Port, City, Date, guid.ToString()));

            newsImageController.ConfigureForTesting(request, "GetImage", new HttpRoute(GOUriBuilder.NewsImageTemplate));

            HttpResponseMessage response = newsImageController.Get(City, Date, guid.ToString()).Result;

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public void TestGetImageReturns404_WhenTheNewsIsNotTheDatabase()
        {
            Guid guid = Guid.NewGuid();

            this.businessValidation.Setup(validation => validation.IsValidGetNews(this.mockNewsRepository.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(false));
            this.businessValidation.Setup(validation => validation.IsValidGetImageNews(this.mockImageRepository.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(true));

            this.mockImageRepository.Setup(storage => storage.GetNewsThumbnailImage(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(Image.FromFile("goten.png")));

            var newsImageController = new NewsImageController(this.mockNewsRepository.Object, this.mockImageRepository.Object, this.inputValidation.Object, this.businessValidation.Object);

            var request = new HttpRequestMessage(HttpMethod.Get, GOUriBuilder.BuildAbsoluteNewsImageUri(Scheme, Host, Port, City, Date, guid.ToString()));
            request.Headers.Referrer = new Uri(GOUriBuilder.BuildAbsoluteNewsImageUri(Scheme, Host, Port, City, Date, guid.ToString()));

            newsImageController.ConfigureForTesting(request, "GetImage", new HttpRoute(GOUriBuilder.NewsImageTemplate));

            HttpResponseMessage response = newsImageController.Get(City, Date, guid.ToString()).Result;

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public void TestGetImageReturns404_WhenTheNewsImageIsNotTheDatabase()
        {
            Guid guid = Guid.NewGuid();

            this.businessValidation.Setup(validation => validation.IsValidGetNews(this.mockNewsRepository.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(true));
            this.businessValidation.Setup(validation => validation.IsValidGetImageNews(this.mockImageRepository.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(false));

            this.mockImageRepository.Setup(storage => storage.GetNewsThumbnailImage(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(Image.FromFile("goten.png")));

            var newsImageController = new NewsImageController(this.mockNewsRepository.Object, this.mockImageRepository.Object, this.inputValidation.Object, this.businessValidation.Object);

            var request = new HttpRequestMessage(HttpMethod.Get, GOUriBuilder.BuildAbsoluteNewsImageUri(Scheme, Host, Port, City, Date, guid.ToString()));
            request.Headers.Referrer = new Uri(GOUriBuilder.BuildAbsoluteNewsImageUri(Scheme, Host, Port, City, Date, guid.ToString()));

            newsImageController.ConfigureForTesting(request, "GetImage", new HttpRoute(GOUriBuilder.NewsImageTemplate));

            HttpResponseMessage response = newsImageController.Get(City, Date, guid.ToString()).Result;

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public void TestPostImageReturns200_WhenTheImageIsInTheDatabase()
        {
            Guid guid = Guid.NewGuid();

            this.businessValidation.Setup(validation => validation.IsValidGetNews(this.mockNewsRepository.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(true));
            this.businessValidation.Setup(validation => validation.IsValidGetImageNews(this.mockImageRepository.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(false));

            this.mockImageRepository.Setup(storage => storage.GetNewsImage(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(Image.FromFile("goten.png")));

            var newsImageController = new NewsImageController(this.mockNewsRepository.Object, this.mockImageRepository.Object, this.inputValidation.Object, this.businessValidation.Object);

            Image image = Image.FromFile("goten.png");
            MemoryStream stream = new MemoryStream();
            image.Save(stream, image.RawFormat);
            stream.Position = 0;

            var request = new HttpRequestMessage(HttpMethod.Post, GOUriBuilder.BuildAbsoluteNewsImageUri(Scheme, Host, Port, City, Date, guid.ToString()));
            request.Headers.Referrer = new Uri(GOUriBuilder.BuildAbsoluteNewsImageUri(Scheme, Host, Port, City, Date, guid.ToString()));
            request.Content = new ByteArrayContent(stream.GetBuffer());

            newsImageController.ConfigureForTesting(request, "PostImage", new HttpRoute(GOUriBuilder.NewsImageTemplate));

            HttpResponseMessage response = newsImageController.Post(City, Date, guid.ToString()).Result;

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public void TestPostImageReturns400_IfTheImageFormatIsWrong()
        {
            Guid guid = Guid.NewGuid();

            this.inputValidation.Setup(validation => validation.ValidateImage(It.IsAny<byte[]>(), It.IsAny<MediaTypeHeaderValue>())).Throws(new InputValidationException(HttpStatusCode.BadRequest, string.Empty));
            this.businessValidation.Setup(validation => validation.IsValidGetNews(this.mockNewsRepository.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(true));
            this.businessValidation.Setup(validation => validation.IsValidGetImageNews(this.mockImageRepository.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(false));

            this.mockImageRepository.Setup(storage => storage.GetNewsImage(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(Image.FromFile("goten.png")));

            var newsImageController = new NewsImageController(this.mockNewsRepository.Object, this.mockImageRepository.Object, this.inputValidation.Object, this.businessValidation.Object);

            var request = new HttpRequestMessage(HttpMethod.Post, GOUriBuilder.BuildAbsoluteNewsImageUri(Scheme, Host, Port, City, Date, guid.ToString()));
            request.Headers.Referrer = new Uri(GOUriBuilder.BuildAbsoluteNewsImageUri(Scheme, Host, Port, City, Date, guid.ToString()));

            newsImageController.ConfigureForTesting(request, "PostImage", new HttpRoute(GOUriBuilder.NewsImageTemplate));

            HttpResponseMessage response = newsImageController.Post(City, Date, guid.ToString()).Result;

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public void TestPostImageReturns400_WhenNewsInputValidationFails()
        {
            Guid guid = Guid.NewGuid();

            this.inputValidation.Setup(validation => validation.ValidateNewsParameters(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Throws(new InputValidationException(HttpStatusCode.BadRequest, string.Empty));

            var newsImageController = new NewsImageController(this.mockNewsRepository.Object, this.mockImageRepository.Object, this.inputValidation.Object, this.businessValidation.Object);

            var request = new HttpRequestMessage(HttpMethod.Post, GOUriBuilder.BuildAbsoluteNewsImageUri(Scheme, Host, Port, City, Date, guid.ToString()));
            request.Headers.Referrer = new Uri(GOUriBuilder.BuildAbsoluteNewsImageUri(Scheme, Host, Port, City, Date, guid.ToString()));

            newsImageController.ConfigureForTesting(request, "PostImage", new HttpRoute(GOUriBuilder.NewsImageTemplate));

            HttpResponseMessage response = newsImageController.Post(City, Date, guid.ToString()).Result;

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public void TestPostImageReturns404_WhenTheNewsIsNotTheDatabase()
        {
            Guid guid = Guid.NewGuid();

            this.businessValidation.Setup(validation => validation.IsValidGetNews(this.mockNewsRepository.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(false));
            this.businessValidation.Setup(validation => validation.IsValidGetImageNews(this.mockImageRepository.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(true));

            this.mockImageRepository.Setup(storage => storage.GetNewsThumbnailImage(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(Image.FromFile("goten.png")));

            var newsImageController = new NewsImageController(this.mockNewsRepository.Object, this.mockImageRepository.Object, this.inputValidation.Object, this.businessValidation.Object);

            var request = new HttpRequestMessage(HttpMethod.Post, GOUriBuilder.BuildAbsoluteNewsImageUri(Scheme, Host, Port, City, Date, guid.ToString()));
            request.Headers.Referrer = new Uri(GOUriBuilder.BuildAbsoluteNewsImageUri(Scheme, Host, Port, City, Date, guid.ToString()));

            newsImageController.ConfigureForTesting(request, "PostImage", new HttpRoute(GOUriBuilder.NewsImageTemplate));

            HttpResponseMessage response = newsImageController.Post(City, Date, guid.ToString()).Result;

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public void TestPostImageReturns400_WhenTheNewsImageIsInTheDatabase()
        {
            Guid guid = Guid.NewGuid();

            this.businessValidation.Setup(validation => validation.IsValidGetNews(this.mockNewsRepository.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(true));
            this.businessValidation.Setup(validation => validation.IsValidGetImageNews(this.mockImageRepository.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(true));

            this.mockImageRepository.Setup(storage => storage.GetNewsThumbnailImage(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(Image.FromFile("goten.png")));

            var newsImageController = new NewsImageController(this.mockNewsRepository.Object, this.mockImageRepository.Object, this.inputValidation.Object, this.businessValidation.Object);

            var request = new HttpRequestMessage(HttpMethod.Post, GOUriBuilder.BuildAbsoluteNewsImageUri(Scheme, Host, Port, City, Date, guid.ToString()));
            request.Headers.Referrer = new Uri(GOUriBuilder.BuildAbsoluteNewsImageUri(Scheme, Host, Port, City, Date, guid.ToString()));

            newsImageController.ConfigureForTesting(request, "PostImage", new HttpRoute(GOUriBuilder.NewsImageTemplate));

            HttpResponseMessage response = newsImageController.Post(City, Date, guid.ToString()).Result;

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public void TestDeleteImageReturns200_WhenTheThumbnailIsInTheDatabase()
        {
            Guid guid = Guid.NewGuid();

            this.businessValidation.Setup(validation => validation.IsValidGetNews(this.mockNewsRepository.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(true));
            this.businessValidation.Setup(validation => validation.IsValidGetImageNews(this.mockImageRepository.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(true));

            this.mockImageRepository.Setup(storage => storage.GetNewsImage(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(Image.FromFile("goten.png")));

            var newsImageController = new NewsImageController(this.mockNewsRepository.Object, this.mockImageRepository.Object, this.inputValidation.Object, this.businessValidation.Object);

            var request = new HttpRequestMessage(HttpMethod.Delete, GOUriBuilder.BuildAbsoluteNewsImageUri(Scheme, Host, Port, City, Date, guid.ToString()));
            request.Headers.Referrer = new Uri(GOUriBuilder.BuildAbsoluteNewsImageUri(Scheme, Host, Port, City, Date, guid.ToString()));

            newsImageController.ConfigureForTesting(request, "DeleteImage", new HttpRoute(GOUriBuilder.NewsImageTemplate));

            HttpResponseMessage response = newsImageController.Delete(City, Date, guid.ToString()).Result;

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public void TestDeleteImageReturns400_WhenInputValidationFails()
        {
            Guid guid = Guid.NewGuid();

            this.inputValidation.Setup(validation => validation.ValidateNewsParameters(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Throws(new InputValidationException(HttpStatusCode.BadRequest, string.Empty));

            var newsImageController = new NewsImageController(this.mockNewsRepository.Object, this.mockImageRepository.Object, this.inputValidation.Object, this.businessValidation.Object);

            var request = new HttpRequestMessage(HttpMethod.Delete, GOUriBuilder.BuildAbsoluteNewsImageUri(Scheme, Host, Port, City, Date, guid.ToString()));
            request.Headers.Referrer = new Uri(GOUriBuilder.BuildAbsoluteNewsImageUri(Scheme, Host, Port, City, Date, guid.ToString()));

            newsImageController.ConfigureForTesting(request, "DeleteImage", new HttpRoute(GOUriBuilder.NewsImageTemplate));

            HttpResponseMessage response = newsImageController.Delete(City, Date, guid.ToString()).Result;

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public void TestDeleteImageReturns404_WhenTheNewsIsNotTheDatabase()
        {
            Guid guid = Guid.NewGuid();

            this.businessValidation.Setup(validation => validation.IsValidGetNews(this.mockNewsRepository.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(false));
            this.businessValidation.Setup(validation => validation.IsValidGetImageNews(this.mockImageRepository.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(true));

            this.mockImageRepository.Setup(storage => storage.GetNewsThumbnailImage(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(Image.FromFile("goten.png")));

            var newsImageController = new NewsImageController(this.mockNewsRepository.Object, this.mockImageRepository.Object, this.inputValidation.Object, this.businessValidation.Object);

            var request = new HttpRequestMessage(HttpMethod.Delete, GOUriBuilder.BuildAbsoluteNewsImageUri(Scheme, Host, Port, City, Date, guid.ToString()));
            request.Headers.Referrer = new Uri(GOUriBuilder.BuildAbsoluteNewsImageUri(Scheme, Host, Port, City, Date, guid.ToString()));

            newsImageController.ConfigureForTesting(request, "DeleteImage", new HttpRoute(GOUriBuilder.NewsImageTemplate));

            HttpResponseMessage response = newsImageController.Delete(City, Date, guid.ToString()).Result;

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public void TestDeleteImageReturns404_WhenTheNewsImageIsNotTheDatabase()
        {
            Guid guid = Guid.NewGuid();

            this.businessValidation.Setup(validation => validation.IsValidGetNews(this.mockNewsRepository.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(true));
            this.businessValidation.Setup(validation => validation.IsValidGetImageNews(this.mockImageRepository.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(false));

            this.mockImageRepository.Setup(storage => storage.GetNewsThumbnailImage(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(Image.FromFile("goten.png")));

            var newsImageController = new NewsImageController(this.mockNewsRepository.Object, this.mockImageRepository.Object, this.inputValidation.Object, this.businessValidation.Object);

            var request = new HttpRequestMessage(HttpMethod.Delete, GOUriBuilder.BuildAbsoluteNewsImageUri(Scheme, Host, Port, City, Date, guid.ToString()));
            request.Headers.Referrer = new Uri(GOUriBuilder.BuildAbsoluteNewsImageUri(Scheme, Host, Port, City, Date, guid.ToString()));

            newsImageController.ConfigureForTesting(request, "DeleteImage", new HttpRoute(GOUriBuilder.NewsImageTemplate));

            HttpResponseMessage response = newsImageController.Delete(City, Date, guid.ToString()).Result;

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
