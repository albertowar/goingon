// ****************************************************************************
// <copyright file="NewsTableStorage.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace Storage.TableStorage
{
    using System;
    using System.Configuration;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;
    using Microsoft.WindowsAzure.Storage.Table.Queryable;

    using Model.EntitiesBll;
    using Storage.TableStorage.Entities;

    public class NewsTableStorage : INewsStorage
    {
        // Configuration info
        private static readonly string TableName = ConfigurationManager.AppSettings["NewsTableName"];
        private static readonly string StorageConnectionString = ConfigurationManager.AppSettings["StorageConnectionString"];

        // Singleton pattern
        private static NewsTableStorage  instance;

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

            var partitionKeyFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.GreaterThanOrEqual, NewsEntity.BuildPartitionkey(city, date));
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
                throw new Storage.StorageException("The news is not in the database");
            }

            return NewsEntity.ToNewsBll(element);
        }

        public Task<bool> Exists(string city, DateTime date, Guid id)
        {
            return Task.Run(() =>{
                var tableClient = this.storageAccount.CreateCloudTableClient();

                var table = tableClient.GetTableReference(TableName);

                var query = table.CreateQuery<NewsEntity>().Where(news => news.RowKey == id.ToString()).AsTableQuery();

                var retrievedResult = query.Execute();

                return retrievedResult.Any();
            });
        }

        public Task<bool> IsAuthorOf(string city, DateTime date, Guid id, string author)
        {
            return Task.Run(() =>
            {
                var tableClient = this.storageAccount.CreateCloudTableClient();

                var table = tableClient.GetTableReference(TableName);

                var query = table.CreateQuery<NewsEntity>().Where(news => news.RowKey == id.ToString() && news.Author == author).AsTableQuery();

                var retrievedResult = query.Execute();

                return retrievedResult.Any();
            });
        }

        public Task<bool> ContainsNews(NewsBll newsBll)
        {
            return Task.Run(() =>
            {
                var tableClient = this.storageAccount.CreateCloudTableClient();

                var table = tableClient.GetTableReference(TableName);

                var partitionKeyFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "World");
                var titleFilter = TableQuery.GenerateFilterCondition("Title", QueryComparisons.Equal, newsBll.Title);
                var authorFilter = TableQuery.GenerateFilterCondition("Author", QueryComparisons.Equal, newsBll.Author);

                // TODO: a proper date filter
                //var dateFilter = TableQuery.GenerateFilterCondition("Date", QueryComparisons.Equal, new DateTime(newsBll.Date.Year, newsBll.Date.Month, newsBll.Date.Day, newsBll.Date.Hour, newsBll.Date.Minute, newsBll.Date.Second).ToString());
                var filter = TableQuery.CombineFilters(
                    partitionKeyFilter,
                    TableOperators.And,
                    TableQuery.CombineFilters(
                        titleFilter,
                        TableOperators.And,
                        authorFilter));

                var query = new TableQuery<UserEntity>().Where(filter);

                return table.ExecuteQuery(query).Any();
            });
        }

        public Task UpdateNews(NewsBll newsBll)
        {
            return Task.Run(() =>
            {
                var tableClient = this.storageAccount.CreateCloudTableClient();

                var table = tableClient.GetTableReference(TableName);

                //TODO: remove the WORLD
                var retrieveOperation = TableOperation.Retrieve<NewsEntity>("World", newsBll.Id.ToString());

                var query = table.CreateQuery<NewsEntity>().Where(news => news.RowKey == newsBll.ToString()).AsTableQuery();

                var retrievedResult = table.Execute(retrieveOperation);

                var updateEntity = retrievedResult.Result as NewsEntity;

                if (updateEntity != null)
                {
                    updateEntity.Merge(NewsEntity.FromNewsBll(newsBll));

                    var insertOrReplaceOperation = TableOperation.InsertOrReplace(updateEntity);

                    table.Execute(insertOrReplaceOperation);
                }
            });
        }

        public Task DeleteNews(string city, DateTime date, Guid id)
        {
            return Task.Run(() =>
            {
                var tableClient = this.storageAccount.CreateCloudTableClient();

                var table = tableClient.GetTableReference(TableName);

                //TODO: remove the WORLD
                var retrieveOperation = TableOperation.Retrieve<NewsEntity>("World", id.ToString());

                var retrievedResult = table.Execute(retrieveOperation);

                var deleteEntity = retrievedResult.Result as NewsEntity;

                if (deleteEntity != null)
                {
                    var deleteOperation = TableOperation.Delete(deleteEntity);

                    table.Execute(deleteOperation);
                }
            });
        }
    }
}
