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
            storage.DeleteAllUsers("Malaga").Wait();
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

            storage.UpdateUser(updatedUser).Wait();

            Assert.IsFalse(storage.ContainsUser(updatedUser).Result);
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

            storage.DeleteAllUsers("Malaga").Wait();

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
