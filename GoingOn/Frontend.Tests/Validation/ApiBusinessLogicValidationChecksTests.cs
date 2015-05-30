// ****************************************************************************
// <copyright file="ApiBusinessLogicValidationChecksTests.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// Validation of business rules
// </summary>
// ****************************************************************************

namespace GoingOn.Frontend.Tests.Validation
{
    using System;
    using System.Threading.Tasks;
    using GoingOn.Frontend.Entities;
    using GoingOn.Frontend.Validation;
    using GoingOn.Repository;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;
    using Model.EntitiesBll;
    
    [TestClass]
    public class ApiBusinessLogicValidationChecksTests
    {
        private Mock<IUserRepository> userRepositoryMock;
        private Mock<INewsRepository> newsRepositoryMock;
        private Mock<IHotNewsRepository> hotNewsRepositoryMock;
        private Mock<IImageRepository> imageRepositoryMock;
        private Mock<IVoteRepository> voteRepositoryMock;
        private ApiBusinessLogicValidationChecks businessValidation;

        private static readonly User user = new User { Nickname = "nickname", Password = "password" };

        private static readonly News news = new News { Title = "title", Content = "content" };

        [TestInitialize]
        public void Initialize()
        {
            this.userRepositoryMock = new Mock<IUserRepository>();
            this.newsRepositoryMock = new Mock<INewsRepository>();
            this.hotNewsRepositoryMock = new Mock<IHotNewsRepository>();
            this.voteRepositoryMock = new Mock<IVoteRepository>();
            this.imageRepositoryMock = new Mock<IImageRepository>();
            this.businessValidation = new ApiBusinessLogicValidationChecks();
        }

        [TestMethod]
        public void TestIsValidCreateUserSucceedsIfStorageDoesNotContainsTheUser()
        {
            this.userRepositoryMock.Setup(storage => storage.ContainsUser(It.IsAny<UserBll>())).Returns(Task.FromResult(false));

            Assert.IsTrue(this.businessValidation.IsValidCreateUser(this.userRepositoryMock.Object, user).Result);
        }

        [TestMethod]
        public void TestIsValidCreateUserFailsIfStorageContainsTheUser()
        {
            this.userRepositoryMock.Setup(storage => storage.ContainsUser(It.IsAny<UserBll>())).Returns(Task.FromResult(true));

            Assert.IsFalse(this.businessValidation.IsValidCreateUser(this.userRepositoryMock.Object, user).Result);
        }

        [TestMethod]
        public void TestIsValidCreateNewsSucceedsIfStorageDoesNotContainsTheNews()
        {
            this.newsRepositoryMock.Setup(storage => storage.ContainsNewsCheckContent(It.IsAny<NewsBll>())).Returns(Task.FromResult(false));

            Assert.IsTrue(this.businessValidation.IsValidCreateNews(this.newsRepositoryMock.Object, news, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()).Result);
        }

        [TestMethod]
        public void TestIsValidCreateNewsFailsIfStorageContainsTheNews()
        {
            this.newsRepositoryMock.Setup(storage => storage.ContainsNewsCheckContent(It.IsAny<NewsBll>())).Returns(Task.FromResult(true));

            Assert.IsFalse(this.businessValidation.IsValidCreateNews(this.newsRepositoryMock.Object, news, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()).Result);
        }

        [TestMethod]
        public void TestIsValidGetHotNews_SucceedsIfStorageHasNews()
        {
            this.hotNewsRepositoryMock.Setup(storage => storage.ContainsAnyHotNews(It.IsAny<string>(), It.IsAny<DateTime>())).Returns(Task.FromResult(true));

            Assert.IsTrue(this.businessValidation.IsValidGetHotNews(this.hotNewsRepositoryMock.Object, It.IsAny<string>(), It.IsAny<DateTime>()).Result);
        }

        [TestMethod]
        public void TestIsValidGetHotNews_FailsIfStorageDoesNotHaveNews()
        {
            this.hotNewsRepositoryMock.Setup(storage => storage.ContainsAnyHotNews(It.IsAny<string>(), It.IsAny<DateTime>())).Returns(Task.FromResult(false));

            Assert.IsFalse(this.businessValidation.IsValidGetHotNews(this.hotNewsRepositoryMock.Object, It.IsAny<string>(), It.IsAny<DateTime>()).Result);
        }

        [TestMethod]
        public void TestIsValidGetVote()
        {
            this.voteRepositoryMock.Setup(
                storage =>
                    storage.ContainsVote(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>(), It.IsAny<string>()))
                .Returns(Task.FromResult(true));

            Assert.IsTrue(this.businessValidation.IsValidGetVote(this.voteRepositoryMock.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>(), It.IsAny<string>()).Result);
        }

        [TestMethod]
        public void TestIsValidGetVote_FailsIfStorageDoesNotHaveVote()
        {
            this.voteRepositoryMock.Setup(
                storage =>
                    storage.ContainsVote(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>(), It.IsAny<string>()))
                .Returns(Task.FromResult(false));

            Assert.IsFalse(this.businessValidation.IsValidGetVote(this.voteRepositoryMock.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>(), It.IsAny<string>()).Result);
        }

