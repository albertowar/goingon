// ****************************************************************************
// <copyright file="UserControllerTest.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

using System.Net;
using System.Net.Http;
using GoingOn.Entities;
using GoingOn.Validation;
using Model.EntitiesBll;
using WebApiContrib.Testing;

namespace GoingOn.Tests.Controllers
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using GoingOn.Controllers;
    using Moq;

    [TestClass]
    public class UserControllerTest
    {
        private Mock<IUserStorage> userStorageMock;
        private Mock<IApiInputValidationChecks> inputValidation;
        private Mock<IApiBusinessLogicValidationChecks> businessValidation;

        private static readonly User user = new User("nickname", "passowrd");

        [TestInitialize]
        public void Initizalize()
        {
            userStorageMock = new Mock<IUserStorage>();
            inputValidation = new Mock<IApiInputValidationChecks>();
            businessValidation = new Mock<IApiBusinessLogicValidationChecks>();
        }

        [TestMethod]
        public void TestPostUserReturn200OkWhenCreatesUser()
        {
            inputValidation.Setup(validation => validation.IsValidUser(It.IsAny<User>())).Returns(true);
            businessValidation.Setup(validation => validation.IsValidUser(userStorageMock.Object, It.IsAny<User>())).Returns(true);

            UserController userController = new UserController(userStorageMock.Object, inputValidation.Object, businessValidation.Object);
            userController.ConfigureForTesting(HttpMethod.Post, "http://test.com/api/user");

            HttpResponseMessage response = userController.Post(user);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            userStorageMock.Verify(storage => storage.AddUser(It.IsAny<UserBll>()), Times.Once());
        }

        [TestMethod]
        public void TestPostUserReturn400BadRequestWhenInputValidationFails()
        {
            inputValidation.Setup(validation => validation.IsValidUser(It.IsAny<User>())).Returns(false);
            businessValidation.Setup(validation => validation.IsValidUser(userStorageMock.Object, It.IsAny<User>())).Returns(true);

            UserController userController = new UserController(userStorageMock.Object, inputValidation.Object, businessValidation.Object);
            userController.ConfigureForTesting(HttpMethod.Post, "http://test.com/api/user");

            HttpResponseMessage response = userController.Post(user);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            userStorageMock.Verify(storage => storage.AddUser(It.IsAny<UserBll>()), Times.Never());
        }

        [TestMethod]
        public void TestPostUserReturn400BadRequestWhenBusinessValidationFails()
        {
            inputValidation.Setup(validation => validation.IsValidUser(It.IsAny<User>())).Returns(true);
            businessValidation.Setup(validation => validation.IsValidUser(userStorageMock.Object, It.IsAny<User>())).Returns(false);

            UserController userController = new UserController(userStorageMock.Object, inputValidation.Object, businessValidation.Object);
            userController.ConfigureForTesting(HttpMethod.Post, "http://test.com/api/user");

            HttpResponseMessage response = userController.Post(user);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            userStorageMock.Verify(storage => storage.AddUser(It.IsAny<UserBll>()), Times.Never());
        }
    }
}
