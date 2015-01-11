// ****************************************************************************
// <copyright file="UserDocumentDBStorageTests.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace Storage.Tests.MemoryStorageTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Common.Tests;
    using Model.EntitiesBll;
    using Storage.MemoryStorage;

    [TestClass]
    public class UserMemoryStorageTests
    {
        private static readonly UserBll User = new UserBll { Nickname = "nickname", Password = "password" };

        private IUserStorage storage;

        [TestInitialize]
        public void Initialize()
        {
            storage = UserMemoryStorage.GetInstance();
        }

        [TestCleanup]
        public void Cleanup()
        {
            storage.DeleteAllUsers().Wait();
        }

        [TestMethod]
        public void TestAddExistingUser()
        {
            storage.AddUser(User).Wait();
            storage.AddUser(User).Wait();

            Assert.IsTrue(storage.ContainsUser(User).Result);
        }

        [TestMethod]
        public void TestGetNonExistingUser()
        {
            Assert.IsNull(storage.GetUser(User.Nickname).Result);
        }

        [TestMethod]
        public void TestContainsNonExistingUser()
        {
            Assert.IsFalse(storage.ContainsUser(User).Result);
        }

        [TestMethod]
        public void TestUpdateNonExistingUser()
        {
            UserBll updatedUser = new UserBll { Nickname = User.Nickname, Password = "other password" };

            storage.UpdateUser(updatedUser).Wait();

            Assert.IsFalse(storage.ContainsUser(User).Result);
        }

        [TestMethod]
        public void TestDeleteNonExistingUser()
        {
            storage.DeleteUser(User);

            Assert.IsFalse(storage.ContainsUser(User).Result);
        }
    }
}
