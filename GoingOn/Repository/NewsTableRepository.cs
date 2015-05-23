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
    using GoingOn.XStoreProxy;
    using GoingOn.XStoreProxy.Entities;
    using GoingOn.XStoreProxy.TableStore;

    public class NewsTableRepository : INewsRepository, IHotNewsRepository
    {
        private readonly ITableStore tableStore;

        public NewsTableRepository(ITableStore tableStore)
        {
            this.tableStore = tableStore;
        }

        public async Task AddNews(NewsBll newsBll)
        {
            await this.tableStore.AddTableEntity(NewsEntity.FromNewsBll(newsBll));
        }

        public async Task<NewsBll> GetNews(string city, DateTime date, Guid id)
        {
            return NewsEntity.ToNewsBll(await this.tableStore.GetTableEntity<NewsEntity>(NewsEntity.BuildPartitionkey(city, date), id.ToString()));
        }

        public async Task<IEnumerable<NewsBll>> ListNews(string city, DateTime date)
        {
            return 
                (await this.tableStore.ListTableEntity<NewsEntity>(NewsEntity.BuildPartitionkey(city, date)))
                .Select(NewsEntity.ToNewsBll);
        }

        public async Task<bool> ContainsAnyHotNews(string city, DateTime date)
        {
            List<NewsEntity> newsList = (await this.tableStore.ListTableEntity<NewsEntity>(NewsEntity.BuildPartitionkey(city, date))).ToList();

            return newsList.Any();
        }

        public async Task<bool> ContainsNews(string city, DateTime date, Guid id)
        {
            try
            {
                await this.GetNews(city, date, id);

                return true;
            }
            catch (AzureXStoreException)
            {
                return false;
            }
        }

        public async Task<bool> IsAuthorOf(string city, DateTime date, Guid id, string author)
        {
            NewsBll news = await this.GetNews(city, date, id);

            return string.Equals(news.Author, author);
        }

        public async Task<bool> ContainsNewsCheckContent(NewsBll newsBll)
        {
            return 
                (await this.tableStore.ListTableEntity<NewsEntity>(NewsEntity.FromNewsBll(newsBll).PartitionKey))
                .Any(news => string.Equals(newsBll.Title, news.Title) && string.Equals(newsBll.Author, news.Author));
        }

        public async Task UpdateNews(NewsBll newsBll)
        {
            await this.tableStore.UpdateTableEntity(NewsEntity.FromNewsBll(newsBll));
        }

        public async Task DeleteNews(string city, DateTime date, Guid id)
        {
            await this.tableStore.DeleteTableEntity<NewsEntity>(NewsEntity.BuildPartitionkey(city, date), id.ToString());
        }

        public async Task DeleteAllNews(string city)
        {
            await this.tableStore.DeleteAllTableEntitiesInPartition<NewsEntity>(city);
        }
    }
}
