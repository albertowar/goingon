// ****************************************************************************
// <copyright file="UserDocumentDBStorageTests.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace Storage.Tests.TableStorageTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Common.Tests;
    using Model.EntitiesBll;
    using Storage.TableStorage;

    [TestClass]
    public class UserTableStorageTests
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
        public void TestAddExistingUser()
        {
            storage.AddUser(User).Wait();

            AssertExtensions.Throws<Microsoft.WindowsAzure.Storage.StorageException>(() => storage.AddUser(User).Wait());
        }

        [TestMethod]
        public void TestGetNonExistingUser()
        {
            AssertExtensions.Throws<Storage.StorageException>(() => storage.GetUser(User.Nickname).Wait());
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

            Assert.IsFalse(storage.ContainsUser(updatedUser).Result);
        }

        [TestMethod]
        public void TestDeleteNonExistingUser()
        {
            storage.DeleteUser(User);

            Assert.IsFalse(storage.ContainsUser(User).Result);
        }
    }
}
