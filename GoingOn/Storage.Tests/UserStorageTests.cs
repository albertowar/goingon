// ****************************************************************************
// <copyright file="UserDocumentDBStorageTests.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace Storage.Tests
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Common.Tests;
    using Model.EntitiesBll;
    using Storage.TableStorage;

    [TestClass]
    public class UserStorageTests
    {
        // RegistrationDate has to be initializated otherwise it will get the min value that crashes on TableStorage
        private static readonly UserBll User = new UserBll { Nickname = "nickname", Password = "password", City = "Malaga", RegistrationDate = DateTime.Today };

        private IUserStorage storage;

        [TestInitialize]
        public void Initialize()
        {
            storage = UserTableStorage.GetInstance();
        }

        [TestCleanup]
        public void Cleanup()
        {
            storage.DeleteAllUsers().Wait();
        }

        [TestMethod]
        public void TestAddUser()
        {
            storage.AddUser(User).Wait();

            var containsUserTask = storage.ContainsUser(User);
            containsUserTask.Wait();

            Assert.IsTrue(containsUserTask.Result);
        }

        [TestMethod]
        public void TestAddExistingUser()
        {
            storage.AddUser(User).Wait();

            AssertExtensions.Throws<StorageException>(() => storage.AddUser(User).Wait());
        }

        [TestMethod]
        public void TestAddSameUserDifferentCity()
        {
            var user = new UserBll { Nickname = "nickname", Password = "password", City = "Malaga", RegistrationDate = DateTime.Today };

            storage.AddUser(user).Wait();

            var differentCityUser = new UserBll { Nickname = "nickname", Password = "password", City = "Dublin", RegistrationDate = DateTime.Today };

            AssertExtensions.Throws<StorageException>(() => storage.AddUser(differentCityUser).Wait());
        }

        [TestMethod]
        public void TestGetUser()
        {
            storage.AddUser(User).Wait();

            var actualUser = storage.GetUser(User.Nickname).Result;

            Assert.IsTrue(new UserBllEqualityComparer().Equals(User, actualUser));
        }

        [TestMethod]
        public void TestGetNonExistingUser()
        {
            AssertExtensions.Throws<StorageException>(() => storage.GetUser(User.Nickname).Wait());
        }

        [TestMethod]
        public void TestContainsExistingUser()
        {
            storage.AddUser(User).Wait();

            Assert.IsTrue(storage.ContainsUser(User).Result);
        }

        [TestMethod]
        public void TestContainsNonExistingUser()
        {
            Assert.IsFalse(storage.ContainsUser(User).Result);
        }

        [TestMethod]
        public void TestContainsUserIgnoresCity()
        {
            var user = new UserBll { Nickname = "nickname", Password = "password", City = "Malaga", RegistrationDate = DateTime.Today };

            storage.AddUser(user).Wait();

            var differentCityUser = new UserBll { Nickname = "nickname", Password = "password", City = "Dublin", RegistrationDate = DateTime.Today };

            Assert.IsTrue(storage.ContainsUser(user).Result);
            Assert.IsTrue(storage.ContainsUser(differentCityUser).Result);
        }

        [TestMethod]
        public void TestUpdateUser()
        {
            storage.AddUser(User).Wait();

            var updatedUser = new UserBll { Nickname = User.Nickname, Password = "other password", City = "Malaga", RegistrationDate = DateTime.Today };

            storage.UpdateUser(updatedUser).Wait();

            var actualUser = storage.GetUser(User.Nickname).Result;

            Assert.IsNotNull(actualUser);
            Assert.AreEqual(updatedUser.Nickname, actualUser.Nickname);
            Assert.AreEqual(updatedUser.Password, actualUser.Password);
        }

        [TestMethod]
        public void TestUpdateNonExistingUser()
        {
            var updatedUser = new UserBll { Nickname = User.Nickname, Password = "other password" };

            AssertExtensions.Throws<StorageException>(() => storage.UpdateUser(updatedUser).Wait());
        }

        [TestMethod]
        public void TestDeleteUser()
        {
            storage.AddUser(User).Wait();

            storage.DeleteUser(User).Wait();

            Assert.IsFalse(storage.ContainsUser(User).Result);
        }

        [TestMethod]
        public void TestDeleteNonExistingUser()
        {
            storage.DeleteUser(User);

            Assert.IsFalse(storage.ContainsUser(User).Result);
        }

        [TestMethod]
        public void TestDeleteStorage()
        {
            this.AddUsers();

            storage.DeleteAllUsers().Wait();

            for (int i = 0; i < 10; ++i)
            {
                Assert.IsFalse(storage.ContainsUser(new UserBll { Nickname = "nickname" + i, Password = "password" + i }).Result);
            }
        }

        #region Helper methods

        private void AddUsers()
        {
            for (int i = 0; i < 10; ++i)
            {
                var user = new UserBll { Nickname = "nickname" + i, Password = "password" + i, City = "Malaga", RegistrationDate = DateTime.Today };
                storage.AddUser(user).Wait();
            }
        }

        #endregion
    }
}
