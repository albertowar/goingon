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
            if (NewsTableStorage.instance == null)
            {
                NewsTableStorage.instance = new NewsTableStorage();
            }

            return NewsTableStorage.instance;
        }

        public async Task AddNews(NewsBll newsBll)
        {
            await Task.Run(() =>
            {
                var tableClient = storageAccount.CreateCloudTableClient();

                var table = tableClient.GetTableReference(TableName);

                var news = NewsEntity.FromNewsBll(newsBll);

                var insertOperation = TableOperation.Insert(news);

                table.Execute(insertOperation);
            });
        }

        public async Task<NewsBll> GetNews(Guid id)
        {
            return await Task.Run(() =>
            {
                var tableClient = storageAccount.CreateCloudTableClient();

                var table = tableClient.GetTableReference(TableName);

                var retrieveOperation = TableOperation.Retrieve<NewsEntity>("World", id.ToString());

                var retrievedResult = table.Execute(retrieveOperation);

                if (retrievedResult.Result == null)
                {
                    throw new Storage.StorageException("The news is not in the database");
                }

                return NewsEntity.ToNewsBll(retrievedResult.Result as NewsEntity);
            });
        }

        public async Task<bool> ContainsNews(Guid id)
        {
            return await Task.Run(() =>
            {
                var tableClient = storageAccount.CreateCloudTableClient();

                var table = tableClient.GetTableReference(TableName);

                var retrieveOperation = TableOperation.Retrieve<NewsEntity>("World", id.ToString());

                var retrievedResult = table.Execute(retrieveOperation);

                return retrievedResult.Result != null;
            });
        }

        public async Task<bool> ContainsNews(Guid id, string author)
        {
            return await Task.Run(() =>
            {
                var tableClient = storageAccount.CreateCloudTableClient();

                var table = tableClient.GetTableReference(TableName);

                var retrieveOperation = TableOperation.Retrieve<NewsEntity>("World", id.ToString());

                var retrievedResult = table.Execute(retrieveOperation);

                return
                    retrievedResult.Result != null &&
                    string.Equals((retrievedResult.Result as NewsEntity).Author, author, StringComparison.Ordinal);
            });
        }

        public async Task<bool> ContainsNews(NewsBll newsBll)
        {
            return await Task.Run(() =>
            {
                var tableClient = storageAccount.CreateCloudTableClient();

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

        public async Task UpdateNews(NewsBll newsBll)
        {
            await Task.Run(() =>
            {
                var tableClient = storageAccount.CreateCloudTableClient();

                var table = tableClient.GetTableReference(TableName);

                var retrieveOperation = TableOperation.Retrieve<NewsEntity>("World", newsBll.Id.ToString());

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

        public async Task DeleteNews(Guid id)
        {
            await Task.Run(() =>
            {
                var tableClient = storageAccount.CreateCloudTableClient();

                var table = tableClient.GetTableReference(TableName);

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

        public async Task DeleteAllNews()
        {
            await Task.Run(() =>
            {
                var tableClient = storageAccount.CreateCloudTableClient();

                var table = tableClient.GetTableReference(TableName);

                var query = new TableQuery<UserEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "World"));

                foreach (var entity in table.ExecuteQuery(query))
                {
                    var deleteOperation = TableOperation.Delete(entity);
                    table.Execute(deleteOperation);
                }
            });
        }
    }
}
