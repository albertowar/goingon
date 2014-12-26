// ****************************************************************************
// <copyright file="NewsMemoryStorage.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace MemoryStorage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using MemoryStorage.Entities;
    using Model.EntitiesBll;

    public class NewsMemoryStorage : INewsStorage
    {
        // Singleton pattern
        private static NewsMemoryStorage instance;

        private readonly List<NewsMemory> storage;

        private NewsMemoryStorage()
        {
            this.storage = new List<NewsMemory>();
        }

        public Task AddNews(NewsBll newsBll)
        {
            storage.Add(NewsMemory.FromNewsBll(newsBll));

            return Task.FromResult(0);
        }

        public Task<NewsBll> GetNews(Guid id)
        {
            if (storage.Any(news => news.Id.Equals(id)))
            {
                return Task.FromResult(NewsMemory.ToNewsBll(this.storage.First(news => news.Id.Equals(id))));
            }

            return Task.FromResult<NewsBll>(null);
        }

        public Task<bool> ContainsNews(Guid id)
        {
            return Task.FromResult(storage.Any(news => news.Id.Equals(id)));
        }

        public Task<bool> ContainsNews(NewsBll newsBll)
        {
            return Task.FromResult(storage.Any(news => new NewsMemoryEqualityComparer().Equals(news, NewsMemory.FromNewsBll(newsBll))));
        }

        public Task UpdateNews(Guid id, NewsBll newsBll)
        {
            var newsMemory = NewsMemory.FromNewsBll(newsBll);

            var foundNews = storage.Find(news => news.Equals(newsMemory));

            if (foundNews != null)
            {
                foundNews.Merge(newsMemory);
            }

            return Task.FromResult(0);
        }

        public Task DeleteNews(Guid id)
        {
            storage.Remove(new NewsMemory(id));

            return Task.FromResult(0);
        }

        public Task DeleteAllNews()
        {
            storage.Clear();

            return Task.FromResult(0);
        }

        public static NewsMemoryStorage GetInstance()
        {
            if (NewsMemoryStorage.instance == null)
            {
                NewsMemoryStorage.instance = new NewsMemoryStorage();
            }

            return NewsMemoryStorage.instance;
        }
    }
}
