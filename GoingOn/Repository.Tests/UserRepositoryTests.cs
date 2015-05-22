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
        private static readonly UserBll DefaultUser = new UserBll();

        private IUserRepository repository;

        private Mock<ITableStore> mockStore;

        [TestInitialize]
        public void Initialize()
        {
            this.mockStore = new Mock<ITableStore>();
            this.repository = new UserTableRepository(this.mockStore.Object);
        }

        [TestMethod]
        public void TestContainsExistingUser()
        {
            this.mockStore.Setup(store => store.GetTableEntity<UserEntity>(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(UserEntity.FromUserBll(DefaultUser)));

            Assert.IsTrue(this.repository.ContainsUser(It.IsAny<UserBll>()).Result);
        }

        [TestMethod]
        public void TestContainsNonExistingUser()
        {
            this.mockStore.Setup(store => store.GetTableEntity<UserEntity>(It.IsAny<string>(), It.IsAny<string>())).Throws<AzureTableStorageException>();

            Assert.IsFalse(this.repository.ContainsUser(DefaultUser).Result);
        }
    }
}
