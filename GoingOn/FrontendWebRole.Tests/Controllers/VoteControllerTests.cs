// ****************************************************************************
// <copyright file="VoteControllerTests.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// UserController tests class
// </summary>
// ****************************************************************************

namespace GoingOn.FrontendWebRole.Tests.Controllers
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Security.Principal;
    using System.Threading.Tasks;
    using System.Web.Http.Routing;
    using GoingOn.Common;
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
    public class VoteControllerTests
    {
        private const string Scheme = "http";
        private const string Host = "test.com";
        private const int Port = 123;

        private const string DefaultNickname = "nickname";
        private const string City = "Malaga";
        private const string Date = "2015-05-11";
        private static readonly Guid Id = Guid.NewGuid();

        private static readonly Vote DefaultVote = new Vote { Value = 5 };

        private Mock<INewsRepository> mockNewsRepository;
        private Mock<IVoteRepository> mockVoteRepository;
        private Mock<IApiInputValidationChecks> inputValidation;
        private Mock<IApiBusinessLogicValidationChecks> businessValidation;

        // TODO: write tests

        [TestInitialize]
        public void Initizalize()
        {
            this.mockNewsRepository = new Mock<INewsRepository>();
            this.mockVoteRepository = new Mock<IVoteRepository>();
            this.inputValidation = new Mock<IApiInputValidationChecks>();
            this.businessValidation = new Mock<IApiBusinessLogicValidationChecks>();
        }

        [TestMethod]
        public void TestGetReturns200_IfEverythingIsOk()
        {
            this.businessValidation.Setup(validation => validation.IsValidGetNews(this.mockNewsRepository.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(true));
            this.businessValidation.Setup(validation => validation.IsValidGetVote(this.mockVoteRepository.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>(), It.IsAny<string>())).Returns(Task.FromResult(true));

            this.mockVoteRepository.Setup(
                repository =>
                    repository.GetVote(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new VoteBll { Value = 5 }));

            var voteController = new VoteController(this.mockNewsRepository.Object, this.mockVoteRepository.Object, this.inputValidation.Object, this.businessValidation.Object);

            var request = new HttpRequestMessage(HttpMethod.Get, GOUriBuilder.BuildAbsoluteVoteUri(Scheme, Host, Port, City, Date, Id.ToString()));
            request.Headers.Referrer = new Uri(GOUriBuilder.BuildAbsoluteVoteUri(Scheme, Host, Port, City, Date, Id.ToString()));

            voteController.ConfigureForTesting(request, "GetVote", new HttpRoute(GOUriBuilder.NewsVoteTemplate));
            voteController.User = new GenericPrincipal(new GenericIdentity(DefaultNickname), null);

            HttpResponseMessage response = voteController.Get(City, Date, Id.ToString()).Result;

            HttpContent content = response.Content;
            string jsonContent = content.ReadAsStringAsync().Result;
            var actualVote = JsonConvert.DeserializeObject<Vote>(jsonContent);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(5, actualVote.Value);
        }

        [TestMethod]
        public void TestGetReturns400_IfInputValidationFails()
        {
            this.inputValidation.Setup(validation => validation.ValidateNewsParameters(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new InputValidationException(HttpStatusCode.BadRequest, string.Empty));

            this.AssertGetFails(HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void TestGetReturns404_IfNewsIsNotInTheDatabase()
        {
            this.businessValidation.Setup(validation => validation.IsValidGetNews(this.mockNewsRepository.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(false));

            this.AssertGetFails(HttpStatusCode.NotFound);
        }

        [TestMethod]
        public void TestGetReturns400_IfTheUserHasNotVotedYet()
        {
            this.businessValidation.Setup(validation => validation.IsValidGetNews(this.mockNewsRepository.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(true));
            this.businessValidation.Setup(validation => validation.IsValidGetVote(this.mockVoteRepository.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>(), It.IsAny<string>())).Returns(Task.FromResult(false));

            this.AssertGetFails(HttpStatusCode.NotFound);
        }

        [TestMethod]
        public void TestPostReturns200_IfEverythingIsOk()
        {
            this.inputValidation.Setup(validation => validation.IsValidVote(It.IsAny<Vote>())).Returns(true);
            this.businessValidation.Setup(validation => validation.IsValidGetNews(this.mockNewsRepository.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(true));
            this.businessValidation.Setup(validation => validation.IsValidGetVote(this.mockVoteRepository.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>(), It.IsAny<string>())).Returns(Task.FromResult(false));

            var voteController = new VoteController(this.mockNewsRepository.Object, this.mockVoteRepository.Object, this.inputValidation.Object, this.businessValidation.Object);

            var request = new HttpRequestMessage(HttpMethod.Post, GOUriBuilder.BuildAbsoluteVoteUri(Scheme, Host, Port, City, Date, Id.ToString()));
            request.Headers.Referrer = new Uri(GOUriBuilder.BuildAbsoluteVoteUri(Scheme, Host, Port, City, Date, Id.ToString()));

            voteController.ConfigureForTesting(request, "PostVote", new HttpRoute(GOUriBuilder.NewsVoteTemplate));
            voteController.User = new GenericPrincipal(new GenericIdentity(DefaultNickname), null);

            HttpResponseMessage response = voteController.Post(City, Date, Id.ToString(), new Vote { Value = 5 }).Result;

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            this.mockVoteRepository.Verify(repository => repository.AddVote(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<VoteBll>()), Times.Once);
        }

        [TestMethod]
        public void TestPostReturns400_IfNewsInputValidationFails()
        {
            this.inputValidation.Setup(validation => validation.ValidateNewsParameters(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new InputValidationException(HttpStatusCode.BadRequest, string.Empty));

            this.AssertPostFails(HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void TestPostReturns400_IfVoteInputValidationFails()
        {
            this.inputValidation.Setup(validation => validation.IsValidVote(It.IsAny<Vote>())).Returns(false);

            this.AssertPostFails(HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void TestPostReturns404_IfNewsIsNotInTheDatabase()
        {
            this.inputValidation.Setup(validation => validation.IsValidVote(It.IsAny<Vote>())).Returns(true);
            this.businessValidation.Setup(validation => validation.IsValidGetNews(this.mockNewsRepository.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(false));

            this.AssertPostFails(HttpStatusCode.NotFound);
        }

        [TestMethod]
        public void TestPostReturns400_IfTheUserHasVotedAlready()
        {
            this.inputValidation.Setup(validation => validation.IsValidVote(It.IsAny<Vote>())).Returns(true);
            this.businessValidation.Setup(validation => validation.IsValidGetNews(this.mockNewsRepository.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(true));
            this.businessValidation.Setup(validation => validation.IsValidGetVote(this.mockVoteRepository.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>(), It.IsAny<string>())).Returns(Task.FromResult(true));

            this.AssertPostFails(HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void TestPatchReturns200_IfEverythingIsOk()
        {
            this.inputValidation.Setup(validation => validation.IsValidVote(It.IsAny<Vote>())).Returns(true);
            this.businessValidation.Setup(validation => validation.IsValidGetNews(this.mockNewsRepository.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(true));
            this.businessValidation.Setup(validation => validation.IsValidGetVote(this.mockVoteRepository.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>(), It.IsAny<string>())).Returns(Task.FromResult(true));

            var voteController = new VoteController(this.mockNewsRepository.Object, this.mockVoteRepository.Object, this.inputValidation.Object, this.businessValidation.Object);

            var request = new HttpRequestMessage(new HttpMethod("PATCH"), GOUriBuilder.BuildAbsoluteVoteUri(Scheme, Host, Port, City, Date, Id.ToString()));
            request.Headers.Referrer = new Uri(GOUriBuilder.BuildAbsoluteVoteUri(Scheme, Host, Port, City, Date, Id.ToString()));

            voteController.ConfigureForTesting(request, "PatchVote", new HttpRoute(GOUriBuilder.NewsVoteTemplate));
            voteController.User = new GenericPrincipal(new GenericIdentity(DefaultNickname), null);

            HttpResponseMessage response = voteController.Patch(City, Date, Id.ToString(), new Vote { Value = 5 }).Result;

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            this.mockVoteRepository.Verify(repository => repository.UpdateVote(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<VoteBll>()), Times.Once);
        }

        [TestMethod]
        public void TestPatchReturns400_IfNewsInputValidationFails()
        {
            this.inputValidation.Setup(validation => validation.ValidateNewsParameters(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new InputValidationException(HttpStatusCode.BadRequest, string.Empty));

            this.AssertPatchFails(HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void TestPatchReturns400_IfVoteInputValidationFails()
        {
            this.inputValidation.Setup(validation => validation.IsValidVote(It.IsAny<Vote>())).Returns(false);

            this.AssertPatchFails(HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void TestPatchReturns404_IfNewsIsNotInTheDatabase()
        {
            this.inputValidation.Setup(validation => validation.IsValidVote(It.IsAny<Vote>())).Returns(true);
            this.businessValidation.Setup(validation => validation.IsValidGetNews(this.mockNewsRepository.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(false));

            this.AssertPatchFails(HttpStatusCode.NotFound);
        }

        [TestMethod]
        public void TestPatchReturns404_IfTheUserHasNotVotedYet()
        {
            this.inputValidation.Setup(validation => validation.IsValidVote(It.IsAny<Vote>())).Returns(true);
            this.businessValidation.Setup(validation => validation.IsValidGetNews(this.mockNewsRepository.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(true));
            this.businessValidation.Setup(validation => validation.IsValidGetVote(this.mockVoteRepository.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>(), It.IsAny<string>())).Returns(Task.FromResult(false));

            this.AssertPatchFails(HttpStatusCode.NotFound);
        }

        [TestMethod]
        public void TestDeleteReturns200_IfEverythingIsOk()
        {
            this.businessValidation.Setup(validation => validation.IsValidGetNews(this.mockNewsRepository.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(true));
            this.businessValidation.Setup(validation => validation.IsValidGetVote(this.mockVoteRepository.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>(), It.IsAny<string>())).Returns(Task.FromResult(true));

            var voteController = new VoteController(this.mockNewsRepository.Object, this.mockVoteRepository.Object, this.inputValidation.Object, this.businessValidation.Object);

            var request = new HttpRequestMessage(HttpMethod.Delete, GOUriBuilder.BuildAbsoluteVoteUri(Scheme, Host, Port, City, Date, Id.ToString()));
            request.Headers.Referrer = new Uri(GOUriBuilder.BuildAbsoluteVoteUri(Scheme, Host, Port, City, Date, Id.ToString()));

            voteController.ConfigureForTesting(request, "DeleteVote", new HttpRoute(GOUriBuilder.NewsVoteTemplate));
            voteController.User = new GenericPrincipal(new GenericIdentity(DefaultNickname), null);

            HttpResponseMessage response = voteController.Delete(City, Date, Id.ToString()).Result;

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            this.mockVoteRepository.Verify(repository => repository.DeleteVote(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void TestDeleteReturns400_IfInputValidationFails()
        {
            this.inputValidation.Setup(validation => validation.ValidateNewsParameters(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new InputValidationException(HttpStatusCode.BadRequest, string.Empty));

            this.AssertDeleteFails(HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void TestDeleteReturns404_IfNewsIsNotInTheDatabase()
        {
            this.businessValidation.Setup(validation => validation.IsValidGetNews(this.mockNewsRepository.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(false));

            this.AssertDeleteFails(HttpStatusCode.NotFound);
        }

        [TestMethod]
        public void TestDeleteReturns404_IfTheUserHasNotVotedYet()
        {
            this.businessValidation.Setup(validation => validation.IsValidGetNews(this.mockNewsRepository.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(Task.FromResult(true));
            this.businessValidation.Setup(validation => validation.IsValidGetVote(this.mockVoteRepository.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>(), It.IsAny<string>())).Returns(Task.FromResult(false));

            this.AssertDeleteFails(HttpStatusCode.NotFound);
        }

        #region Assert helper methods

        private void AssertGetFails(HttpStatusCode resultCode)
        {
            var voteController = new VoteController(this.mockNewsRepository.Object, this.mockVoteRepository.Object, this.inputValidation.Object, this.businessValidation.Object);

            var request = new HttpRequestMessage(HttpMethod.Get, GOUriBuilder.BuildAbsoluteVoteUri(Scheme, Host, Port, City, Date, Id.ToString()));
            request.Headers.Referrer = new Uri(GOUriBuilder.BuildAbsoluteVoteUri(Scheme, Host, Port, City, Date, Id.ToString()));

            voteController.ConfigureForTesting(request, "GetVote", new HttpRoute(GOUriBuilder.NewsVoteTemplate));
            voteController.User = new GenericPrincipal(new GenericIdentity(DefaultNickname), null);

            HttpResponseMessage response = voteController.Get(City, Date, Id.ToString()).Result;

            Assert.AreEqual(resultCode, response.StatusCode);
            this.mockVoteRepository.Verify(storage => storage.GetVote(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>(), It.IsAny<string>()), Times.Never());
        }

        private void AssertPostFails(HttpStatusCode resultCode)
        {
            var voteController = new VoteController(this.mockNewsRepository.Object, this.mockVoteRepository.Object, this.inputValidation.Object, this.businessValidation.Object);

            var request = new HttpRequestMessage(HttpMethod.Post, GOUriBuilder.BuildAbsoluteVoteUri(Scheme, Host, Port, City, Date, Id.ToString()));
            request.Headers.Referrer = new Uri(GOUriBuilder.BuildAbsoluteVoteUri(Scheme, Host, Port, City, Date, Id.ToString()));

            voteController.ConfigureForTesting(request, "PostVote", new HttpRoute(GOUriBuilder.NewsVoteTemplate));
            voteController.User = new GenericPrincipal(new GenericIdentity(DefaultNickname), null);

            HttpResponseMessage response = voteController.Post(City, Date, Id.ToString(), DefaultVote).Result;

            Assert.AreEqual(resultCode, response.StatusCode);
            this.mockVoteRepository.Verify(storage => storage.AddVote(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<VoteBll>()), Times.Never());
        }

        private void AssertPatchFails(HttpStatusCode resultCode)
        {
            var voteController = new VoteController(this.mockNewsRepository.Object, this.mockVoteRepository.Object, this.inputValidation.Object, this.businessValidation.Object);

            var request = new HttpRequestMessage(HttpMethod.Post, GOUriBuilder.BuildAbsoluteVoteUri(Scheme, Host, Port, City, Date, Id.ToString()));
            request.Headers.Referrer = new Uri(GOUriBuilder.BuildAbsoluteVoteUri(Scheme, Host, Port, City, Date, Id.ToString()));

            voteController.ConfigureForTesting(request, "PostVote", new HttpRoute(GOUriBuilder.NewsVoteTemplate));
            voteController.User = new GenericPrincipal(new GenericIdentity(DefaultNickname), null);

            HttpResponseMessage response = voteController.Patch(City, Date, Id.ToString(), DefaultVote).Result;

            Assert.AreEqual(resultCode, response.StatusCode);
            this.mockVoteRepository.Verify(storage => storage.AddVote(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<VoteBll>()), Times.Never());
        }

        private void AssertDeleteFails(HttpStatusCode resultCode)
        {
            var voteController = new VoteController(this.mockNewsRepository.Object, this.mockVoteRepository.Object, this.inputValidation.Object, this.businessValidation.Object);

            var request = new HttpRequestMessage(HttpMethod.Delete, GOUriBuilder.BuildAbsoluteVoteUri(Scheme, Host, Port, City, Date, Id.ToString()));
            request.Headers.Referrer = new Uri(GOUriBuilder.BuildAbsoluteVoteUri(Scheme, Host, Port, City, Date, Id.ToString()));

            voteController.ConfigureForTesting(request, "DeleteVote", new HttpRoute(GOUriBuilder.NewsVoteTemplate));
            voteController.User = new GenericPrincipal(new GenericIdentity(DefaultNickname), null);

            HttpResponseMessage response = voteController.Delete(City, Date, Id.ToString()).Result;

            Assert.AreEqual(resultCode, response.StatusCode);
            this.mockVoteRepository.Verify(storage => storage.DeleteVote(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>(), It.IsAny<string>()), Times.Never());
        }

        #endregion
    }
}
