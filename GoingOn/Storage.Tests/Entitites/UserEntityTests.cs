// ****************************************************************************
// <copyright file="NewsEntity.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// Tests for User entity
// </summary>
// ****************************************************************************

namespace GoingOn.Storage.Tests.Entitites
{
    using System;
    using GoingOn.Model.EntitiesBll;
    using GoingOn.Storage.TableStorage.Entities;
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

            Assert.AreEqual(userBll.City, generatedUserEntity.PartitionKey);
            Assert.AreEqual(userBll.Nickname, generatedUserEntity.RowKey);
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

            Assert.AreEqual(userEntity.PartitionKey, generatedUserBll.City);
            Assert.AreEqual(userEntity.RowKey, generatedUserBll.Nickname);
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
                RowKey = "username"
            };

            var sameNameUser = new UserEntity
            {
                RowKey = "username"
            };

            var differentNameUser = new UserEntity
            {
                RowKey = "anotherName"
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
