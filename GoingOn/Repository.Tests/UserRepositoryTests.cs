// ****************************************************************************
// <copyright file="UserDocumentDBStorageTests.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// Tests for User repository
// </summary>
// ****************************************************************************

namespace GoingOn.Repository.Tests
{
    using System.Threading.Tasks;
    using GoingOn.Model.EntitiesBll;
    using GoingOn.Repository;
    using GoingOn.XStoreProxy;
    using GoingOn.XStoreProxy.Entities;
    using GoingOn.XStoreProxy.TableStore;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class UserRepositoryTests
    {
        private static readonly UserBll DefaultUser = new UserBll { City = string.Empty, Nickname = string.Empty };

        private IUserRepository repository;

        private Mock<ITableStore> mockUserStore;

        [TestInitialize]
        public void Initialize()
        {
            this.mockUserStore = new Mock<ITableStore>();
            this.repository = new UserTableRepository(this.mockUserStore.Object);
        }

        [TestMethod]
        public void TestContainsExistingUser()
        {
            this.mockUserStore.Setup(store => store.GetTableEntity<UserEntity>(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(It.IsAny<UserEntity>()));

            Assert.IsTrue(this.repository.ContainsUser(DefaultUser).Result);
        }

        [TestMethod]
        public void TestContainsNonExistingUser()
        {
            this.mockUserStore.Setup(store => store.GetTableEntity<UserEntity>(It.IsAny<string>(), It.IsAny<string>())).Throws<AzureXStoreException>();

            Assert.IsFalse(this.repository.ContainsUser(DefaultUser).Result);
        }
    }
}
