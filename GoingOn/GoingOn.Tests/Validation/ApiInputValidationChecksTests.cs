// ****************************************************************************
// <copyright file="ApiInputValidationChecksTests.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

using System;
using Model.EntitiesBll;
using Moq;

namespace GoingOn.Tests.Validation
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using GoingOn.Entities;
    using GoingOn.Validation;

    [TestClass]
    public class ApiInputValidationChecksTests
    {
        private ApiInputValidationChecks inputValidation;

        [TestInitialize]
        public void Initialize()
        {
            inputValidation = new ApiInputValidationChecks();
        }

        [TestMethod]
        public void TestIsValidUserSucceedsWithWellFormedUser()
        {
            User user = new User("nickname", "password");

            Assert.IsTrue(inputValidation.IsValidUser(user));
        }

        [TestMethod]
        public void TestIsValidUserFailsWithNullUser()
        {
            Assert.IsFalse(inputValidation.IsValidUser(null));
        }

        [TestMethod]
        public void TestIsValidUserFailsWithWrongNickname()
        {
            User nullNickNameUser = new User(null, "password");
            User emptyNickNameUser = new User(string.Empty, "password");
            User whiteSpaceNickNameUser = new User(" \n\t", "password");

            Assert.IsFalse(inputValidation.IsValidUser(nullNickNameUser));
            Assert.IsFalse(inputValidation.IsValidUser(emptyNickNameUser));
            Assert.IsFalse(inputValidation.IsValidUser(whiteSpaceNickNameUser));
        }

        [TestMethod]
        public void TestIsValidUserFailsWithWrongPassword()
        {
            User nullPasswordUser = new User("username", null);
            User emptyPasswordUser = new User("username", string.Empty);
            User whiteSpacePasswordUser = new User("username", " \n\t");

            Assert.IsFalse(inputValidation.IsValidUser(nullPasswordUser));
            Assert.IsFalse(inputValidation.IsValidUser(emptyPasswordUser));
            Assert.IsFalse(inputValidation.IsValidUser(whiteSpacePasswordUser));
        }

        [TestMethod]
        public void TestIsValidNewsFailsWithNullUser()
        {
            Assert.IsFalse(inputValidation.IsValidNews(null));
        }

        [TestMethod]
        public void TestIsValidNewsFailsWithWrongTitle()
        {
            News nullTitleNews = new News(null, "content");
            News emptyTitleNews = new News(string.Empty, "content");
            News whiteSpaceTitleNews = new News(" \n\t", "content");

            Assert.IsFalse(inputValidation.IsValidNews(nullTitleNews));
            Assert.IsFalse(inputValidation.IsValidNews(emptyTitleNews));
            Assert.IsFalse(inputValidation.IsValidNews(whiteSpaceTitleNews));
        }

        [TestMethod]
        public void TestIsValidNewsFailsWithWrongContent()
        {
            News nullTitleNews = new News("title", null);
            News emptyTitleNews = new News("title", string.Empty);
            News whiteSpaceTitleNews = new News("title", " \n\t");

            Assert.IsFalse(inputValidation.IsValidNews(nullTitleNews));
            Assert.IsFalse(inputValidation.IsValidNews(emptyTitleNews));
            Assert.IsFalse(inputValidation.IsValidNews(whiteSpaceTitleNews));
        }
    }
}
