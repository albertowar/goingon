// ****************************************************************************
// <copyright file="ITableStorage.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// News repository base interface
// </summary>
// ****************************************************************************

namespace GoingOn.XStoreProxy.TableStore
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using GoingOn.XStoreProxy;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;

    public class TableStore : ITableStore
    {
        // Configuration info
        private readonly string tableName;
        private readonly CloudStorageAccount storageAccount;

        public TableStore(string connectionString, string tableName)
        {
            this.tableName = tableName;

            try
            {
                this.storageAccount = CloudStorageAccount.Parse(connectionString);
            }
            catch (Exception e)
            {
                throw new AzureXStoreException(string.Format("The repository account could not be created. Error: {0}", e.Message));
            }
        }

        public async Task AddTableEntity(ITableEntity entity)
        {
            CloudTable table = this.GetStorageTable();

            await table.ExecuteAsync(TableOperation.Insert(entity));
        }

        public async Task UpdateTableEntity<T>(T entity) where T : ITableEntity, new()
        {
            CloudTable table = this.GetStorageTable();

            await this.GetTableEntity<T>(entity.PartitionKey, entity.RowKey);

            TableOperation insertOrReplaceOperation = TableOperation.InsertOrReplace(entity);

            await table.ExecuteAsync(insertOrReplaceOperation);
        }

        public async Task<T> GetTableEntity<T>(string partitionKey, string rowKey) where T : ITableEntity, new()
        {
            CloudTable table = this.GetStorageTable();

            string partitionKeyFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey);
            string rowKeyFilter = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, rowKey);

            string filter = TableQuery.CombineFilters(
                partitionKeyFilter,
                TableOperators.And,
                rowKeyFilter);

            TableQuery<T> newsQuery = new TableQuery<T>().Where(filter);

            TableQuerySegment<T> retrievedNews = await table.ExecuteQuerySegmentedAsync(newsQuery, null);

            T element = retrievedNews.FirstOrDefault();

            if (element == null)
            {
                throw new AzureXStoreException("The entity is not in the store.");
            }

            return element;
        }

        public async Task<T> GetTableEntityByRowKey<T>(string rowKey) where T : ITableEntity, new()
        {
            // TODO: add tests
            CloudTable table = this.GetStorageTable();

            string rowKeyFilter = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, rowKey);

            TableQuery<T> newsQuery = new TableQuery<T>().Where(rowKeyFilter);

            TableQuerySegment<T> retrievedNews = await table.ExecuteQuerySegmentedAsync(newsQuery, null);

            T element = retrievedNews.FirstOrDefault();

            if (element == null)
            {
                throw new AzureXStoreException("The entity is not in the store.");
            }

            return element;
        }

        public async Task<IEnumerable<T>> ListTableEntity<T>(string partitionKey) where T : ITableEntity, new()
        {
            CloudTable table = this.GetStorageTable();

            string partitionKeyFilter = TableQuery.GenerateFilterCondition(
                "PartitionKey", 
                QueryComparisons.Equal,
                partitionKey);

            TableQuery<T> query = new TableQuery<T>().Where(partitionKeyFilter);

            var tableEntities = new List<T>();

            TableContinuationToken token = null;

            do
            {
                TableQuerySegment<T> tableEntitySegment = await table.ExecuteQuerySegmentedAsync(query, token);

                tableEntities.AddRange(tableEntitySegment);

                token = tableEntitySegment.ContinuationToken;

            } while (token != null);

            return tableEntities;
        }

        public async Task DeleteTableEntity<T>(string partitionKey, string rowKey) where T : ITableEntity, new()
        {
            T entity = await this.GetTableEntity<T>(partitionKey, rowKey);

            CloudTable table = this.GetStorageTable();

            TableOperation deleteOperation = TableOperation.Delete(entity);

            await table.ExecuteAsync(deleteOperation);
        }

        public async Task DeleteAllTableEntitiesInPartition<T>(string partitionKey) where T : ITableEntity, new()
        {
            CloudTable table = this.GetStorageTable();

            string partitionKeyFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.GreaterThanOrEqual, partitionKey);

            TableQuery<T> query = new TableQuery<T>().Where(partitionKeyFilter);

            TableContinuationToken token = null;

            do
            {
                TableQuerySegment<T> tableEntitySegment = await table.ExecuteQuerySegmentedAsync(query, token);

                foreach (T tableEntity in tableEntitySegment)
                {
                    await table.ExecuteAsync(TableOperation.Delete(tableEntity));
                }

                token = tableEntitySegment.ContinuationToken;
            } while (token != null);
        }

        #region Helper methods

        private CloudTable GetStorageTable()
        {
            CloudTableClient tableClient = this.storageAccount.CreateCloudTableClient();

            return tableClient.GetTableReference(this.tableName);
        }

        #endregion
    }
}
