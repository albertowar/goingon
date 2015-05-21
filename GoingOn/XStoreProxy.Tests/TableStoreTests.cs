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
            this.store.DeleteAllTableEntitiesInPartition<TableEntity>(PartitionKey);
        }

        [TestMethod]
        public void TestAddEntity()
        {
            var entity = new TableEntity(PartitionKey, RowKey);
            this.store.AddTableEntity(entity).Wait();

            TableEntity retrievedEntity = this.store.GetTableEntity<TableEntity>(PartitionKey, RowKey).Result;

            Assert.IsNotNull(retrievedEntity);
        }
    }
}
