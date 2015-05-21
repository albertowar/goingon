// ****************************************************************************
// <copyright file="UserDocumentDBStorageTests.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// Tests for User repository
// </summary>
// ****************************************************************************

namespace GoingOn.Repository.Tests
{
    using System;
    using System.Configuration;
    using GoingOn.Model.EntitiesBll;
    using GoingOn.Repository;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    // TODO: mock Store behaviour

    [TestClass]
    public class UserRepositoryTests
    {
        // RegistrationDate has to be initializated otherwise it will get the min value that crashes on TableStore
        private static readonly UserBll DefaultUser = new UserBll { Nickname = "nickname", Password = "password", City = "Malaga", BirthDate = new DateTime(2014, 12, 4), RegistrationDate = new DateTime(2014, 12, 4) };

        private IUserRepository repository;

        [TestInitialize]
        public void Initialize()
        {
            string connectionString = ConfigurationManager.AppSettings["StorageConnectionString"];
            string userTableName = ConfigurationManager.AppSettings["UserTableName"];

            this.repository = new UserTableRepository(connectionString, userTableName);
        }

        [TestCleanup]
        public void Cleanup()
        {
            this.repository.DeleteAllUsers(DefaultUser.City).Wait();
            this.repository.DeleteAllUsers("Dublin").Wait();
        }

        [TestMethod]
        public void TestContainsExistingUser()
        {
            this.repository.AddUser(DefaultUser).Wait();

            Assert.IsTrue(this.repository.ContainsUser(DefaultUser).Result);
        }

        [TestMethod]
        public void TestContainsNonExistingUser()
        {
            Assert.IsFalse(this.repository.ContainsUser(DefaultUser).Result);
        }

        [TestMethod]
        public void TestContainsUserIgnoresCity()
        {
            var user = new UserBll { Nickname = "nickname", Password = "password", City = "Malaga", RegistrationDate = DateTime.Today };

            this.repository.AddUser(user).Wait();

            var differentCityUser = new UserBll { Nickname = "nickname", Password = "password", City = "Dublin", RegistrationDate = DateTime.Today };

            Assert.IsTrue(this.repository.ContainsUser(user).Result);
            Assert.IsTrue(this.repository.ContainsUser(differentCityUser).Result);
        }
    }
}
