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
            var table = this.GetStorageTable();

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
                throw new AzureTableStorageException("The news is not in the database");
            }

            return NewsEntity.ToNewsBll(element);
        }

        public async Task<bool> Exists(string city, DateTime date, Guid id)
        {
            var table = this.GetStorageTable();

            var retrieveOperation = TableOperation.Retrieve<NewsEntity>(NewsEntity.BuildPartitionkey(city, date), id.ToString());

            var retrievedResult = await table.ExecuteAsync(retrieveOperation);

            return retrievedResult.Result != null;
        }

        public async Task<bool> IsAuthorOf(string city, DateTime date, Guid id, string author)
        {
            var table = this.GetStorageTable();

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
            var table = this.GetStorageTable();

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
            var table = this.GetStorageTable();

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
            var table = this.GetStorageTable();

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
            var table = this.GetStorageTable();

            var partitionKeyFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.GreaterThanOrEqual, city);

            var newsQuery = new TableQuery<NewsEntity>().Where(partitionKeyFilter);

            var newsSegment = await table.ExecuteQuerySegmentedAsync(newsQuery, null);

            Parallel.ForEach(newsSegment.Results, async user =>
            {
                await table.ExecuteAsync(TableOperation.Delete(user));
            });
        }

        #region Helper methods

        private CloudTable GetStorageTable()
        {
            var tableClient = this.storageAccount.CreateCloudTableClient();

            return tableClient.GetTableReference(this.tableName);
        }

        #endregion
    }
}
