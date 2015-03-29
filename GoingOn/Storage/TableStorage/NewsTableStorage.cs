// ****************************************************************************
// <copyright file="NewsTableStorage.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace GoingOn.Storage.TableStorage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using GoingOn.Model.EntitiesBll;
    using GoingOn.Storage;
    using GoingOn.Storage.TableStorage.Entities;

    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;

    public class NewsTableStorage : INewsStorage
    {
        // Configuration info
        private readonly string tableName;
        private readonly CloudStorageAccount storageAccount;

        public NewsTableStorage(string connectionString, string tableName)
        {
            this.tableName = tableName;

            try
            {
                this.storageAccount = CloudStorageAccount.Parse(connectionString);
            }
            catch (Exception e)
            {
                throw new AzureTableStorageException(string.Format("The storage account could not be created. Erro: {0}", e.Message));
            }
        }

        public async Task AddNews(NewsBll newsBll)
        {
            var table = this.GetStorageTable();

            await table.ExecuteAsync(TableOperation.InsertOrReplace(NewsEntity.FromNewsBll(newsBll)));
        }

        public async Task<NewsBll> GetNews(string city, DateTime date, Guid id)
        {
            CloudTable table = this.GetStorageTable();

            string partitionKeyFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, NewsEntity.BuildPartitionkey(city, date));
            string rowKeyFilter = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, id.ToString());

            string filter = TableQuery.CombineFilters(
                partitionKeyFilter,
                TableOperators.And,
                rowKeyFilter);

            TableQuery<NewsEntity> newsQuery = new TableQuery<NewsEntity>().Where(filter);

            TableQuerySegment<NewsEntity> retrievedNews = await table.ExecuteQuerySegmentedAsync(newsQuery, null);

            var element = retrievedNews.FirstOrDefault();

            if (element == null)
            {
                throw new AzureTableStorageException("The news is not in the database");
            }

            return NewsEntity.ToNewsBll(element);
        }

        public async Task<IEnumerable<NewsBll>> GetNews(string city, DateTime date)
        {
            CloudTable table = this.GetStorageTable();

            string partitionKeyFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, NewsEntity.BuildPartitionkey(city, date));

            TableQuery<NewsEntity> newsQuery = new TableQuery<NewsEntity>().Where(partitionKeyFilter);

            TableQuerySegment<NewsEntity> retrievedNews = await table.ExecuteQuerySegmentedAsync(newsQuery, null);

            NewsEntity element = retrievedNews.FirstOrDefault();

            if (element == null)
            {
                throw new AzureTableStorageException("The news is not in the database");
            }

            return retrievedNews.Select(newsEntity => NewsEntity.ToNewsBll(newsEntity));
        }

        public async Task<bool> Exists(string city, DateTime date, Guid id)
        {
            CloudTable table = this.GetStorageTable();

            TableOperation retrieveOperation = TableOperation.Retrieve<NewsEntity>(NewsEntity.BuildPartitionkey(city, date), id.ToString());

            TableResult retrievedResult = await table.ExecuteAsync(retrieveOperation);

            return retrievedResult.Result != null;
        }

        public async Task<bool> IsAuthorOf(string city, DateTime date, Guid id, string author)
        {
            CloudTable table = this.GetStorageTable();

            string partitionKeyFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, NewsEntity.BuildPartitionkey(city, date));
            string rowKey = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, id.ToString());
            string authorFilter = TableQuery.GenerateFilterCondition("Author", QueryComparisons.Equal, author);

            string filter = TableQuery.CombineFilters(
                partitionKeyFilter,
                TableOperators.And,
                TableQuery.CombineFilters(
                    rowKey,
                    TableOperators.And,
                    authorFilter));

            TableQuery<NewsEntity> newsQuery = new TableQuery<NewsEntity>().Where(filter);

            TableQuerySegment<NewsEntity> result = await table.ExecuteQuerySegmentedAsync(newsQuery, null);

            return result.Any();
        }

        public async Task<bool> ContainsNews(NewsBll newsBll)
        {
            CloudTable table = this.GetStorageTable();

            string partitionKeyFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, NewsEntity.BuildPartitionkey(newsBll.City, newsBll.Date));
            string titleFilter = TableQuery.GenerateFilterCondition("Title", QueryComparisons.Equal, newsBll.Title);
            string authorFilter = TableQuery.GenerateFilterCondition("Author", QueryComparisons.Equal, newsBll.Author);

            string filter = TableQuery.CombineFilters(
                partitionKeyFilter,
                TableOperators.And,
                TableQuery.CombineFilters(
                    titleFilter,
                    TableOperators.And,
                    authorFilter));

            TableQuery<NewsEntity> newsQuery = new TableQuery<NewsEntity>().Where(filter);

            TableQuerySegment<NewsEntity> result = await table.ExecuteQuerySegmentedAsync(newsQuery, null);

            return result.Any();
        }

        public async Task UpdateNews(NewsBll newsBll)
        {
            CloudTable table = this.GetStorageTable();

            TableOperation retrieveOperation = TableOperation.Retrieve<NewsEntity>(NewsEntity.BuildPartitionkey(newsBll.City, newsBll.Date), newsBll.Id.ToString());

            TableResult retrievedNews = await table.ExecuteAsync(retrieveOperation);

            var updateEntity = retrievedNews.Result as NewsEntity;

            if (updateEntity != null)
            {
                updateEntity.Merge(NewsEntity.FromNewsBll(newsBll));

                TableOperation insertOrReplaceOperation = TableOperation.InsertOrReplace(updateEntity);

                await table.ExecuteAsync(insertOrReplaceOperation);
            }
        }

        public async Task DeleteNews(string city, DateTime date, Guid id)
        {
            CloudTable table = this.GetStorageTable();

            TableOperation retrieveOperation = TableOperation.Retrieve<NewsEntity>(NewsEntity.BuildPartitionkey(city,date), id.ToString());

            TableResult retrievedResult = await table.ExecuteAsync(retrieveOperation);

            var deleteEntity = retrievedResult.Result as NewsEntity;

            if (deleteEntity != null)
            {
                TableOperation deleteOperation = TableOperation.Delete(deleteEntity);

                await table.ExecuteAsync(deleteOperation);
            }
        }

        public async Task DeleteAllNews(string city)
        {
            CloudTable table = this.GetStorageTable();

            string partitionKeyFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.GreaterThanOrEqual, city);

            TableQuery<NewsEntity> newsQuery = new TableQuery<NewsEntity>().Where(partitionKeyFilter);

            TableQuerySegment<NewsEntity> newsSegment = await table.ExecuteQuerySegmentedAsync(newsQuery, null);

            Parallel.ForEach(newsSegment.Results, async user =>
            {
                await table.ExecuteAsync(TableOperation.Delete(user));
            });
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
