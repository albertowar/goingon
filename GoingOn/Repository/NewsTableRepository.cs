// ****************************************************************************
// <copyright file="NewsTableRepository.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// Manages the repository of the news
// </summary>
// ****************************************************************************

namespace GoingOn.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using GoingOn.Model.EntitiesBll;
    using GoingOn.Repository.Entities;
    using GoingOn.XStoreProxy;
    using GoingOn.XStoreProxy.TableStore;

    public class NewsTableRepository : INewsRepository, IHotNewsRepository
    {
        private readonly ITableStore _tableStore;

        public NewsTableRepository(string connectionString, string tableName)
        {
            this._tableStore = new TableStore(connectionString, tableName);
        }

        public async Task AddNews(NewsEntity newsEntity)
        {
            await this._tableStore.AddTableEntity(newsEntity);
        }

        public async Task<NewsBll> GetNews(string city, DateTime date, Guid id)
        {
            return NewsEntity.ToNewsBll(await this._tableStore.GetTableEntity<NewsEntity>(NewsEntity.BuildPartitionkey(city, date), id.ToString()));
        }

        public async Task<IEnumerable<NewsBll>> ListNews(string city, DateTime date)
        {
            List<NewsEntity> newsList = (await this._tableStore.ListTableEntity<NewsEntity>(NewsEntity.BuildPartitionkey(city, date))).ToList();

            if (!newsList.Any())
            {
                throw new AzureTableStorageException("The news is not in the database");
            }

            return newsList.Select(NewsEntity.ToNewsBll);
        }

        public async Task<bool> ContainsAnyHotNews(string city, DateTime date)
        {
            List<NewsEntity> newsList = (await this._tableStore.ListTableEntity<NewsEntity>(NewsEntity.BuildPartitionkey(city, date))).ToList();

            if (!newsList.Any())
            {
                throw new AzureTableStorageException("The news is not in the database");
            }

            return newsList.Any();
        }

        public async Task<bool> ContainsNews(string city, DateTime date, Guid id)
        {
            try
            {
                await this.GetNews(city, date, id);

                return true;
            }
            catch (AzureTableStorageException)
            {
                return false;
            }
        }

        public async Task<bool> IsAuthorOf(string city, DateTime date, Guid id, string author)
        {
            NewsBll news = await this.GetNews(city, date, id);

            return string.Equals(news.Author, author);
        }

        public async Task<bool> ContainsNewsCheckContent(NewsEntity newsEntity)
        {
            IEnumerable<NewsEntity> newsEnumeration = (await this._tableStore.ListTableEntity<NewsEntity>(newsEntity.PartitionKey));

            return newsEnumeration.Any(news => string.Equals(newsEntity.Title, news.Title) && string.Equals(newsEntity.Author, news.Author));
        }

        public async Task UpdateNews(NewsBll newsBll)
        {
            await this._tableStore.UpdateTableEntity<NewsEntity>(NewsEntity.FromNewsBll(newsBll));
        }

        public async Task DeleteNews(string city, DateTime date, Guid id)
        {
            await this._tableStore.DeleteTableEntity<NewsEntity>(NewsEntity.BuildPartitionkey(city, date), id.ToString());
        }

        public async Task DeleteAllNews(string city)
        {
            await this._tableStore.DeleteAllTableEntitiesInPartition<NewsEntity>(city);
        }
    }
}
