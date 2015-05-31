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
    using System.Security.Principal;
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

        private const string DefaultNickname = "nickname";
        private const string City = "Malaga";
        private const string Date = "2015-05-11";
        private static readonly Guid Id = Guid.NewGuid();

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
            this.inputValidation.Setup(validation => validation.ValidateNewsParameters(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Throws(new InputValidationException(HttpStatusCode.BadRequest, string.Empty));

            this.AssertGetFails(HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void TestGetImageReturns404_WhenTheNewsIsNotTheDatabase()
        {
            this.businessValidation.Setup(validation => validation.IsValidGetNews(this.mockNewsRepository.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(false));
            this.businessValidation.Setup(validation => validation.IsValidGetImageNews(this.mockImageRepository.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(true));

            this.mockImageRepository.Setup(storage => storage.GetNewsThumbnailImage(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(Image.FromFile("goten.png")));

            this.AssertGetFails(HttpStatusCode.NotFound);
        }

        [TestMethod]
        public void TestGetImageReturns404_WhenTheNewsImageIsNotTheDatabase()
        {
            this.businessValidation.Setup(validation => validation.IsValidGetNews(this.mockNewsRepository.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(true));
            this.businessValidation.Setup(validation => validation.IsValidGetImageNews(this.mockImageRepository.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(false));

            this.mockImageRepository.Setup(storage => storage.GetNewsThumbnailImage(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(Image.FromFile("goten.png")));

            this.AssertGetFails(HttpStatusCode.NotFound);
        }

        [TestMethod]
        public void TestPostImageReturns200_WhenTheImageIsInTheDatabase()
        {
            Guid guid = Guid.NewGuid();

            this.businessValidation.Setup(validation => validation.IsValidGetNews(this.mockNewsRepository.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(true));
            this.businessValidation.Setup(validation => validation.IsValidModifyNews(this.mockNewsRepository.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>(), It.IsAny<string>())).Returns(Task.FromResult(true));
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
            newsImageController.User = new GenericPrincipal(new GenericIdentity(DefaultNickname), null);

            HttpResponseMessage response = newsImageController.Post(City, Date, guid.ToString()).Result;

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public void TestPostImageReturns400_IfTheImageFormatIsWrong()
        {
            this.inputValidation.Setup(validation => validation.ValidateImage(It.IsAny<byte[]>(), It.IsAny<MediaTypeHeaderValue>())).Throws(new InputValidationException(HttpStatusCode.BadRequest, string.Empty));
            this.businessValidation.Setup(validation => validation.IsValidGetNews(this.mockNewsRepository.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(true));
            this.businessValidation.Setup(validation => validation.IsValidGetImageNews(this.mockImageRepository.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(false));

            this.mockImageRepository.Setup(storage => storage.GetNewsImage(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(Image.FromFile("goten.png")));

            this.AssertPostFails(HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void TestPostImageReturns400_WhenNewsInputValidationFails()
        {
            this.inputValidation.Setup(validation => validation.ValidateNewsParameters(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Throws(new InputValidationException(HttpStatusCode.BadRequest, string.Empty));

            this.AssertPostFails(HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void TestPostImageReturns404_WhenTheNewsIsNotTheDatabase()
        {
            this.businessValidation.Setup(validation => validation.IsValidGetNews(this.mockNewsRepository.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(false));
            this.businessValidation.Setup(validation => validation.IsValidModifyNews(this.mockNewsRepository.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>(), It.IsAny<string>())).Returns(Task.FromResult(true));
            this.businessValidation.Setup(validation => validation.IsValidGetImageNews(this.mockImageRepository.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(true));

            this.mockImageRepository.Setup(storage => storage.GetNewsThumbnailImage(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(Image.FromFile("goten.png")));

            this.AssertPostFails(HttpStatusCode.NotFound);
        }

        [TestMethod]
        public void TestPostImageReturns401_WhenTheUserIsNotTheAuthor()
        {
            this.businessValidation.Setup(validation => validation.IsValidGetNews(this.mockNewsRepository.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(true));
            this.businessValidation.Setup(validation => validation.IsValidModifyNews(this.mockNewsRepository.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>(), It.IsAny<string>())).Returns(Task.FromResult(false));
            this.businessValidation.Setup(validation => validation.IsValidGetImageNews(this.mockImageRepository.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(true));

            this.mockImageRepository.Setup(storage => storage.GetNewsThumbnailImage(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(Image.FromFile("goten.png")));

            this.AssertPostFails(HttpStatusCode.Unauthorized);
        }

        [TestMethod]
        public void TestPostImageReturns400_WhenTheNewsImageIsInTheDatabase()
        {
            this.businessValidation.Setup(validation => validation.IsValidGetNews(this.mockNewsRepository.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(true));
            this.businessValidation.Setup(validation => validation.IsValidModifyNews(this.mockNewsRepository.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>(), It.IsAny<string>())).Returns(Task.FromResult(true));
            this.businessValidation.Setup(validation => validation.IsValidGetImageNews(this.mockImageRepository.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(true));

            this.mockImageRepository.Setup(storage => storage.GetNewsThumbnailImage(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(Image.FromFile("goten.png")));

            this.AssertPostFails(HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void TestDeleteImageReturns200_WhenTheThumbnailIsInTheDatabase()
        {
            Guid guid = Guid.NewGuid();

            this.businessValidation.Setup(validation => validation.IsValidGetNews(this.mockNewsRepository.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(true));
            this.businessValidation.Setup(validation => validation.IsValidModifyNews(this.mockNewsRepository.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>(), It.IsAny<string>())).Returns(Task.FromResult(true));
            this.businessValidation.Setup(validation => validation.IsValidGetImageNews(this.mockImageRepository.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(true));

            this.mockImageRepository.Setup(storage => storage.GetNewsImage(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(Image.FromFile("goten.png")));

            var newsImageController = new NewsImageController(this.mockNewsRepository.Object, this.mockImageRepository.Object, this.inputValidation.Object, this.businessValidation.Object);

            var request = new HttpRequestMessage(HttpMethod.Delete, GOUriBuilder.BuildAbsoluteNewsImageUri(Scheme, Host, Port, City, Date, guid.ToString()));
            request.Headers.Referrer = new Uri(GOUriBuilder.BuildAbsoluteNewsImageUri(Scheme, Host, Port, City, Date, guid.ToString()));

            newsImageController.ConfigureForTesting(request, "DeleteImage", new HttpRoute(GOUriBuilder.NewsImageTemplate));
            newsImageController.User = new GenericPrincipal(new GenericIdentity(DefaultNickname), null);

            HttpResponseMessage response = newsImageController.Delete(City, Date, guid.ToString()).Result;

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public void TestDeleteImageReturns400_WhenInputValidationFails()
        {
            this.inputValidation.Setup(validation => validation.ValidateNewsParameters(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Throws(new InputValidationException(HttpStatusCode.BadRequest, string.Empty));

            this.AssertDeleteFails(HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void TestDeleteImageReturns404_WhenTheNewsIsNotTheDatabase()
        {
            this.businessValidation.Setup(validation => validation.IsValidGetNews(this.mockNewsRepository.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(false));
            this.businessValidation.Setup(validation => validation.IsValidModifyNews(this.mockNewsRepository.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>(), It.IsAny<string>())).Returns(Task.FromResult(true));
            this.businessValidation.Setup(validation => validation.IsValidGetImageNews(this.mockImageRepository.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(true));

            this.mockImageRepository.Setup(storage => storage.GetNewsThumbnailImage(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(Image.FromFile("goten.png")));

            this.AssertDeleteFails(HttpStatusCode.NotFound);
        }

        [TestMethod]
        public void TestDeleteImageReturns401_WhenTheUserIsNotTheAuthor()
        {
            this.businessValidation.Setup(validation => validation.IsValidGetNews(this.mockNewsRepository.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(true));
            this.businessValidation.Setup(validation => validation.IsValidModifyNews(this.mockNewsRepository.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>(), It.IsAny<string>())).Returns(Task.FromResult(false));
            this.businessValidation.Setup(validation => validation.IsValidGetImageNews(this.mockImageRepository.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(true));

            this.mockImageRepository.Setup(storage => storage.GetNewsThumbnailImage(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(Image.FromFile("goten.png")));

            this.AssertDeleteFails(HttpStatusCode.Unauthorized);
        }

        [TestMethod]
        public void TestDeleteImageReturns404_WhenTheNewsImageIsNotTheDatabase()
        {
            this.businessValidation.Setup(validation => validation.IsValidGetNews(this.mockNewsRepository.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(true));
            this.businessValidation.Setup(validation => validation.IsValidModifyNews(this.mockNewsRepository.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>(), It.IsAny<string>())).Returns(Task.FromResult(true));
            this.businessValidation.Setup(validation => validation.IsValidGetImageNews(this.mockImageRepository.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(false));

            this.mockImageRepository.Setup(storage => storage.GetNewsThumbnailImage(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(Image.FromFile("goten.png")));

            this.AssertDeleteFails(HttpStatusCode.NotFound);
        }

        #region Assert helper methods

        private void AssertGetFails(HttpStatusCode resultCode)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, GOUriBuilder.BuildAbsoluteNewsImageUri(Scheme, Host, Port, City, Date, Id.ToString()));
            request.Headers.Referrer = new Uri(GOUriBuilder.BuildAbsoluteNewsImageUri(Scheme, Host, Port, City, Date, Id.ToString()));

            var newsImageController = new NewsImageController(this.mockNewsRepository.Object, this.mockImageRepository.Object, this.inputValidation.Object, this.businessValidation.Object);
            newsImageController.ConfigureForTesting(request, "GetImage", new HttpRoute(GOUriBuilder.NewsImageTemplate));

            HttpResponseMessage response = newsImageController.Get(City, Date, Id.ToString()).Result;

            Assert.AreEqual(resultCode, response.StatusCode);
            this.mockImageRepository.Verify(storage => storage.GetNewsImage(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>()), Times.Never());
        }

        private void AssertPostFails(HttpStatusCode resultCode)
        {
            var newsImageController = new NewsImageController(this.mockNewsRepository.Object, this.mockImageRepository.Object, this.inputValidation.Object, this.businessValidation.Object);

            var request = new HttpRequestMessage(HttpMethod.Post, GOUriBuilder.BuildAbsoluteNewsImageUri(Scheme, Host, Port, City, Date, Id.ToString()));
            request.Headers.Referrer = new Uri(GOUriBuilder.BuildAbsoluteNewsImageUri(Scheme, Host, Port, City, Date, Id.ToString()));
            request.Content = new ByteArrayContent(new byte[0]);

            newsImageController.ConfigureForTesting(request, "PostImage", new HttpRoute(GOUriBuilder.NewsImageTemplate));
            newsImageController.User = new GenericPrincipal(new GenericIdentity(DefaultNickname), null);

            HttpResponseMessage response = newsImageController.Post(City, Date, Id.ToString()).Result;

            Assert.AreEqual(resultCode, response.StatusCode);
            this.mockImageRepository.Verify(storage => storage.CreateNewsImage(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>(), It.IsAny<Image>()), Times.Never());
        }

        private void AssertDeleteFails(HttpStatusCode resultCode)
        {
            var newsImageController = new NewsImageController(this.mockNewsRepository.Object, this.mockImageRepository.Object, this.inputValidation.Object, this.businessValidation.Object);

            var request = new HttpRequestMessage(HttpMethod.Delete, GOUriBuilder.BuildAbsoluteNewsImageUri(Scheme, Host, Port, City, Date, Id.ToString()));
            request.Headers.Referrer = new Uri(GOUriBuilder.BuildAbsoluteNewsImageUri(Scheme, Host, Port, City, Date, Id.ToString()));

            newsImageController.ConfigureForTesting(request, "DeleteImage", new HttpRoute(GOUriBuilder.NewsImageTemplate));
            newsImageController.User = new GenericPrincipal(new GenericIdentity(DefaultNickname), null);

            HttpResponseMessage response = newsImageController.Delete(City, Date, Id.ToString()).Result;

            Assert.AreEqual(resultCode, response.StatusCode);
            this.mockImageRepository.Verify(storage => storage.DeleteNewsImage(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>()), Times.Never());
        }

        #endregion
    }
}
