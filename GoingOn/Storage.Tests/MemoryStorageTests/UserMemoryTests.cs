// ****************************************************************************
// <copyright file="UserMemory.cs" company="Universidad de Malaga">
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
    
    using Moq;

    using Common.Tests;
    using Model.EntitiesBll;
    using Storage.MemoryStorage.Entities;

    [TestClass]
    public class UserMemoryTests
    {
        [TestMethod]
        public void TestEqualsSucceeds()
        {
            UserMemory user1 = new UserMemory("nickname", It.IsAny<string>());
            UserMemory user2 = new UserMemory("nickname", "password");
            UserMemory user3 = new UserMemory("nickname", "other password");

            Assert.AreEqual(user1, user2, "Equal");
            Assert.AreEqual(user1, user3, "Equal - Different password");
        }

        [TestMethod]
        public void TestEqualsFails()
        {
            UserMemory user1 = new UserMemory("nickname", It.IsAny<string>());
            UserMemory user2 = new UserMemory("another nickname", It.IsAny<string>());

            Assert.AreNotEqual(user1, user2, "Different nickname");
            Assert.AreNotEqual(user1, null, "Null author");
        }

        [TestMethod]
        public void TestHashCode()
        {
            UserMemory user1 = new UserMemory("nickname", "password");

            Assert.IsTrue(user1.GetHashCode() < 0);
        }

        [TestMethod]
        public void TestFromUserBll()
        {
            UserMemory userMemory = new UserMemory("nickname", It.IsAny<string>());
            UserBll userBll = new UserBll{ Nickname = "nickname", Password = It.IsAny<string>() };

            Assert.AreEqual(userMemory, UserMemory.FromUserBll(userBll));
        }

        [TestMethod]
        public void TestToUserBll()
        {
            UserMemory userMemory = new UserMemory("nickname", It.IsAny<string>());
            UserBll userBll = new UserBll { Nickname = "nickname", Password = It.IsAny<string>() };

            Assert.IsTrue(new UserBllEqualityComparer().Equals(userBll, UserMemory.ToUserBll(userMemory)));
        }

        [TestMethod]
        public void TestMerge()
        {
            UserMemory user1 = new UserMemory("nickname", "password");
            UserMemory user2 = new UserMemory("nickname", "password2");

            user1.Merge(user2);

            Assert.AreEqual(user1.Nickname, user2.Nickname);
            Assert.AreEqual(user1.Password, user2.Password);
        }

        [TestMethod]
        public void TestMergeNullPassword()
        {
            UserMemory user1 = new UserMemory("nickname", "password");
            UserMemory user2 = new UserMemory("nickname", null);

            user1.Merge(user2);

            Assert.AreEqual(user1.Nickname, user2.Nickname);
            Assert.AreEqual("password", user1.Password);
        }

        [TestMethod]
        public void TestMergeDifferentUsers()
        {
            UserMemory user1 = new UserMemory("nickname", "password");
            UserMemory user2 = new UserMemory("other nickname", "other password");

            user1.Merge(user2);

            Assert.AreNotEqual(user1.Nickname, user2.Nickname);
            Assert.AreNotEqual(user1.Password, user2.Password);
        }
    }
}
