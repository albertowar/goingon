// ****************************************************************************
// <copyright file="UserControllerTest.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
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
    
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Newtonsoft.Json;
    using WebApiContrib.Testing;

    using GoingOn.FrontendWebRole.Controllers;
    using GoingOn.Common;
    using GoingOn.Common.Tests;
    using GoingOn.Frontend.Entities;
    using GoingOn.Frontend.Validation;
    using Model.EntitiesBll;
    using Storage;

    [TestClass]
    public class UserControllerTest
    {
        private Mock<IUserStorage> userStorageMock;
        private Mock<IApiInputValidationChecks> inputValidation;
        private Mock<IApiBusinessLogicValidationChecks> businessValidation;

        private static readonly User User = new User { Nickname = "nickname", Password = "password", City = "Malaga" };

        [TestInitialize]
        public void Initizalize()
        {
            this.userStorageMock = new Mock<IUserStorage>();
            this.inputValidation = new Mock<IApiInputValidationChecks>();
            this.businessValidation = new Mock<IApiBusinessLogicValidationChecks>();
        }

        [TestMethod]
        public void TestGetUserReturns200OkWhenTheUserIsInTheDatabase()
        {
            this.inputValidation.Setup(validation => validation.IsValidNickName(It.IsAny<string>())).Returns(true);
            this.businessValidation.Setup(validation => validation.IsValidGetUser(this.userStorageMock.Object, It.IsAny<string>())).Returns(Task.FromResult(true));
            this.userStorageMock.Setup(storage => storage.GetUser(It.IsAny<string>())).Returns(Task.FromResult(User.ToUserBll(User)));

            var userController = new UserController(this.userStorageMock.Object, this.inputValidation.Object, this.businessValidation.Object);
            userController.ConfigureForTesting(HttpMethod.Get, "http://test.com/api/user/nickname", "GetUser", new HttpRoute(GOUriBuilder.GetUserTemplate));

            HttpResponseMessage response = userController.Get("nickname").Result;

            var content = response.Content;
            var jsonContent = content.ReadAsStringAsync().Result;
            var actualUser = JsonConvert.DeserializeObject<UserREST>(jsonContent);
            
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(new UserCompleteEqualityComparer().Equals(User, actualUser.User));
            Assert.IsTrue(actualUser.Links.Any());
            Assert.AreEqual("self", actualUser.Links.First().Rel);
            Assert.AreEqual(new Uri("http://test.com/api/user/nickname"), actualUser.Links.First().Href);
        }

        [TestMethod]
        public void TestGetUserReturns400BadRequestWhenInputValidationFails()
        {
            this.inputValidation.Setup(validation => validation.IsValidNickName(It.IsAny<string>())).Returns(false);
            this.businessValidation.Setup(validation => validation.IsValidGetUser(this.userStorageMock.Object, It.IsAny<string>())).Returns(Task.FromResult(true));
            this.userStorageMock.Setup(storage => storage.GetUser(It.IsAny<string>())).Returns(Task.FromResult(new UserBll{ Nickname = "username", Password = "password" }));

            this.AssertGetFails(url: "http://test.com/api/user/nickname", nickname: "username", resultCode: HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void TestGetUserReturns404NotFoundWhenBusinessValidationFails()
        {
            this.inputValidation.Setup(validation => validation.IsValidNickName(It.IsAny<string>())).Returns(true);
            this.businessValidation.Setup(validation => validation.IsValidGetUser(this.userStorageMock.Object, It.IsAny<string>())).Returns(Task.FromResult(false));
            this.userStorageMock.Setup(storage => storage.GetUser(It.IsAny<string>())).Returns(Task.FromResult(new UserBll { Nickname = "username", Password = "password" }));

            this.AssertGetFails(url: "http://test.com/api/user/nickname", nickname: "username", resultCode: HttpStatusCode.NotFound);
        }

        [TestMethod]
        public void TestPostUserReturns200OkWhenCreatesUser()
        {
            this.inputValidation.Setup(validation => validation.IsValidUser(It.IsAny<User>())).Returns(true);
            this.businessValidation.Setup(validation => validation.IsValidCreateUser(this.userStorageMock.Object, It.IsAny<User>())).Returns(Task.FromResult(true));

            UserController userController = new UserController(this.userStorageMock.Object, this.inputValidation.Object, this.businessValidation.Object);
            userController.ConfigureForTesting(HttpMethod.Post, "http://test.com/api/user", "GetUser", new HttpRoute(GOUriBuilder.GetUserTemplate));

            HttpResponseMessage response = userController.Post(User).Result;

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            Assert.AreEqual(new Uri("http://test.com/api/user/nickname"), response.Headers.Location);
            this.userStorageMock.Verify(storage => storage.AddUser(It.IsAny<UserBll>()), Times.Once());
        }

        [TestMethod]
        public void TestPostUserReturns400BadRequestWhenInputValidationFails()
        {
            this.inputValidation.Setup(validation => validation.IsValidUser(It.IsAny<User>())).Returns(false);
            this.businessValidation.Setup(validation => validation.IsValidCreateUser(this.userStorageMock.Object, It.IsAny<User>())).Returns(Task.FromResult(true));

            this.AssertPostFails(url: "http://test.com/api/user/", user: User, resultCode: HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void TestPostUserReturns400BadRequestWhenBusinessValidationFails()
        {
            this.inputValidation.Setup(validation => validation.IsValidUser(It.IsAny<User>())).Returns(true);
            this.businessValidation.Setup(validation => validation.IsValidCreateUser(this.userStorageMock.Object, It.IsAny<User>())).Returns(Task.FromResult(false));

            this.AssertPostFails(url: "http://test.com/api/user/", user: User, resultCode: HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void TestPatchUserReturns204NoContentWhenUpdatesUser()
        {
            this.inputValidation.Setup(validation => validation.IsValidNickName(It.IsAny<string>())).Returns(true);
            this.inputValidation.Setup(validation => validation.IsValidUser(It.IsAny<User>())).Returns(true);
            this.businessValidation.Setup(validation => validation.IsAuthorizedUser(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            this.businessValidation.Setup(validation => validation.IsValidUpdateUser(this.userStorageMock.Object, It.IsAny<User>())).Returns(Task.FromResult(true));

            UserController userController = new UserController(this.userStorageMock.Object, this.inputValidation.Object, this.businessValidation.Object);
            userController.ConfigureForTesting(new HttpMethod("PATCH"), "http://test.com/api/user/nickname", "GetUser", new HttpRoute(GOUriBuilder.GetUserTemplate));
            userController.User = new GenericPrincipal(new GenericIdentity(User.Nickname), null);

            HttpResponseMessage response = userController.Patch("username", User).Result;

            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
            this.userStorageMock.Verify(storage => storage.UpdateUser(It.IsAny<UserBll>()), Times.Once());
        }

        [TestMethod]
        public void TestPatchUserReturns400BadRequestWhenNicknameValidationFails()
        {
            this.inputValidation.Setup(validation => validation.IsValidNickName(It.IsAny<string>())).Returns(false);
            this.inputValidation.Setup(validation => validation.IsValidUser(It.IsAny<User>())).Returns(true);
            this.businessValidation.Setup(validation => validation.IsAuthorizedUser(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            this.businessValidation.Setup(validation => validation.IsValidUpdateUser(this.userStorageMock.Object, It.IsAny<User>())).Returns(Task.FromResult(true));

            this.AssertPatchFails(nickname: "username", url: "http://test.com/api/user/nickname", user: User, resultCode: HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void TestPatchUserReturns400BadRequestWhenUserValidationFails()
        {
            this.inputValidation.Setup(validation => validation.IsValidNickName(It.IsAny<string>())).Returns(true);
            this.inputValidation.Setup(validation => validation.IsValidUser(It.IsAny<User>())).Returns(false);
            this.businessValidation.Setup(validation => validation.IsAuthorizedUser(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            this.businessValidation.Setup(validation => validation.IsValidUpdateUser(this.userStorageMock.Object, It.IsAny<User>())).Returns(Task.FromResult(true));

            this.AssertPatchFails(nickname: "nickname", url: "http://test.com/api/user/nickname", user: User, resultCode: HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void TestPatchUserReturns404NotFoundWhenBusinessValidationFails()
        {
            this.inputValidation.Setup(validation => validation.IsValidNickName(It.IsAny<string>())).Returns(true);
            this.inputValidation.Setup(validation => validation.IsValidUser(It.IsAny<User>())).Returns(true);
            this.businessValidation.Setup(validation => validation.IsAuthorizedUser(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            this.businessValidation.Setup(validation => validation.IsValidUpdateUser(this.userStorageMock.Object, It.IsAny<User>())).Returns(Task.FromResult(false));

            this.AssertPatchFails(nickname: "nickname", url: "http://test.com/api/user/nickname", user: User, resultCode: HttpStatusCode.NotFound);
        }

        [TestMethod]
        public void TestPatchUserReturns401UnauthorizedWhenTheUserTryesToUpdateAnotherUser()
        {
            this.inputValidation.Setup(validation => validation.IsValidNickName(It.IsAny<string>())).Returns(true);
            this.inputValidation.Setup(validation => validation.IsValidUser(It.IsAny<User>())).Returns(true);
            this.businessValidation.Setup(validation => validation.IsAuthorizedUser(It.IsAny<string>(), It.IsAny<string>())).Returns(false);
            this.businessValidation.Setup(validation => validation.IsValidUpdateUser(this.userStorageMock.Object, It.IsAny<User>())).Returns(Task.FromResult(true));

            this.AssertPatchFails(nickname: "username", url: "http://test.com/api/user/nickname", user: User, resultCode: HttpStatusCode.Unauthorized);
        }

        [TestMethod]
        public void TestDeleteUserReturns204NoContentWhenDeletesUser()
        {
            this.inputValidation.Setup(validation => validation.IsValidNickName(It.IsAny<string>())).Returns(true);
            this.businessValidation.Setup(validation => validation.IsAuthorizedUser(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            this.businessValidation.Setup(validation => validation.IsValidDeleteUser(this.userStorageMock.Object, It.IsAny<string>())).Returns(Task.FromResult(true));

            UserController userController = new UserController(this.userStorageMock.Object, this.inputValidation.Object, this.businessValidation.Object);
            userController.ConfigureForTesting(HttpMethod.Delete, "http://test.com/api/user/" + User.Nickname, "GetUser", new HttpRoute(GOUriBuilder.GetUserTemplate));
            userController.User = new GenericPrincipal(new GenericIdentity(User.Nickname), null); 

            HttpResponseMessage response = userController.Delete(User.Nickname).Result;

            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
            this.userStorageMock.Verify(storage => storage.DeleteUser(It.IsAny<UserBll>()), Times.Once());
        }

        [TestMethod]
        public void TestDeleteUserReturns400BadRequestWhenInputValidationFails()
        {
            this.inputValidation.Setup(validation => validation.IsValidNickName(It.IsAny<string>())).Returns(false);
            this.businessValidation.Setup(validation => validation.IsAuthorizedUser(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            this.businessValidation.Setup(validation => validation.IsValidDeleteUser(this.userStorageMock.Object, It.IsAny<string>())).Returns(Task.FromResult(true));

            this.AssertDeleteFails(nickname: User.Nickname, url: "http://test.com/api/user/" + User.Nickname, resultCode: HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void TestDeleteUserReturns401UnauthorizedWhenTheUserTryesToDeleteAnotherUser()
        {
            this.inputValidation.Setup(validation => validation.IsValidNickName(It.IsAny<string>())).Returns(true);
            this.businessValidation.Setup(validation => validation.IsAuthorizedUser(It.IsAny<string>(), It.IsAny<string>())).Returns(false);
            this.businessValidation.Setup(validation => validation.IsValidDeleteUser(this.userStorageMock.Object, It.IsAny<string>())).Returns(Task.FromResult(true));

            this.AssertDeleteFails(nickname: User.Nickname, url: "http://test.com/api/user/" + User.Nickname, resultCode: HttpStatusCode.Unauthorized);
        }

        [TestMethod]
        public void TestDeleteUserReturns404NotFoundWhenTheUserIsNotInTheDatabase()
        {
            this.inputValidation.Setup(validation => validation.IsValidNickName(It.IsAny<string>())).Returns(true);
            this.businessValidation.Setup(validation => validation.IsAuthorizedUser(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            this.businessValidation.Setup(validation => validation.IsValidDeleteUser(this.userStorageMock.Object, It.IsAny<string>())).Returns(Task.FromResult(false));

            this.AssertDeleteFails(nickname: User.Nickname, url: "http://test.com/api/user/" + User.Nickname, resultCode: HttpStatusCode.NotFound);
        }

        #region Assert helper methods

        private void AssertGetFails(string url, string nickname, HttpStatusCode resultCode)
        {
            UserController userController = new UserController(this.userStorageMock.Object, this.inputValidation.Object, this.businessValidation.Object);
            userController.ConfigureForTesting(HttpMethod.Get, url, "GetUser", new HttpRoute(GOUriBuilder.GetUserTemplate));

            HttpResponseMessage response = userController.Get(nickname).Result;

            Assert.AreEqual(resultCode, response.StatusCode);
            this.userStorageMock.Verify(storage => storage.GetUser(It.IsAny<string>()), Times.Never());
        }

        private void AssertPostFails(string url, User user, HttpStatusCode resultCode)
        {
            UserController userController = new UserController(this.userStorageMock.Object, this.inputValidation.Object, this.businessValidation.Object);
            userController.ConfigureForTesting(HttpMethod.Post, url, "GetUser", new HttpRoute(GOUriBuilder.GetUserTemplate));

            HttpResponseMessage response = userController.Post(user).Result;

            Assert.AreEqual(resultCode, response.StatusCode);
            this.userStorageMock.Verify(storage => storage.AddUser(It.IsAny<UserBll>()), Times.Never());
        }

        private void AssertPatchFails(string nickname, string url, User user, HttpStatusCode resultCode)
        {
            UserController userController = new UserController(this.userStorageMock.Object, this.inputValidation.Object, this.businessValidation.Object);
            userController.ConfigureForTesting(new HttpMethod("PATCH"), url, "GetUser", new HttpRoute(GOUriBuilder.GetUserTemplate));
            userController.User = new GenericPrincipal(new GenericIdentity(user.Nickname), null);

            HttpResponseMessage response = userController.Patch(nickname, user).Result;

            Assert.AreEqual(resultCode, response.StatusCode);
            this.userStorageMock.Verify(storage => storage.UpdateUser(It.IsAny<UserBll>()), Times.Never());
        }

        private void AssertDeleteFails(string nickname, string url, HttpStatusCode resultCode)
        {
            UserController userController = new UserController(this.userStorageMock.Object, this.inputValidation.Object, this.businessValidation.Object);
            userController.ConfigureForTesting(HttpMethod.Delete, url, "GetUser", new HttpRoute(GOUriBuilder.GetUserTemplate));
            userController.User = new GenericPrincipal(new GenericIdentity(User.Nickname), null);

            HttpResponseMessage response = userController.Delete(nickname).Result;

            Assert.AreEqual(resultCode, response.StatusCode);
            this.userStorageMock.Verify(storage => storage.DeleteUser(It.IsAny<UserBll>()), Times.Never());
        }

        #endregion
    }
}
