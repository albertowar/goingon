// ****************************************************************************
// <copyright file="NewsEntity.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// Tests for User entity
// </summary>
// ****************************************************************************

namespace GoingOn.XStoreProxy.Tests.Entitites
{
    using System;
    using GoingOn.Model.EntitiesBll;
    using GoingOn.XStoreProxy.Entities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class UserEntityTests
    {
        [TestMethod]
        public void TestFromUserBll()
        {
            var userBll = new UserBll
            {
                Nickname = "nickname",
                City = "Malaga",
                Password = "password",
                Email = "email",
                BirthDate = DateTime.Today,
                RegistrationDate = DateTime.Today
            };

            UserEntity generatedUserEntity = UserEntity.FromUserBll(userBll);

            Assert.AreEqual(userBll.City, generatedUserEntity.RowKey);
            Assert.AreEqual(userBll.Nickname, generatedUserEntity.PartitionKey);
            Assert.AreEqual(userBll.Password, generatedUserEntity.Password);
            Assert.AreEqual(userBll.Email, generatedUserEntity.Email);
            Assert.AreEqual(userBll.BirthDate, generatedUserEntity.BirthDate);
            Assert.AreEqual(userBll.RegistrationDate, generatedUserEntity.RegistrationDate);
        }

        [TestMethod]
        public void TestToUserBll()
        {
            var userEntity = new UserEntity
            {
                PartitionKey = "Malaga",
                RowKey = "nickname",
                Password = "password",
                Email = "email",
                BirthDate = DateTime.Today,
                RegistrationDate = DateTime.Today
            };

            UserBll generatedUserBll = UserEntity.ToUserBll(userEntity);

            Assert.AreEqual(userEntity.PartitionKey, generatedUserBll.Nickname);
            Assert.AreEqual(userEntity.RowKey, generatedUserBll.City);
            Assert.AreEqual(userEntity.Password, generatedUserBll.Password);
            Assert.AreEqual(userEntity.Email, generatedUserBll.Email);
            Assert.AreEqual(userEntity.BirthDate, generatedUserBll.BirthDate);
            Assert.AreEqual(userEntity.RegistrationDate, generatedUserBll.RegistrationDate);
        }

        [TestMethod]
        public void TestEquals()
        {
            var referenceUser = new UserEntity
            {
                PartitionKey = "username"
            };

            var sameNameUser = new UserEntity
            {
                PartitionKey = "username"
            };

            var differentNameUser = new UserEntity
            {
                PartitionKey = "anotherName"
            };

            Assert.AreEqual(referenceUser, sameNameUser);
            Assert.AreNotEqual(referenceUser, differentNameUser);
        }

        [TestMethod]
        public void TestMerge()
        {
            var userToMerge = new UserEntity
            {
                PartitionKey = "Malaga",
                RowKey = "nickname",
                Password = "password",
                Email = "email",
                BirthDate = DateTime.Today,
                RegistrationDate = DateTime.Today
            };

            var userDifferentPassword = new UserEntity
            {
                PartitionKey = "Malaga",
                RowKey = "nickname",
                Password = "another-password",
                Email = "email",
                BirthDate = DateTime.Today,
                RegistrationDate = DateTime.Today
            };

            var userDifferentEmail = new UserEntity
            {
                PartitionKey = "Malaga",
                RowKey = "nickname",
                Password = "password",
                Email = "another-email",
                BirthDate = DateTime.Today,
                RegistrationDate = DateTime.Today
            };

            UserEntity userDifferentBirthday = new UserEntity
            {
                PartitionKey = "Malaga",
                RowKey = "nickname",
                Password = "password",
                Email = "email",
                BirthDate = DateTime.Today.AddDays(-1),
                RegistrationDate = DateTime.Today
            };

            userDifferentPassword.Merge(userToMerge);
            userDifferentEmail.Merge(userToMerge);
            userDifferentBirthday.Merge(userToMerge);

            Assert.AreEqual(userDifferentPassword.Password, userToMerge.Password);
            Assert.AreEqual(userDifferentPassword.Email, userToMerge.Email);
            Assert.AreEqual(userDifferentPassword.BirthDate, userToMerge.BirthDate);
        }
    }
}
