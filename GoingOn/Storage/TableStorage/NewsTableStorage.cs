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
    using System.Configuration;
    using System.Linq;
    using System.Threading.Tasks;
    using GoingOn.Storage;
    using GoingOn.Storage.TableStorage.Entities;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;
    using Model.EntitiesBll;
    using StorageException = GoingOn.Storage.StorageException;

    public class NewsTableStorage : INewsStorage
    {
        // Configuration info
        private static readonly string TableName = ConfigurationManager.AppSettings["NewsTableName"];
        private static readonly string StorageConnectionString = ConfigurationManager.AppSettings["StorageConnectionString"];

        // Singleton pattern
        private static NewsTableStorage instance;

        // Retrieve the storage account from the connection string.
        private readonly CloudStorageAccount storageAccount = CloudStorageAccount.Parse(StorageConnectionString);

        private NewsTableStorage()
        {
        }

        public static NewsTableStorage GetInstance()
        {
            return NewsTableStorage.instance ?? (NewsTableStorage.instance = new NewsTableStorage());
        }

        public async Task AddNews(NewsBll newsBll)
        {
            var tableClient = this.storageAccount.CreateCloudTableClient();

            var table = tableClient.GetTableReference(TableName);

            await table.ExecuteAsync(TableOperation.InsertOrReplace(NewsEntity.FromNewsBll(newsBll)));
        }

        public async Task<NewsBll> GetNews(string city, DateTime date, Guid id)
        {
            var tableClient = this.storageAccount.CreateCloudTableClient();

            var table = tableClient.GetTableReference(TableName);

            var partitionKeyFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, NewsEntity.BuildPartitionkey(city, date));
            var rowKeyFilter = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, id.ToString());

            var filter = TableQuery.CombineFilters(
                partitionKeyFilter,
                TableOperators.And,
                rowKeyFilter);

            var newsQuery = new TableQuery<NewsEntity>().Where(filter);

            var retrievedNews = await table.ExecuteQuerySegmentedAsync(newsQuery, null);

            var element = retrievedNews.FirstOrDefault();

            if (element == null)
            {
                throw new StorageException("The news is not in the database");
            }

            return NewsEntity.ToNewsBll(element);
        }

        public async Task<bool> Exists(string city, DateTime date, Guid id)
        {
            var tableClient = this.storageAccount.CreateCloudTableClient();

            var table = tableClient.GetTableReference(TableName);

            var retrieveOperation = TableOperation.Retrieve<NewsEntity>(NewsEntity.BuildPartitionkey(city, date), id.ToString());

            var retrievedResult = await table.ExecuteAsync(retrieveOperation);

            return retrievedResult.Result != null;
        }

        public async Task<bool> IsAuthorOf(string city, DateTime date, Guid id, string author)
        {
            var tableClient = this.storageAccount.CreateCloudTableClient();

            var table = tableClient.GetTableReference(TableName);

            var partitionKeyFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, NewsEntity.BuildPartitionkey(city, date));
            var rowKey = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, id.ToString());
            var authorFilter = TableQuery.GenerateFilterCondition("Author", QueryComparisons.Equal, author);

            var filter = TableQuery.CombineFilters(
                partitionKeyFilter,
                TableOperators.And,
                TableQuery.CombineFilters(
                    rowKey,
                    TableOperators.And,
                    authorFilter));

            var newsQuery = new TableQuery<NewsEntity>().Where(filter);

            var result = await table.ExecuteQuerySegmentedAsync(newsQuery, null);

            return result.Any();
        }

        public async Task<bool> ContainsNews(NewsBll newsBll)
        {
            var tableClient = this.storageAccount.CreateCloudTableClient();

            var table = tableClient.GetTableReference(TableName);

            var partitionKeyFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, NewsEntity.BuildPartitionkey(newsBll.City, newsBll.Date));
            var titleFilter = TableQuery.GenerateFilterCondition("Title", QueryComparisons.Equal, newsBll.Title);
            var authorFilter = TableQuery.GenerateFilterCondition("Author", QueryComparisons.Equal, newsBll.Author);

            var filter = TableQuery.CombineFilters(
                partitionKeyFilter,
                TableOperators.And,
                TableQuery.CombineFilters(
                    titleFilter,
                    TableOperators.And,
                    authorFilter));

            var newsQuery = new TableQuery<NewsEntity>().Where(filter);

            var result = await table.ExecuteQuerySegmentedAsync(newsQuery, null);

            return result.Any();
        }

        public async Task UpdateNews(NewsBll newsBll)
        {
            var tableClient = this.storageAccount.CreateCloudTableClient();

            var table = tableClient.GetTableReference(TableName);

            var retrieveOperation = TableOperation.Retrieve<NewsEntity>(NewsEntity.BuildPartitionkey(newsBll.City, newsBll.Date), newsBll.Id.ToString());

            var retrievedNews = await table.ExecuteAsync(retrieveOperation);

            var updateEntity = retrievedNews.Result as NewsEntity;

            if (updateEntity != null)
            {
                updateEntity.Merge(NewsEntity.FromNewsBll(newsBll));

                var insertOrReplaceOperation = TableOperation.InsertOrReplace(updateEntity);

                await table.ExecuteAsync(insertOrReplaceOperation);
            }
        }

        public async Task DeleteNews(string city, DateTime date, Guid id)
        {
            var tableClient = this.storageAccount.CreateCloudTableClient();

            var table = tableClient.GetTableReference(TableName);

            var retrieveOperation = TableOperation.Retrieve<NewsEntity>(NewsEntity.BuildPartitionkey(city,date), id.ToString());

            var retrievedResult = await table.ExecuteAsync(retrieveOperation);

            var deleteEntity = retrievedResult.Result as NewsEntity;

            if (deleteEntity != null)
            {
                var deleteOperation = TableOperation.Delete(deleteEntity);

                await table.ExecuteAsync(deleteOperation);
            }
        }

        public async Task DeleteAllNews(string city)
        {
            var tableClient = this.storageAccount.CreateCloudTableClient();

            var table = tableClient.GetTableReference(TableName);

            var partitionKeyFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.GreaterThanOrEqual, city);

            var newsQuery = new TableQuery<NewsEntity>().Where(partitionKeyFilter);

            var result = await table.ExecuteQuerySegmentedAsync(newsQuery, null);

            foreach (var news in result)
            {
                var deleteOperation = TableOperation.Delete(news);

                await table.ExecuteAsync(deleteOperation);
            }
        }
    }
}
