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
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
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

        private static readonly User user = new User("nickname", "password");

        [TestInitialize]
        public void Initizalize()
        {
            userStorageMock = new Mock<IUserStorage>();
            inputValidation = new Mock<IApiInputValidationChecks>();
            businessValidation = new Mock<IApiBusinessLogicValidationChecks>();
        }

        [TestMethod]
        public void TestGetUserReturns200OkWhenTheUserIsInTheDatabase()
        {
            userStorageMock.Setup(storage => storage.ContainsUser(It.IsAny<UserBll>())).Returns(Task.FromResult(true));
            userStorageMock.Setup(storage => storage.GetUser(It.IsAny<string>())).Returns(Task.FromResult(new UserBll("username", "password")));

            UserController userController = new UserController(userStorageMock.Object, inputValidation.Object, businessValidation.Object);
            userController.ConfigureForTesting(HttpMethod.Get, "http://test.com/api/user");

            var getTask = userController.Get("username");

            getTask.Wait();

            HttpResponseMessage response = getTask.Result;

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            // Check the user too
        }

        [TestMethod]
        public void TestGetUserReturns404NotFoundWhenTheUserIsNotInTheDatabase()
        {

        }

        [TestMethod]
        public void TestPostUserReturn200OkWhenCreatesUser()
        {
            inputValidation.Setup(validation => validation.IsValidUser(It.IsAny<User>())).Returns(true);
            businessValidation.Setup(validation => validation.IsValidCreateUser(userStorageMock.Object, It.IsAny<User>())).Returns(true);

            UserController userController = new UserController(userStorageMock.Object, inputValidation.Object, businessValidation.Object);
            userController.ConfigureForTesting(HttpMethod.Post, "http://test.com/api/user");

            HttpResponseMessage response = userController.Post(user);

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            userStorageMock.Verify(storage => storage.AddUser(It.IsAny<UserBll>()), Times.Once());
        }

        [TestMethod]
        public void TestPostUserReturn400BadRequestWhenInputValidationFails()
        {
            inputValidation.Setup(validation => validation.IsValidUser(It.IsAny<User>())).Returns(false);
            businessValidation.Setup(validation => validation.IsValidCreateUser(userStorageMock.Object, It.IsAny<User>())).Returns(true);

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
            businessValidation.Setup(validation => validation.IsValidCreateUser(userStorageMock.Object, It.IsAny<User>())).Returns(false);

            UserController userController = new UserController(userStorageMock.Object, inputValidation.Object, businessValidation.Object);
            userController.ConfigureForTesting(HttpMethod.Post, "http://test.com/api/user");

            HttpResponseMessage response = userController.Post(user);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            userStorageMock.Verify(storage => storage.AddUser(It.IsAny<UserBll>()), Times.Never());
        }
    }
}
