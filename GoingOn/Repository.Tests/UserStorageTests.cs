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
    using System.Threading.Tasks;
    using GoingOn.Common.Tests;
    using GoingOn.Model.EntitiesBll;
    using GoingOn.Repository;
    using GoingOn.XStoreProxy;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Storage;

    [TestClass]
    public class UserStorageTests
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
        public void TestAddUser()
        {
            this.repository.AddUser(DefaultUser).Wait();

            Task<bool> containsUserTask = this.repository.ContainsUser(DefaultUser);
            containsUserTask.Wait();

            Assert.IsTrue(containsUserTask.Result);
        }

        [TestMethod]
        public void TestAddExistingUser()
        {
            this.repository.AddUser(DefaultUser).Wait();

            AssertExtensions.Throws<StorageException>(() => this.repository.AddUser(DefaultUser).Wait());
        }

        [TestMethod]
        public void TestGetUser()
        {
            this.repository.AddUser(DefaultUser).Wait();

            UserBll actualUser = this.repository.GetUser(DefaultUser.City, DefaultUser.Nickname).Result;

            Assert.IsTrue(new UserBllEqualityComparer().Equals(DefaultUser, actualUser));
        }

        [TestMethod]
        public void TestGetNonExistingUser()
        {
            AssertExtensions.Throws<AzureTableStorageException>(() => this.repository.GetUser(DefaultUser.City, DefaultUser.Nickname).Wait());
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

        [TestMethod]
        public void TestUpdateUser()
        {
            this.repository.AddUser(DefaultUser).Wait();

            var updatedUser = new UserBll { Nickname = DefaultUser.Nickname, Password = "other password", City = DefaultUser.City, BirthDate = new DateTime(2015, 12, 4) };

            this.repository.UpdateUser(updatedUser).Wait();

            UserBll actualUser = this.repository.GetUser(DefaultUser.City, DefaultUser.Nickname).Result;

            Assert.IsNotNull(actualUser);
            Assert.AreEqual(updatedUser.Nickname, actualUser.Nickname);
            Assert.AreEqual(updatedUser.Password, actualUser.Password);
            Assert.AreEqual(updatedUser.BirthDate, actualUser.BirthDate);
        }

        [TestMethod]
        public void TestUpdateNonExistingUser()
        {
            var updatedUser = new UserBll { Nickname = DefaultUser.Nickname, Password = "other password" };

            AssertExtensions.Throws<AzureTableStorageException>(() => this.repository.UpdateUser(updatedUser).Wait());
        }

        [TestMethod]
        public void TestDeleteUser()
        {
            this.repository.AddUser(DefaultUser).Wait();

            this.repository.DeleteUser(DefaultUser).Wait();

            Assert.IsFalse(this.repository.ContainsUser(DefaultUser).Result);
        }

        [TestMethod]
        public void TestDeleteNonExistingUser()
        {
            AssertExtensions.Throws<AzureTableStorageException>(() => this.repository.DeleteUser(DefaultUser).Wait());
        }

        [TestMethod]
        public void TestDeleteUserDifferentCities()
        {
            var user = new UserBll { Nickname = "nickname", Password = "password", City = "Malaga", RegistrationDate = DateTime.Today };

            this.repository.AddUser(user).Wait();

            var differentCityUser = new UserBll { Nickname = "nickname", Password = "password", City = "Dublin", RegistrationDate = DateTime.Today };

            this.repository.DeleteUser(differentCityUser).Wait();

            Assert.IsFalse(this.repository.ContainsUser(user).Result);
        }

        [TestMethod]
        public void TestDeleteStorage()
        {
            this.AddUsers();

            this.repository.DeleteAllUsers(DefaultUser.City).Wait();

            for (int i = 0; i < 10; ++i)
            {
                Assert.IsFalse(this.repository.ContainsUser(new UserBll { Nickname = "nickname" + i, Password = "password" + i }).Result);
            }
        }

        #region Helper methods

        private void AddUsers()
        {
            for (int i = 0; i < 10; ++i)
            {
                var user = new UserBll { Nickname = "nickname" + i, Password = "password" + i, City = "Malaga", RegistrationDate = DateTime.Today };
                this.repository.AddUser(user).Wait();
            }
        }

        #endregion
    }
}
