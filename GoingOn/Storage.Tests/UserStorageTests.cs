// ****************************************************************************
// <copyright file="UserDocumentDBStorageTests.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace GoingOn.Storage.Tests
{
    using System;
    using System.Configuration;
    using System.Threading.Tasks;
    using GoingOn.Common.Tests;
    using GoingOn.Model.EntitiesBll;
    using GoingOn.Storage.TableStorage;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Storage;

    [TestClass]
    public class UserStorageTests
    {
        // RegistrationDate has to be initializated otherwise it will get the min value that crashes on TableStorage
        private static readonly UserBll DefaultUser = new UserBll { Nickname = "nickname", Password = "password", City = "Malaga", BirthDate = new DateTime(2014, 12, 4), RegistrationDate = new DateTime(2014, 12, 4) };

        private IUserStorage storage;

        [TestInitialize]
        public void Initialize()
        {
            string connectionString = ConfigurationManager.AppSettings["StorageConnectionString"];
            string userTableName = ConfigurationManager.AppSettings["UserTableName"];

            this.storage = new UserTableStorage(connectionString, userTableName);
        }

        [TestCleanup]
        public void Cleanup()
        {
            this.storage.DeleteAllUsers(DefaultUser.City).Wait();
            this.storage.DeleteAllUsers("Dublin").Wait();
        }

        [TestMethod]
        public void TestAddUser()
        {
            this.storage.AddUser(DefaultUser).Wait();

            Task<bool> containsUserTask = this.storage.ContainsUser(DefaultUser);
            containsUserTask.Wait();

            Assert.IsTrue(containsUserTask.Result);
        }

        [TestMethod]
        public void TestAddExistingUser()
        {
            this.storage.AddUser(DefaultUser).Wait();

            AssertExtensions.Throws<StorageException>(() => this.storage.AddUser(DefaultUser).Wait());
        }

        [TestMethod]
        public void TestGetUser()
        {
            this.storage.AddUser(DefaultUser).Wait();

            UserBll actualUser = this.storage.GetUser(DefaultUser.Nickname).Result;

            Assert.IsTrue(new UserBllEqualityComparer().Equals(DefaultUser, actualUser));
        }

        [TestMethod]
        public void TestGetNonExistingUser()
        {
            AssertExtensions.Throws<AzureTableStorageException>(() => this.storage.GetUser(DefaultUser.Nickname).Wait());
        }

        [TestMethod]
        public void TestContainsExistingUser()
        {
            this.storage.AddUser(DefaultUser).Wait();

            Assert.IsTrue(this.storage.ContainsUser(DefaultUser).Result);
        }

        [TestMethod]
        public void TestContainsNonExistingUser()
        {
            Assert.IsFalse(this.storage.ContainsUser(DefaultUser).Result);
        }

        [TestMethod]
        public void TestContainsUserIgnoresCity()
        {
            var user = new UserBll { Nickname = "nickname", Password = "password", City = "Malaga", RegistrationDate = DateTime.Today };

            this.storage.AddUser(user).Wait();

            var differentCityUser = new UserBll { Nickname = "nickname", Password = "password", City = "Dublin", RegistrationDate = DateTime.Today };

            Assert.IsTrue(this.storage.ContainsUser(user).Result);
            Assert.IsTrue(this.storage.ContainsUser(differentCityUser).Result);
        }

        [TestMethod]
        public void TestUpdateUser()
        {
            this.storage.AddUser(DefaultUser).Wait();

            var updatedUser = new UserBll { Nickname = DefaultUser.Nickname, Password = "other password", City = DefaultUser.City, BirthDate = new DateTime(2015, 12, 4) };

            this.storage.UpdateUser(updatedUser).Wait();

            UserBll actualUser = this.storage.GetUser(DefaultUser.Nickname).Result;

            Assert.IsNotNull(actualUser);
            Assert.AreEqual(updatedUser.Nickname, actualUser.Nickname);
            Assert.AreEqual(updatedUser.Password, actualUser.Password);
            Assert.AreEqual(updatedUser.BirthDate, actualUser.BirthDate);
        }

        [TestMethod]
        public void TestUpdateNonExistingUser()
        {
            var updatedUser = new UserBll { Nickname = DefaultUser.Nickname, Password = "other password" };

            AssertExtensions.Throws<AzureTableStorageException>(() => this.storage.UpdateUser(updatedUser).Wait());
        }

        [TestMethod]
        public void TestDeleteUser()
        {
            this.storage.AddUser(DefaultUser).Wait();

            this.storage.DeleteUser(DefaultUser).Wait();

            Assert.IsFalse(this.storage.ContainsUser(DefaultUser).Result);
        }

        [TestMethod]
        public void TestDeleteNonExistingUser()
        {
            AssertExtensions.Throws<AzureTableStorageException>(() => this.storage.DeleteUser(DefaultUser).Wait());
        }

        [TestMethod]
        public void TestDeleteUserDifferentCities()
        {
            var user = new UserBll { Nickname = "nickname", Password = "password", City = "Malaga", RegistrationDate = DateTime.Today };

            this.storage.AddUser(user).Wait();

            var differentCityUser = new UserBll { Nickname = "nickname", Password = "password", City = "Dublin", RegistrationDate = DateTime.Today };

            this.storage.DeleteUser(differentCityUser).Wait();

            Assert.IsFalse(this.storage.ContainsUser(user).Result);
        }

        [TestMethod]
        public void TestDeleteStorage()
        {
            this.AddUsers();

            this.storage.DeleteAllUsers(DefaultUser.City).Wait();

            for (int i = 0; i < 10; ++i)
            {
                Assert.IsFalse(this.storage.ContainsUser(new UserBll { Nickname = "nickname" + i, Password = "password" + i }).Result);
            }
        }

        #region Helper methods

        private void AddUsers()
        {
            for (int i = 0; i < 10; ++i)
            {
                var user = new UserBll { Nickname = "nickname" + i, Password = "password" + i, City = "Malaga", RegistrationDate = DateTime.Today };
                this.storage.AddUser(user).Wait();
            }
        }

        #endregion
    }
}
