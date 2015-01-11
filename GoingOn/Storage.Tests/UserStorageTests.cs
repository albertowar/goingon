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
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Common.Tests;
    using Model.EntitiesBll;
    using Storage.TableStorage;

    [TestClass]
    public class UserStorageTests
    {
        private static readonly UserBll User = new UserBll { Nickname = "nickname", Password = "password" };

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
        public void TestGetUser()
        {
            storage.AddUser(User).Wait();

            var actualUser = storage.GetUser(User.Nickname).Result;

            Assert.IsTrue(new UserBllEqualityComparer().Equals(User, actualUser));
        }

        [TestMethod]
        public void TestContainsExistingUser()
        {
            storage.AddUser(User).Wait();

            Assert.IsTrue(storage.ContainsUser(User).Result);
        }

        [TestMethod]
        public void TestUpdateUser()
        {
            storage.AddUser(User).Wait();

            UserBll updatedUser = new UserBll { Nickname = User.Nickname, Password = "other password" };

            storage.UpdateUser(updatedUser).Wait();

            var getUserTask = storage.GetUser(User.Nickname);
            getUserTask.Wait();

            UserBll actualUser = getUserTask.Result;

            Assert.IsNotNull(actualUser);
            Assert.AreEqual(updatedUser.Nickname, actualUser.Nickname);
            Assert.AreEqual(updatedUser.Password, actualUser.Password);
        }

        [TestMethod]
        public void TestDeleteUser()
        {
            storage.AddUser(User).Wait();

            storage.DeleteUser(User).Wait();

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
                var user = new UserBll { Nickname = "nickname" + i, Password = "password" + i };
                storage.AddUser(user).Wait();
            }
        }

        #endregion
    }
}
