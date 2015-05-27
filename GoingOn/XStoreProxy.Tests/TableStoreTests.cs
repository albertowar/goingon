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
            AssertExtensions.Throws<AzureXStoreException>(() => this.store.UpdateTableEntity(new TableEntity(PartitionKey, RowKey)).Wait());
        }

        [TestMethod]
        public void TestGetThrowsIfNotFound()
        {
            AssertExtensions.Throws<AzureXStoreException>(() => this.store.GetTableEntity<TableEntity>(PartitionKey, RowKey).Wait());
        }

        [TestMethod]
        public void TestListEntity()
        {
            var entity = new TableEntity(PartitionKey, RowKey);

            this.store.AddTableEntity(entity).Wait();

            Assert.IsTrue(this.store.ListTableEntityByPartitionKey<TableEntity>(PartitionKey).Result.Any());
        }

        [TestMethod]
        public void TestListEntityReturnsEmptyListIfNonExisting()
        {
            Assert.IsFalse(this.store.ListTableEntityByPartitionKey<TableEntity>(PartitionKey).Result.Any());
        }

        [TestMethod]
        public void TestDeleteEntity()
        {
            var entity = new TableEntity(PartitionKey, RowKey);

            this.store.AddTableEntity(entity).Wait();

            this.store.DeleteTableEntity<TableEntity>(PartitionKey, RowKey).Wait();

            Assert.IsFalse(this.store.ListTableEntityByPartitionKey<TableEntity>(PartitionKey).Result.Any());
        }

        [TestMethod]
        public void TestDeleteThrowsIfNotFound()
        {
            AssertExtensions.Throws<AzureXStoreException>(() => this.store.DeleteTableEntity<TableEntity>(PartitionKey, RowKey).Wait());
        }

        [TestMethod]
        public void TestDeleteAllTableEntitiesInPartition()
        {
            this.store.AddTableEntity(new TableEntity(PartitionKey, RowKey)).Wait();

            this.store.DeleteAllTableEntitiesInPartition<TableEntity>(PartitionKey).Wait();

            Assert.IsFalse(this.store.ListTableEntityByPartitionKey<TableEntity>(PartitionKey).Result.Any());
        }

        [TestMethod]
        public void TestListTableEntityByPartitionKey()
        {
            this.store.AddTableEntity(new TableEntity(PartitionKey, RowKey)).Wait();
            this.store.AddTableEntity(new TableEntity(PartitionKey, RowKey + 1)).Wait();

            TableEntity[] existingEntities = this.store.ListTableEntityByPartitionKey<TableEntity>(PartitionKey).Result.ToArray();

            Assert.AreEqual(2, existingEntities.Length);
            Assert.AreEqual(PartitionKey, existingEntities[0].PartitionKey);
            Assert.AreEqual(RowKey, existingEntities[0].RowKey);
            Assert.AreEqual(PartitionKey, existingEntities[1].PartitionKey);
            Assert.AreEqual(RowKey + 1, existingEntities[1].RowKey);
        }

        [TestMethod]
        public void TestListTableEntityByPartitionKeyReturnsEmptyList_WhenThereAreNoEntitiesInStore()
        {
            Assert.IsFalse(this.store.ListTableEntityByPartitionKey<TableEntity>(PartitionKey).Result.Any());
        }

        [TestMethod]
        public void TestListTableEntityInRange()
        {
            this.store.AddTableEntity(new TableEntity(PartitionKey, RowKey)).Wait();
            this.store.AddTableEntity(new TableEntity(PartitionKey, RowKey + 1)).Wait();
            this.store.AddTableEntity(new TableEntity(PartitionKey, RowKey + 2)).Wait();
            this.store.AddTableEntity(new TableEntity(PartitionKey, RowKey + 3)).Wait();

            TableEntity[] existingEntities = this.store.ListTableEntityInRange<TableEntity>(PartitionKey, RowKey + 1, RowKey + 3).Result.ToArray();

            Assert.AreEqual(2, existingEntities.Length);
            Assert.AreEqual(PartitionKey, existingEntities[0].PartitionKey);
            Assert.AreEqual(RowKey + 1, existingEntities[0].RowKey);
            Assert.AreEqual(PartitionKey, existingEntities[1].PartitionKey);
            Assert.AreEqual(RowKey + 2, existingEntities[1].RowKey);

        }

        [TestMethod]
        public void TestListTableEntityInRangeReturnsEmptyList_WhenThereAreNoEntitiesInStore()
        {
            Assert.IsFalse(this.store.ListTableEntityInRange<TableEntity>(PartitionKey, string.Empty, RowKey).Result.Any());
        }

        [TestMethod]
        public void TestGetTableEntityByPartitionKey()
        {
            this.store.AddTableEntity(new TableEntity(PartitionKey, RowKey)).Wait();

            TableEntity entity = this.store.GetTableEntityByPartitionKey<TableEntity>(PartitionKey).Result;

            Assert.AreEqual(PartitionKey, entity.PartitionKey);
            Assert.AreEqual(RowKey, entity.RowKey);
        }

        [TestMethod]
        public void TestGetTableEntityByPartitionKeyThrowsException_WhenThereAreNoEntitiesInStore()
        {
            AssertExtensions.Throws<AzureXStoreException>(() => this.store.GetTableEntityByPartitionKey<TableEntity>(PartitionKey).Wait());
        }

        #region Helper methods and classes

        public class MockTableEntity : TableEntity
        {
            public string Parameter { get; set; }
        }

        #endregion
    }
}
