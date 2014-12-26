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
        private static readonly UserBll User = new UserBll{ Nickname = "nickname", Password = "password" };

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

            var containsUserTask = storage.ContainsUser(User);
            containsUserTask.Wait();

            Assert.IsTrue(containsUserTask.Result);
        }

        [TestMethod]
        public void TestAddExistingUser()
        {
            storage.AddUser(User);
            storage.AddUser(User);

            var containsUserTask = storage.ContainsUser(User);
            containsUserTask.Wait();

            Assert.IsTrue(containsUserTask.Result);
        }

        [TestMethod]
        public void TestGetUser()
        {
            storage.AddUser(User);

            var getUserTask = storage.GetUser(User.Nickname);
            getUserTask.Wait();

            UserBll actualUser = getUserTask.Result;

            Assert.IsTrue(new UserBllEqualityComparer().Equals(User, actualUser));
        }

        [TestMethod]
        public void TestGetNonExistingUser()
        {
            var getUserTask = storage.GetUser(User.Nickname);
            getUserTask.Wait();

            UserBll actualUser = getUserTask.Result;

            Assert.IsNull(actualUser);
        }

        [TestMethod]
        public void TestContainsExistingUser()
        {
            storage.AddUser(User);

            var containsUserTask = storage.ContainsUser(User);
            containsUserTask.Wait();

            Assert.IsTrue(containsUserTask.Result);
        }

        [TestMethod]
        public void TestContainsNonExistingUser()
        {
            var containsUserTask = storage.ContainsUser(User);
            containsUserTask.Wait();

            Assert.IsFalse(containsUserTask.Result);
        }

        [TestMethod]
        public void TestUpdateUser()
        {
            storage.AddUser(User);

            UserBll updatedUser = new UserBll { Nickname = User.Nickname, Password = "other password" };

            storage.UpdateUser(updatedUser);

            var getUserTask = storage.GetUser(User.Nickname);
            getUserTask.Wait();

            UserBll actualUser = getUserTask.Result;
            
            Assert.IsNotNull(actualUser);
            Assert.AreEqual(updatedUser.Nickname, actualUser.Nickname);
            Assert.AreEqual(updatedUser.Password, actualUser.Password);
        }

        [TestMethod]
        public void TestUpdateNonExistingUser()
        {
            UserBll updatedUser = new UserBll{ Nickname = User.Nickname, Password = "other password" };

            storage.UpdateUser(updatedUser);

            var getUserTask = storage.GetUser(User.Nickname);
            getUserTask.Wait();

            UserBll actualUser = getUserTask.Result;

            Assert.IsNull(actualUser);
        }

        [TestMethod]
        public void TestDeleteUser()
        {
            storage.AddUser(User);

            storage.DeleteUser(User);

            var containsUserTask = storage.ContainsUser(User);
            containsUserTask.Wait();

            Assert.IsFalse(containsUserTask.Result);
        }

        [TestMethod]
        public void TestDeleteNonExistingUser()
        {
            storage.DeleteUser(User);

            var containsUserTask = storage.ContainsUser(User);
            containsUserTask.Wait();

            Assert.IsFalse(containsUserTask.Result);
        }

        [TestMethod]
        public void TestDeleteAllUser()
        {
            this.AddUsers();

            storage.DeleteAllUser();

            for (int i = 0; i < 10; ++i)
            {
                var containsUserTask = storage.ContainsUser(new UserBll { Nickname = "nickname" + i, Password = "password" + i });
                containsUserTask.Wait();

                Assert.IsFalse(containsUserTask.Result);
            }
        }

        #region Helper methods

        private void AddUsers()
        {
            for (int i = 0; i < 10; ++i)
            {
                var news = new UserBll{ Nickname = "nickname" + i, Password = "password" + i };
            }
        }

        #endregion
    }
}
