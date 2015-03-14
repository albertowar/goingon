﻿// ****************************************************************************
// <copyright file="UserDocumentDBStorageTests.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

using IUserStorage = GoingOn.Storage.IUserStorage;
using StorageException = GoingOn.Storage.StorageException;
using UserTableStorage = GoingOn.Storage.TableStorage.UserTableStorage;

namespace GoingOn.Storage.Tests
{
    using System;
    using GoingOn.Common.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Model.EntitiesBll;

    [TestClass]
    public class UserStorageTests
    {
        // RegistrationDate has to be initializated otherwise it will get the min value that crashes on TableStorage
        private static readonly UserBll User = new UserBll { Nickname = "nickname", Password = "password", City = "Malaga", BirthDate = new DateTime(2014, 12, 4), RegistrationDate = new DateTime(2014, 12, 4) };

        private IUserStorage storage;

        [TestInitialize]
        public void Initialize()
        {
            this.storage = UserTableStorage.GetInstance();
        }

        [TestCleanup]
        public void Cleanup()
        {
            this.storage.DeleteAllUsers().Wait();
        }

        [TestMethod]
        public void TestAddUser()
        {
            this.storage.AddUser(User).Wait();

            var containsUserTask = this.storage.ContainsUser(User);
            containsUserTask.Wait();

            Assert.IsTrue(containsUserTask.Result);
        }

        [TestMethod]
        public void TestAddExistingUser()
        {
            this.storage.AddUser(User).Wait();

            AssertExtensions.Throws<StorageException>(() => this.storage.AddUser(User).Wait());
        }

        [TestMethod]
        public void TestAddSameUserDifferentCity()
        {
            var user = new UserBll { Nickname = "nickname", Password = "password", City = "Malaga", RegistrationDate = DateTime.Today };

            this.storage.AddUser(user).Wait();

            var differentCityUser = new UserBll { Nickname = "nickname", Password = "password", City = "Dublin", RegistrationDate = DateTime.Today };

            AssertExtensions.Throws<StorageException>(() => this.storage.AddUser(differentCityUser).Wait());
        }

        [TestMethod]
        public void TestGetUser()
        {
            this.storage.AddUser(User).Wait();

            var actualUser = this.storage.GetUser(User.Nickname).Result;

            Assert.IsTrue(new UserBllEqualityComparer().Equals(User, actualUser));
        }

        [TestMethod]
        public void TestGetNonExistingUser()
        {
            AssertExtensions.Throws<StorageException>(() => this.storage.GetUser(User.Nickname).Wait());
        }

        [TestMethod]
        public void TestContainsExistingUser()
        {
            this.storage.AddUser(User).Wait();

            Assert.IsTrue(this.storage.ContainsUser(User).Result);
        }

        [TestMethod]
        public void TestContainsNonExistingUser()
        {
            Assert.IsFalse(this.storage.ContainsUser(User).Result);
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
            this.storage.AddUser(User).Wait();

            var updatedUser = new UserBll { Nickname = User.Nickname, Password = "other password", City = "Dublin", BirthDate = new DateTime(2015, 12, 4) };

            this.storage.UpdateUser(updatedUser).Wait();

            var actualUser = this.storage.GetUser(User.Nickname).Result;

            Assert.IsNotNull(actualUser);
            Assert.AreEqual(updatedUser.Nickname, actualUser.Nickname);
            Assert.AreEqual(updatedUser.Password, actualUser.Password);
            Assert.AreEqual(updatedUser.City, actualUser.City);
            Assert.AreEqual(updatedUser.BirthDate, actualUser.BirthDate);
        }

        [TestMethod]
        public void TestUpdateNonExistingUser()
        {
            var updatedUser = new UserBll { Nickname = User.Nickname, Password = "other password" };

            AssertExtensions.Throws<StorageException>(() => this.storage.UpdateUser(updatedUser).Wait());
        }

        [TestMethod]
        public void TestDeleteUser()
        {
            this.storage.AddUser(User).Wait();

            this.storage.DeleteUser(User).Wait();

            Assert.IsFalse(this.storage.ContainsUser(User).Result);
        }

        [TestMethod]
        public void TestDeleteNonExistingUser()
        {
            AssertExtensions.Throws<StorageException>(() => this.storage.DeleteUser(User).Wait());
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

            this.storage.DeleteAllUsers().Wait();

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
