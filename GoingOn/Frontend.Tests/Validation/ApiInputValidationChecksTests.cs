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
using Frontend.Entities;
using Frontend.Validation;
using Model.EntitiesBll;
using Moq;

namespace GoingOn.Tests.Validation
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            var user = new User { Nickname = "nickname", Password = "password", City = "Malaga" };

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
            var nullNickNameUser = new User { Nickname = null, Password = "password", City = "Malaga" }; 
            var emptyNickNameUser = new User { Nickname = string.Empty, Password = "password", City = "Malaga" }; 
            var whiteSpaceNickNameUser = new User { Nickname = " \n\t", Password = "password", City = "Malaga" }; 

            Assert.IsFalse(inputValidation.IsValidUser(nullNickNameUser));
            Assert.IsFalse(inputValidation.IsValidUser(emptyNickNameUser));
            Assert.IsFalse(inputValidation.IsValidUser(whiteSpaceNickNameUser));
        }

        [TestMethod]
        public void TestIsValidUserFailsWithWrongPassword()
        {
            var nullPasswordUser = new User { Nickname = "nickname", Password = null, City = "Malaga" };
            var emptyPasswordUser = new User { Nickname = "nickname", Password = string.Empty, City = "Malaga" };
            var whiteSpacePasswordUser = new User { Nickname = "nickname", Password = " \n\t", City = "Malaga" };

            Assert.IsFalse(inputValidation.IsValidUser(nullPasswordUser));
            Assert.IsFalse(inputValidation.IsValidUser(emptyPasswordUser));
            Assert.IsFalse(inputValidation.IsValidUser(whiteSpacePasswordUser));
        }

        [TestMethod]
        public void TestIsValidUserFailsWithWrongCity()
        {
            var nullPasswordUser = new User { Nickname = "nickname", Password = "password", City = null };
            var emptyPasswordUser = new User { Nickname = "nickname", Password = "password", City = string.Empty };
            var whiteSpacePasswordUser = new User { Nickname = "nickname", Password = "password", City = " \n\t" };

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
            var nullTitleNews = new News(null, "content");
            var emptyTitleNews = new News(string.Empty, "content");
            var whiteSpaceTitleNews = new News(" \n\t", "content");

            Assert.IsFalse(inputValidation.IsValidNews(nullTitleNews));
            Assert.IsFalse(inputValidation.IsValidNews(emptyTitleNews));
            Assert.IsFalse(inputValidation.IsValidNews(whiteSpaceTitleNews));
        }

        [TestMethod]
        public void TestIsValidNewsFailsWithWrongContent()
        {
            var nullContent = new News("title", null);
            var emptyContentNews = new News("title", string.Empty);
            var whiteSpaceContentNews = new News("title", " \n\t");

            Assert.IsFalse(inputValidation.IsValidNews(nullContent));
            Assert.IsFalse(inputValidation.IsValidNews(emptyContentNews));
            Assert.IsFalse(inputValidation.IsValidNews(whiteSpaceContentNews));
        }
    }
}
