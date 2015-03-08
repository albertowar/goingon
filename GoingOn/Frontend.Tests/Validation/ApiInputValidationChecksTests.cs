// ****************************************************************************
// <copyright file="ApiInputValidationChecksTests.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace GoingOn.Tests.Validation
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using Frontend.Entities;
    using Frontend.Validation;

    [TestClass]
    public class ApiInputValidationChecksTests
    {
        private ApiInputValidationChecks inputValidation;
        private Mock<IApiInputValidationChecks> mockInputValidation;

        [TestInitialize]
        public void Initialize()
        {
            mockInputValidation = new Mock<IApiInputValidationChecks>();
            inputValidation = new ApiInputValidationChecks(mockInputValidation.Object);
        }

        [TestMethod]
        public void TestIsValidUserSucceedsWithWellFormedUser()
        {
            User user = new User();

            mockInputValidation.Setup(iv => iv.IsValidNickName(It.IsAny<string>())).Returns(true);
            mockInputValidation.Setup(iv => iv.IsValidPassword(It.IsAny<string>())).Returns(true);
            mockInputValidation.Setup(iv => iv.IsValidCity(It.IsAny<string>())).Returns(true);
            mockInputValidation.Setup(iv => iv.IsValidName(It.IsAny<string>())).Returns(true);
            mockInputValidation.Setup(iv => iv.IsValidEmail(It.IsAny<string>())).Returns(true);
            mockInputValidation.Setup(iv => iv.IsValidBirthDate(It.IsAny<DateTime>())).Returns(true);

            Assert.IsTrue(inputValidation.IsValidUser(user));
        }

        [TestMethod]
        public void TestIsValidUserFailsWithNullUser()
        {
            Assert.IsFalse(inputValidation.IsValidUser(null));
        }

        [TestMethod]
        public void TestIsValidNickname()
        {
            Assert.IsTrue(inputValidation.IsValidNickName("nickname"));
            Assert.IsFalse(inputValidation.IsValidNickName(null));
            Assert.IsFalse(inputValidation.IsValidNickName(string.Empty));
            Assert.IsFalse(inputValidation.IsValidNickName(" \n\t"));   
        }

        [TestMethod]
        public void TestIsValidUserFailsWithIfWhenIsValidNicknameFails()
        {
            mockInputValidation.Setup(iv => iv.IsValidNickName(It.IsAny<string>())).Returns(false);
            mockInputValidation.Setup(iv => iv.IsValidPassword(It.IsAny<string>())).Returns(true);
            mockInputValidation.Setup(iv => iv.IsValidCity(It.IsAny<string>())).Returns(true);
            mockInputValidation.Setup(iv => iv.IsValidName(It.IsAny<string>())).Returns(true);
            mockInputValidation.Setup(iv => iv.IsValidEmail(It.IsAny<string>())).Returns(true);
            mockInputValidation.Setup(iv => iv.IsValidBirthDate(It.IsAny<DateTime>())).Returns(true);

            Assert.IsFalse(this.inputValidation.IsValidUser(It.IsAny<User>()));
        }

        [TestMethod]
        public void TestIsValidPassword()
        {
            Assert.IsTrue(inputValidation.IsValidPassword("password"));
            Assert.IsFalse(inputValidation.IsValidPassword(null));
            Assert.IsFalse(inputValidation.IsValidPassword(string.Empty));
            Assert.IsFalse(inputValidation.IsValidPassword(" \n\t"));   
        }

        [TestMethod]
        public void TestIsValidUserFailsWithIfWhenIsValidPasswordFails()
        {
            mockInputValidation.Setup(iv => iv.IsValidNickName(It.IsAny<string>())).Returns(true);
            mockInputValidation.Setup(iv => iv.IsValidPassword(It.IsAny<string>())).Returns(false);
            mockInputValidation.Setup(iv => iv.IsValidCity(It.IsAny<string>())).Returns(true);
            mockInputValidation.Setup(iv => iv.IsValidName(It.IsAny<string>())).Returns(true);
            mockInputValidation.Setup(iv => iv.IsValidEmail(It.IsAny<string>())).Returns(true);
            mockInputValidation.Setup(iv => iv.IsValidBirthDate(It.IsAny<DateTime>())).Returns(true);

            Assert.IsFalse(this.inputValidation.IsValidUser(It.IsAny<User>()));
        }

        [TestMethod]
        public void TestIsValidCity()
        {
            Assert.IsTrue(inputValidation.IsValidCity("Malaga"));
            Assert.IsTrue(inputValidation.IsValidCity("Granada"));
            Assert.IsTrue(inputValidation.IsValidCity("Sevilla"));
            Assert.IsTrue(inputValidation.IsValidCity("Cadiz"));
            Assert.IsTrue(inputValidation.IsValidCity("Almeria"));
            Assert.IsTrue(inputValidation.IsValidCity("Cordoba"));
            Assert.IsTrue(inputValidation.IsValidCity("Huelva"));
            Assert.IsFalse(inputValidation.IsValidCity(null));
            Assert.IsFalse(inputValidation.IsValidCity(string.Empty));
            Assert.IsFalse(inputValidation.IsValidCity(" \n\t"));
        }

        [TestMethod]
        public void TestIsValidUserFailsWithIfWhenIsValidCityFails()
        {
            mockInputValidation.Setup(iv => iv.IsValidNickName(It.IsAny<string>())).Returns(true);
            mockInputValidation.Setup(iv => iv.IsValidPassword(It.IsAny<string>())).Returns(true);
            mockInputValidation.Setup(iv => iv.IsValidCity(It.IsAny<string>())).Returns(false);
            mockInputValidation.Setup(iv => iv.IsValidName(It.IsAny<string>())).Returns(true);
            mockInputValidation.Setup(iv => iv.IsValidEmail(It.IsAny<string>())).Returns(true);
            mockInputValidation.Setup(iv => iv.IsValidBirthDate(It.IsAny<DateTime>())).Returns(true);

            Assert.IsFalse(this.inputValidation.IsValidUser(It.IsAny<User>()));
        }

        [TestMethod]
        public void TestIsValidName()
        {
            Assert.IsTrue(inputValidation.IsValidName(null));
            Assert.IsTrue(inputValidation.IsValidName("Alberto"));
            Assert.IsFalse(inputValidation.IsValidName(string.Empty));
            Assert.IsFalse(inputValidation.IsValidName(" \n\t"));
        }

        [TestMethod]
        public void TestIsValidUserFailsWithIfWhenIsValidNameFails()
        {
            mockInputValidation.Setup(iv => iv.IsValidNickName(It.IsAny<string>())).Returns(true);
            mockInputValidation.Setup(iv => iv.IsValidPassword(It.IsAny<string>())).Returns(true);
            mockInputValidation.Setup(iv => iv.IsValidCity(It.IsAny<string>())).Returns(true);
            mockInputValidation.Setup(iv => iv.IsValidName(It.IsAny<string>())).Returns(false);
            mockInputValidation.Setup(iv => iv.IsValidEmail(It.IsAny<string>())).Returns(true);
            mockInputValidation.Setup(iv => iv.IsValidBirthDate(It.IsAny<DateTime>())).Returns(true);

            Assert.IsFalse(this.inputValidation.IsValidUser(It.IsAny<User>()));
        }

        [TestMethod]
        public void TestIsValidEmail()
        {
            Assert.IsTrue(inputValidation.IsValidEmail("alberto@gmail.com"));
            Assert.IsFalse(inputValidation.IsValidEmail("something else"));
        }

        [TestMethod]
        public void TestIsValidUserFailsWithIfWhenIsValidEmailFails()
        {
            mockInputValidation.Setup(iv => iv.IsValidNickName(It.IsAny<string>())).Returns(true);
            mockInputValidation.Setup(iv => iv.IsValidPassword(It.IsAny<string>())).Returns(true);
            mockInputValidation.Setup(iv => iv.IsValidCity(It.IsAny<string>())).Returns(true);
            mockInputValidation.Setup(iv => iv.IsValidName(It.IsAny<string>())).Returns(true);
            mockInputValidation.Setup(iv => iv.IsValidEmail(It.IsAny<string>())).Returns(false);
            mockInputValidation.Setup(iv => iv.IsValidBirthDate(It.IsAny<DateTime>())).Returns(true);

            Assert.IsFalse(this.inputValidation.IsValidUser(It.IsAny<User>()));
        }

        [TestMethod]
        public void TestIsValidUserSucceedsWithAnyBirthDate()
        {
            Assert.IsTrue(inputValidation.IsValidBirthDate(It.IsAny<DateTime>()));
        }

        [TestMethod]
        public void TestIsValidUserFailsWithIfWhenIsValidBirthDateFails()
        {
            mockInputValidation.Setup(iv => iv.IsValidNickName(It.IsAny<string>())).Returns(true);
            mockInputValidation.Setup(iv => iv.IsValidPassword(It.IsAny<string>())).Returns(true);
            mockInputValidation.Setup(iv => iv.IsValidCity(It.IsAny<string>())).Returns(true);
            mockInputValidation.Setup(iv => iv.IsValidName(It.IsAny<string>())).Returns(true);
            mockInputValidation.Setup(iv => iv.IsValidEmail(It.IsAny<string>())).Returns(true);
            mockInputValidation.Setup(iv => iv.IsValidBirthDate(It.IsAny<DateTime>())).Returns(false);

            Assert.IsFalse(this.inputValidation.IsValidUser(It.IsAny<User>()));
        }

        [TestMethod]
        public void TestIsValidNewsFailsWithNullUser()
        {
            Assert.IsFalse(inputValidation.IsValidNews(null));
        }

        [TestMethod]
        public void TestIsValidNewsFailsWithWrongTitle()
        {
            var nullTitleNews = new News { Title = null, Content = "content" };
            var emptyTitleNews = new News { Title = string.Empty, Content = "content" };
            var whiteSpaceTitleNews = new News { Title = " \n\t", Content = "content" };

            Assert.IsFalse(inputValidation.IsValidNews(nullTitleNews));
            Assert.IsFalse(inputValidation.IsValidNews(emptyTitleNews));
            Assert.IsFalse(inputValidation.IsValidNews(whiteSpaceTitleNews));
        }

        [TestMethod]
        public void TestIsValidNewsFailsWithWrongContent()
        {
            var nullContent = new News { Title = "title", Content = null };
            var emptyContentNews = new News { Title = "title", Content = string.Empty };
            var whiteSpaceContentNews = new News { Title = "title", Content = " \n\t" };

            Assert.IsFalse(inputValidation.IsValidNews(nullContent));
            Assert.IsFalse(inputValidation.IsValidNews(emptyContentNews));
            Assert.IsFalse(inputValidation.IsValidNews(whiteSpaceContentNews));
        }
    }
}
