// ****************************************************************************
// <copyright file="ApiBusinessLogicValidationChecksTests.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

using Frontend.Entities;
using Frontend.Validation;

namespace GoingOn.Tests.Validation
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;
    using Model.EntitiesBll;
    
    [TestClass]
    public class ApiBusinessLogicValidationChecksTests
    {
        private Mock<IUserStorage> userStorageMock;
        private Mock<INewsStorage> newsStorageMock;
        private ApiBusinessLogicValidationChecks businessValidation;

        private static readonly User user = new User("nickname", "password");

        private static readonly News news = new News("title", "content");

        [TestInitialize]
        public void Initialize()
        {
            userStorageMock = new Mock<IUserStorage>();
            newsStorageMock = new Mock<INewsStorage>();
            businessValidation = new ApiBusinessLogicValidationChecks();
        }

        [TestMethod]
        public void TestIsValidCreateUserSucceedsIfStorageDoesNotContainsTheUser()
        {
            userStorageMock.Setup(storage => storage.ContainsUser(It.IsAny<UserBll>())).Returns(Task.FromResult(false));

            Assert.IsTrue(businessValidation.IsValidCreateUser(userStorageMock.Object, user));
        }

        [TestMethod]
        public void TestIsValidCreateUserFailsIfStorageContainsTheUser()
        {
            userStorageMock.Setup(storage => storage.ContainsUser(It.IsAny<UserBll>())).Returns(Task.FromResult(true));

            Assert.IsFalse(businessValidation.IsValidCreateUser(userStorageMock.Object, user));
        }

        [TestMethod]
        public void TestIsValidCreateNewsSucceedsIfStorageDoesNotContainsTheNews()
        {
            newsStorageMock.Setup(storage => storage.ContainsNews(It.IsAny<NewsBll>())).Returns(Task.FromResult(false));

            Assert.IsTrue(businessValidation.IsValidCreateNews(newsStorageMock.Object, news, It.IsAny<string>()));
        }

        [TestMethod]
        public void TestIsValidCreateNewsFailsIfStorageContainsTheNews()
        {
            newsStorageMock.Setup(storage => storage.ContainsNews(It.IsAny<NewsBll>())).Returns(Task.FromResult(true));

            Assert.IsFalse(businessValidation.IsValidCreateNews(newsStorageMock.Object, news, It.IsAny<string>()));
        }
    }
}