        [TestMethod]
        public void TestIsAuthorizedUser()
        {
            Assert.IsTrue(this.businessValidation.IsAuthorizedUser("alberto", "alberto"));
        }

        [TestMethod]
        public void TestIsAuthorizedUser_FailsIfNicknamesAreNotEqual()
        {
            Assert.IsFalse(this.businessValidation.IsAuthorizedUser("alberto", "Alberto"));
            Assert.IsFalse(this.businessValidation.IsAuthorizedUser("alberto", "alBErto"));
            Assert.IsFalse(this.businessValidation.IsAuthorizedUser("alberto", string.Empty));
        }

        [TestMethod]
        public void TestIsUserCreated()
        {
            this.userRepositoryMock.Setup(repository => repository.ContainsUser(It.IsAny<UserBll>()))
                .Returns(Task.FromResult(true));

            Assert.IsTrue(this.businessValidation.IsUserCreated(this.userRepositoryMock.Object, It.IsAny<string>()).Result);
        }

        [TestMethod]
        public void TestIsUserCreated_FailsIfUserDoesNotExist()
        {
            this.userRepositoryMock.Setup(repository => repository.ContainsUser(It.IsAny<UserBll>()))
                .Returns(Task.FromResult(false));

            Assert.IsFalse(this.businessValidation.IsUserCreated(this.userRepositoryMock.Object, It.IsAny<string>()).Result);
        }

        [TestMethod]
        public void TestIsUserCreatedOverload()
        {
            this.userRepositoryMock.Setup(repository => repository.ContainsUser(It.IsAny<UserBll>()))
                .Returns(Task.FromResult(true));

            Assert.IsTrue(this.businessValidation.IsUserCreated(this.userRepositoryMock.Object, new User()).Result);
        }

        [TestMethod]
        public void TestIsUserCreatedOverload_FailsIfUserDoesNotExist()
        {
            this.userRepositoryMock.Setup(repository => repository.ContainsUser(It.IsAny<UserBll>()))
                .Returns(Task.FromResult(false));

            Assert.IsFalse(this.businessValidation.IsUserCreated(this.userRepositoryMock.Object, new User()).Result);
        }

        [TestMethod]
        public void TestIsValidGetNews()
        {
            this.newsRepositoryMock.Setup(
                repository => repository.ContainsNews(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(true));

            Assert.IsTrue(this.businessValidation.IsValidGetNews(this.newsRepositoryMock.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>()).Result);
        }

        [TestMethod]
        public void TestIsValidGetNews_FailsIfNewsDoesNotExist()
        {
            this.newsRepositoryMock.Setup(
                repository => repository.ContainsNews(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(false));

            Assert.IsFalse(this.businessValidation.IsValidGetNews(this.newsRepositoryMock.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>()).Result);
        }

        [TestMethod]
        public void TestIsValidModifyNews()
        {
            this.newsRepositoryMock.Setup(
                repository => repository.ContainsNews(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(true));

            Assert.IsTrue(this.businessValidation.IsValidGetNews(this.newsRepositoryMock.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>()).Result);
        }

        [TestMethod]
        public void TestIsValidModifyNews_FailsIfNewsDoesNotExist()
        {
            this.newsRepositoryMock.Setup(
                repository => repository.ContainsNews(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(false));

            Assert.IsFalse(this.businessValidation.IsValidGetNews(this.newsRepositoryMock.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>()).Result);
        }

        [TestMethod]
        public void TestIsValidGetImageNews()
        {
            this.imageRepositoryMock.Setup(
                repository => repository.ContainsImage(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(true));

            Assert.IsTrue(this.businessValidation.IsValidGetImageNews(this.imageRepositoryMock.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>()).Result);
        }

        [TestMethod]
        public void TestIsValidGetImageNews_FailsIfImageDoesNotExist()
        {
            this.imageRepositoryMock.Setup(
                repository => repository.ContainsImage(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(false));

            Assert.IsFalse(this.businessValidation.IsValidGetImageNews(this.imageRepositoryMock.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>()).Result);
        }

        [TestMethod]
        public void TestIsValidGetThumbnailImageNews()
        {
            this.imageRepositoryMock.Setup(
                repository => repository.ContainsImageThumbnail(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(true));

            Assert.IsTrue(this.businessValidation.IsValidGetThumbnailImageNews(this.imageRepositoryMock.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>()).Result);
        }

        [TestMethod]
        public void TestIsValidGetThumbnailImageNews_FailsIfImageDoesNotExist()
        {
            this.imageRepositoryMock.Setup(
                repository => repository.ContainsImageThumbnail(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(false));

            Assert.IsFalse(this.businessValidation.IsValidGetThumbnailImageNews(this.imageRepositoryMock.Object, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>()).Result);
        }
    }
}
