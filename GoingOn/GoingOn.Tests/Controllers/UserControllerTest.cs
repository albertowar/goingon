// ****************************************************************************
// <copyright file="UserControllerTest.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace GoingOn.Tests.Controllers
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Security.Principal;
    using System.Threading.Tasks;
    using System.Web.Http.Routing;
    
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Newtonsoft.Json;
    using WebApiContrib.Testing;

    using Common.Tests;
    using GoingOn.Entities;
    using GoingOn.Validation;
    using Model.EntitiesBll;
    using GoingOn.Controllers;
  
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
            inputValidation.Setup(validation => validation.IsValidNickName(It.IsAny<string>())).Returns(true);
            businessValidation.Setup(validation => validation.IsValidGetUser(userStorageMock.Object, It.IsAny<string>())).Returns(true);
            userStorageMock.Setup(storage => storage.GetUser(It.IsAny<string>())).Returns(Task.FromResult(User.ToUserBll(user)));

            UserController userController = new UserController(userStorageMock.Object, inputValidation.Object, businessValidation.Object);
            userController.ConfigureForTesting(HttpMethod.Get, "http://test.com/api/user/nickname", "DefaultApi", new HttpRoute("api/{controller}/{id}"));

            HttpResponseMessage response = userController.Get("nickname").Result;

            var content = response.Content;
            var jsonContent = content.ReadAsStringAsync().Result;
            User actualUser = JsonConvert.DeserializeObject<User>(jsonContent);
            
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(new UserCompleteEqualityComparer().Equals(user, actualUser));
            Assert.IsTrue(actualUser.Links.Any());
            Assert.AreEqual("self", actualUser.Links.First().Rel);
            Assert.AreEqual(new Uri("http://test.com/api/user/nickname"), actualUser.Links.First().Href);
        }

        [TestMethod]
        public void TestGetUserReturns400BadRequestWhenInputValidationFails()
        {
            inputValidation.Setup(validation => validation.IsValidNickName(It.IsAny<string>())).Returns(false);
            businessValidation.Setup(validation => validation.IsValidGetUser(userStorageMock.Object, It.IsAny<string>())).Returns(true);
            userStorageMock.Setup(storage => storage.GetUser(It.IsAny<string>())).Returns(Task.FromResult(new UserBll{ Nickname = "username", Password = "password" }));

            UserController userController = new UserController(userStorageMock.Object, inputValidation.Object, businessValidation.Object);
            userController.ConfigureForTesting(HttpMethod.Get, "http://test.com/api/user/nickname", "DefaultApi", new HttpRoute("api/{controller}/{id}"));

            HttpResponseMessage response = userController.Get("username").Result;

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public void TestGetUserReturns404NotFoundWhenBusinessValidationFails()
        {
            inputValidation.Setup(validation => validation.IsValidNickName(It.IsAny<string>())).Returns(true);
            businessValidation.Setup(validation => validation.IsValidGetUser(userStorageMock.Object, It.IsAny<string>())).Returns(false);
            userStorageMock.Setup(storage => storage.GetUser(It.IsAny<string>())).Returns(Task.FromResult(new UserBll { Nickname = "username", Password = "password" }));

            UserController userController = new UserController(userStorageMock.Object, inputValidation.Object, businessValidation.Object);
            userController.ConfigureForTesting(HttpMethod.Get, "http://test.com/api/user/nickname", "DefaultApi", new HttpRoute("api/{controller}/{id}"));

            HttpResponseMessage response = userController.Get("nickname").Result;

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public void TestPostUserReturns200OkWhenCreatesUser()
        {
            inputValidation.Setup(validation => validation.IsValidUser(It.IsAny<User>())).Returns(true);
            businessValidation.Setup(validation => validation.IsValidCreateUser(userStorageMock.Object, It.IsAny<User>())).Returns(true);

            UserController userController = new UserController(userStorageMock.Object, inputValidation.Object, businessValidation.Object);
            userController.ConfigureForTesting(HttpMethod.Post, "http://test.com/api/user", "DefaultApi", new HttpRoute("api/{controller}/{id}"));

            HttpResponseMessage response = userController.Post(user).Result;

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            Assert.AreEqual(new Uri("http://test.com/api/user/nickname"), response.Headers.Location);
            userStorageMock.Verify(storage => storage.AddUser(It.IsAny<UserBll>()), Times.Once());
        }

        [TestMethod]
        public void TestPostUserReturns400BadRequestWhenInputValidationFails()
        {
            inputValidation.Setup(validation => validation.IsValidUser(It.IsAny<User>())).Returns(false);
            businessValidation.Setup(validation => validation.IsValidCreateUser(userStorageMock.Object, It.IsAny<User>())).Returns(true);

            UserController userController = new UserController(userStorageMock.Object, inputValidation.Object, businessValidation.Object);
            userController.ConfigureForTesting(HttpMethod.Post, "http://test.com/api/user", "DefaultApi", new HttpRoute("api/{controller}/{id}"));

            HttpResponseMessage response = userController.Post(user).Result;

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            userStorageMock.Verify(storage => storage.AddUser(It.IsAny<UserBll>()), Times.Never());
        }

        [TestMethod]
        public void TestPostUserReturns400BadRequestWhenBusinessValidationFails()
        {
            inputValidation.Setup(validation => validation.IsValidUser(It.IsAny<User>())).Returns(true);
            businessValidation.Setup(validation => validation.IsValidCreateUser(userStorageMock.Object, It.IsAny<User>())).Returns(false);

            UserController userController = new UserController(userStorageMock.Object, inputValidation.Object, businessValidation.Object);
            userController.ConfigureForTesting(HttpMethod.Post, "http://test.com/api/user", "DefaultApi", new HttpRoute("api/{controller}/{id}"));

            HttpResponseMessage response = userController.Post(user).Result;

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            userStorageMock.Verify(storage => storage.AddUser(It.IsAny<UserBll>()), Times.Never());
        }

        [TestMethod]
        public void TestPatchUserReturns204NoContentWhenUpdatesUser()
        {
            inputValidation.Setup(validation => validation.IsValidNickName(It.IsAny<string>())).Returns(true);
            inputValidation.Setup(validation => validation.IsValidUser(It.IsAny<User>())).Returns(true);
            businessValidation.Setup(validation => validation.IsAuthorizedUser(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            businessValidation.Setup(validation => validation.IsValidUpdateUser(userStorageMock.Object, It.IsAny<User>())).Returns(true);

            UserController userController = new UserController(userStorageMock.Object, inputValidation.Object, businessValidation.Object);
            userController.ConfigureForTesting(new HttpMethod("PATCH"), "http://test.com/api/user/nickname", "DefaultApi", new HttpRoute("api/{controller}/{id}"));
            userController.User = new GenericPrincipal(new GenericIdentity(user.Nickname), null);

            HttpResponseMessage response = userController.Patch("username", user).Result;

            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
            userStorageMock.Verify(storage => storage.UpdateUser(It.IsAny<UserBll>()), Times.Once());
        }

        [TestMethod]
        public void TestPatchUserReturns400BadRequestWhenNicknameValidationFails()
        {
            inputValidation.Setup(validation => validation.IsValidNickName(It.IsAny<string>())).Returns(false);
            inputValidation.Setup(validation => validation.IsValidUser(It.IsAny<User>())).Returns(true);
            businessValidation.Setup(validation => validation.IsAuthorizedUser(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            businessValidation.Setup(validation => validation.IsValidUpdateUser(userStorageMock.Object, It.IsAny<User>())).Returns(true);

            UserController userController = new UserController(userStorageMock.Object, inputValidation.Object, businessValidation.Object);
            userController.ConfigureForTesting(new HttpMethod("PATCH"), "http://test.com/api/user/nickname", "DefaultApi", new HttpRoute("api/{controller}/{id}"));
            userController.User = new GenericPrincipal(new GenericIdentity(user.Nickname), null);

            HttpResponseMessage response = userController.Patch("username", user).Result;

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            userStorageMock.Verify(storage => storage.UpdateUser(It.IsAny<UserBll>()), Times.Never());
        }

        [TestMethod]
        public void TestPatchUserReturns400BadRequestWhenUserValidationFails()
        {
            inputValidation.Setup(validation => validation.IsValidNickName(It.IsAny<string>())).Returns(true);
            inputValidation.Setup(validation => validation.IsValidUser(It.IsAny<User>())).Returns(false);
            businessValidation.Setup(validation => validation.IsAuthorizedUser(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            businessValidation.Setup(validation => validation.IsValidUpdateUser(userStorageMock.Object, It.IsAny<User>())).Returns(true);

            UserController userController = new UserController(userStorageMock.Object, inputValidation.Object, businessValidation.Object);
            userController.ConfigureForTesting(new HttpMethod("PATCH"), "http://test.com/api/user/nickname", "DefaultApi", new HttpRoute("api/{controller}/{id}"));
            userController.User = new GenericPrincipal(new GenericIdentity(user.Nickname), null);

            HttpResponseMessage response = userController.Patch("username", user).Result;

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            userStorageMock.Verify(storage => storage.UpdateUser(It.IsAny<UserBll>()), Times.Never());
        }

        [TestMethod]
        public void TestPatchUserReturns404NotFoundWhenBusinessValidationFails()
        {
            inputValidation.Setup(validation => validation.IsValidNickName(It.IsAny<string>())).Returns(true);
            inputValidation.Setup(validation => validation.IsValidUser(It.IsAny<User>())).Returns(true);
            businessValidation.Setup(validation => validation.IsAuthorizedUser(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            businessValidation.Setup(validation => validation.IsValidUpdateUser(userStorageMock.Object, It.IsAny<User>())).Returns(false);

            UserController userController = new UserController(userStorageMock.Object, inputValidation.Object, businessValidation.Object);
            userController.ConfigureForTesting(new HttpMethod("PATCH"), "http://test.com/api/user/nickname", "DefaultApi", new HttpRoute("api/{controller}/{id}"));
            userController.User = new GenericPrincipal(new GenericIdentity(user.Nickname), null);

            HttpResponseMessage response = userController.Patch("username", user).Result;

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            userStorageMock.Verify(storage => storage.UpdateUser(It.IsAny<UserBll>()), Times.Never());
        }

        [TestMethod]
        public void TestPatchUserReturns401UnauthorizedWhenTheUserTryesToUpdateAnotherUser()
        {
            inputValidation.Setup(validation => validation.IsValidNickName(It.IsAny<string>())).Returns(true);
            inputValidation.Setup(validation => validation.IsValidUser(It.IsAny<User>())).Returns(true);
            businessValidation.Setup(validation => validation.IsAuthorizedUser(It.IsAny<string>(), It.IsAny<string>())).Returns(false);
            businessValidation.Setup(validation => validation.IsValidUpdateUser(userStorageMock.Object, It.IsAny<User>())).Returns(true);

            UserController userController = new UserController(userStorageMock.Object, inputValidation.Object, businessValidation.Object);
            userController.ConfigureForTesting(new HttpMethod("PATCH"), "http://test.com/api/user/nickname", "DefaultApi", new HttpRoute("api/{controller}/{id}"));
            userController.User = new GenericPrincipal(new GenericIdentity(user.Nickname), null);

            HttpResponseMessage response = userController.Patch("username", user).Result;

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
            userStorageMock.Verify(storage => storage.UpdateUser(It.IsAny<UserBll>()), Times.Never());
        }

        [TestMethod]
        public void TestDeleteUserReturns204NoContentWhenDeletesUser()
        {
            inputValidation.Setup(validation => validation.IsValidNickName(It.IsAny<string>())).Returns(true);
            businessValidation.Setup(validation => validation.IsAuthorizedUser(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            businessValidation.Setup(validation => validation.IsValidDeleteUser(userStorageMock.Object, It.IsAny<string>())).Returns(true);

            UserController userController = new UserController(userStorageMock.Object, inputValidation.Object, businessValidation.Object);
            userController.ConfigureForTesting(HttpMethod.Delete, "http://test.com/api/user/" + user.Nickname, "DefaultApi", new HttpRoute("api/{controller}/{id}"));
            userController.User = new GenericPrincipal(new GenericIdentity(user.Nickname), null); 

            HttpResponseMessage response = userController.Delete(user.Nickname).Result;

            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
            userStorageMock.Verify(storage => storage.DeleteUser(It.IsAny<UserBll>()), Times.Once());
        }

        [TestMethod]
        public void TestDeleteUserReturns400BadRequestWhenInputValidationFails()
        {
            inputValidation.Setup(validation => validation.IsValidNickName(It.IsAny<string>())).Returns(false);
            businessValidation.Setup(validation => validation.IsAuthorizedUser(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            businessValidation.Setup(validation => validation.IsValidDeleteUser(userStorageMock.Object, It.IsAny<string>())).Returns(true);

            UserController userController = new UserController(userStorageMock.Object, inputValidation.Object, businessValidation.Object);
            userController.ConfigureForTesting(HttpMethod.Delete, "http://test.com/api/user/" + user.Nickname, "DefaultApi", new HttpRoute("api/{controller}/{id}"));
            userController.User = new GenericPrincipal(new GenericIdentity(user.Nickname), null); 

            HttpResponseMessage response = userController.Delete(user.Nickname).Result;

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            userStorageMock.Verify(storage => storage.DeleteUser(It.IsAny<UserBll>()), Times.Never());
        }

        [TestMethod]
        public void TestDeleteUserReturns401UnauthorizedWhenTheUserTryesToDeleteAnotherUser()
        {
            inputValidation.Setup(validation => validation.IsValidNickName(It.IsAny<string>())).Returns(true);
            businessValidation.Setup(validation => validation.IsAuthorizedUser(It.IsAny<string>(), It.IsAny<string>())).Returns(false);
            businessValidation.Setup(validation => validation.IsValidDeleteUser(userStorageMock.Object, It.IsAny<string>())).Returns(true);

            UserController userController = new UserController(userStorageMock.Object, inputValidation.Object, businessValidation.Object);
            userController.ConfigureForTesting(HttpMethod.Delete, "http://test.com/api/user/" + user.Nickname, "DefaultApi", new HttpRoute("api/{controller}/{id}"));
            userController.User = new GenericPrincipal(new GenericIdentity(user.Nickname), null);

            HttpResponseMessage response = userController.Delete(user.Nickname).Result;

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
            userStorageMock.Verify(storage => storage.DeleteUser(It.IsAny<UserBll>()), Times.Never());
        }

        [TestMethod]
        public void TestDeleteUserReturns404NotFoundWhenTheUserIsNotInTheDatabase()
        {
            inputValidation.Setup(validation => validation.IsValidNickName(It.IsAny<string>())).Returns(true);
            businessValidation.Setup(validation => validation.IsAuthorizedUser(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            businessValidation.Setup(validation => validation.IsValidDeleteUser(userStorageMock.Object, It.IsAny<string>())).Returns(false);

            UserController userController = new UserController(userStorageMock.Object, inputValidation.Object, businessValidation.Object);
            userController.ConfigureForTesting(HttpMethod.Delete, "http://test.com/api/user/" + user.Nickname, "DefaultApi", new HttpRoute("api/{controller}/{id}"));
            userController.User = new GenericPrincipal(new GenericIdentity(user.Nickname), null); 

            HttpResponseMessage response = userController.Delete(user.Nickname).Result;

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            userStorageMock.Verify(storage => storage.DeleteUser(It.IsAny<UserBll>()), Times.Never());
        }
    }
}
