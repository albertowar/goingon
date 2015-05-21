// ****************************************************************************
// <copyright file="TableStoreTests.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// News Storage base interface
// </summary>
// ****************************************************************************

namespace GoingOn.XStoreProxy.Tests
{
    using System.Configuration;
    using System.Linq;
    using GoingOn.Common.Tests;
    using GoingOn.XStoreProxy.TableStore;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Storage.Table;

    [TestClass]
    public class TableStoreTests
    {
        private const string PartitionKey = "PartitionKey";
        private const string RowKey = "RowKey";

        private ITableStore store;

        [TestInitialize]
        public void Initialize()
        {
            string connectionString = ConfigurationManager.AppSettings["StorageConnectionString"];
            string userTableName = ConfigurationManager.AppSettings["UserTableName"];

            this.store = new TableStore(connectionString, userTableName);
        }

        [TestCleanup]
        public void Cleanup()
        {
            this.store.DeleteAllTableEntitiesInPartition<TableEntity>(PartitionKey).Wait();
        }

        [TestMethod]
        public void TestAddGetEntity()
        {
            var entity = new TableEntity(PartitionKey, RowKey);

            this.store.AddTableEntity(entity).Wait();

            TableEntity retrievedEntity = this.store.GetTableEntity<TableEntity>(PartitionKey, RowKey).Result;

            Assert.IsNotNull(retrievedEntity);
        }

        [TestMethod]
        public void TestUpdate()
        {
            var entity = new MockTableEntity
            {
                PartitionKey = PartitionKey,
                RowKey = RowKey,
                Parameter = "something"
            };

            this.store.AddTableEntity(entity).Wait();

            entity.Parameter = "another thing";

            this.store.UpdateTableEntity(entity).Wait();

            MockTableEntity retrievedEntity = this.store.GetTableEntity<MockTableEntity>(PartitionKey, RowKey).Result;

            Assert.AreEqual(entity.Parameter, retrievedEntity.Parameter);
        }

        [TestMethod]
        public void TestUpdateThrowsIfNotFound()
        {
            AssertExtensions.Throws<AzureTableStorageException>(() => this.store.UpdateTableEntity(new TableEntity(PartitionKey, RowKey)).Wait());
        }

        [TestMethod]
        public void TestGetThrowsIfNotFound()
        {
            AssertExtensions.Throws<AzureTableStorageException>(() => this.store.GetTableEntity<TableEntity>(PartitionKey, RowKey).Wait());
        }

        [TestMethod]
        public void TestListEntity()
        {
            var entity = new TableEntity(PartitionKey, RowKey);

            this.store.AddTableEntity(entity).Wait();

            Assert.IsTrue(this.store.ListTableEntity<TableEntity>(PartitionKey).Result.Any());
        }

        [TestMethod]
        public void TestListEntityReturnsEmptyListIfNonExisting()
        {
            Assert.IsFalse(this.store.ListTableEntity<TableEntity>(PartitionKey).Result.Any());
        }

        [TestMethod]
        public void TestDeleteEntity()
        {
            var entity = new TableEntity(PartitionKey, RowKey);

            this.store.AddTableEntity(entity).Wait();

            this.store.DeleteTableEntity<TableEntity>(PartitionKey, RowKey).Wait();

            Assert.IsFalse(this.store.ListTableEntity<TableEntity>(PartitionKey).Result.Any());
        }

        [TestMethod]
        public void TestDeleteThrowsIfNotFound()
        {
            AssertExtensions.Throws<AzureTableStorageException>(() => this.store.DeleteTableEntity<TableEntity>(PartitionKey, RowKey).Wait());
        }

        [TestMethod]
        public void TestDeleteAllTableEntitiesInPartition()
        {
            this.store.AddTableEntity(new TableEntity(PartitionKey, RowKey)).Wait();

            this.store.DeleteAllTableEntitiesInPartition<TableEntity>(PartitionKey).Wait();

            Assert.IsFalse(this.store.ListTableEntity<TableEntity>(PartitionKey).Result.Any());
        }

        #region Helper methods and classes

        public class MockTableEntity : TableEntity
        {
            public string Parameter { get; set; }
        }

        #endregion
    }
}
