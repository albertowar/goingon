// ****************************************************************************
// <copyright file="ApiBusinessLogicValidationChecksTests.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

using System.Threading.Tasks;

namespace GoingOn.Tests.Validation
{
    using GoingOn.Entities;
    using GoingOn.Validation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Model.EntitiesBll;
    using Moq;

    [TestClass]
    public class ApiBusinessLogicValidationChecksTests
    {
        private Mock<IUserStorage> storageMock;
        private ApiBusinessLogicValidationChecks businessValidation;

        private static readonly User user = new User("nickname", "password");

        [TestInitialize]
        public void Initialize()
        {
            storageMock = new Mock<IUserStorage>();
            businessValidation = new ApiBusinessLogicValidationChecks();
        }

        [TestMethod]
        public void TestIsValidCreateUserSucceedsIfStorageDoesNotContainsTheUser()
        {
            storageMock.Setup(storage => storage.ContainsUser(It.IsAny<UserBll>())).Returns(Task.FromResult(false));

            Assert.IsTrue(businessValidation.IsValidCreateUser(storageMock.Object, user));
        }

        [TestMethod]
        public void TestIsValidCreateUserFailsIfStorageContainsTheUser()
        {
            storageMock.Setup(storage => storage.ContainsUser(It.IsAny<UserBll>())).Returns(Task.FromResult(true));

            Assert.IsFalse(businessValidation.IsValidCreateUser(storageMock.Object, user));
        }
    }
}
