// ****************************************************************************
// <copyright file="NewsMemoryTests.cs" company="Universidad de Malaga">
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
    using MemoryStorage;
    using Model.EntitiesBll;

    [TestClass]
    public class UserMemoryStorageTests
    {
        private static readonly UserBll User = new UserBll(nickname: "nickname", password: "password");

        private IUserStorage storage;

        [TestInitialize]
        public void Initialize()
        {
            storage = UserMemoryStorage.GetInstance();
        }

        [TestCleanup]
        public void Cleanup()
        {
            storage.DeleteAllUser();
        }

        [TestMethod]
        public void TestAddUser()
        {
            storage.AddUser(User);

            Assert.IsTrue(storage.ContainsUser(User));
        }

        [TestMethod]
        public void TestAddExistingUser()
        {
            storage.AddUser(User);
            storage.AddUser(User);

            Assert.IsTrue(storage.ContainsUser(User));
        }

        [TestMethod]
        public void TestGetUser()
        {
            storage.AddUser(User);

            UserBll actualUser = storage.GetUser(User.Nickname);

            Assert.IsTrue(new UserBllEqualityComparer().Equals(User, actualUser));
        }

        [TestMethod]
        public void TestGetNonExistingUser()
        {
            Assert.IsNull(storage.GetUser("not an user"));
        }

        [TestMethod]
        public void TestContainsExistingUser()
        {
            storage.AddUser(User);

            Assert.IsTrue(storage.ContainsUser(User));
        }

        [TestMethod]
        public void TestContainsNonExistingUser()
        {
            Assert.IsFalse(storage.ContainsUser(User));
        }

        [TestMethod]
        public void TestDeleteUser()
        {
            storage.AddUser(User);

            storage.DeleteUser(User);

            Assert.IsFalse(storage.ContainsUser(User));
        }

        [TestMethod]
        public void TestDeleteNonExistingUser()
        {
            storage.DeleteUser(User);

            Assert.IsFalse(storage.ContainsUser(User));
        }

        [TestMethod]
        public void TestDeleteAllUser()
        {
            this.AddUsers();

            storage.DeleteAllUser();

            for (int i = 0; i < 10; ++i)
            {
                Assert.IsFalse(storage.ContainsUser(new UserBll(nickname: "nickname" + i, password: "password" + i)));
            }
        }

        #region Helper methods

        private void AddUsers()
        {
            for (int i = 0; i < 10; ++i)
            {
                var news = new UserBll(nickname: "nickname" + i, password: "password" + i);
            }
        }

        #endregion
    }
}
