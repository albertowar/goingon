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
    using System.Collections.Generic;
    using System.Linq;
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

        public IEnumerable<NewsBll> GetNews()
        {
            return this.storage.Select(NewsMemory.ToNewsBll).ToList();
        }

        public void AddNews(NewsBll newsBll)
        {
            storage.Add(NewsMemory.FromNewsBll(newsBll));
        }

        public void UpdateNews(NewsBll newsBll)
        {

        }

        public void DeleteNews(NewsBll newsBll)
        {
            storage.Remove(NewsMemory.FromNewsBll(newsBll));
        }

        public void DeleteAllNews()
        {
            storage.Clear();
        }

        public bool ContainsNews(NewsBll newsBll)
        {
            return storage.Contains(NewsMemory.FromNewsBll(newsBll));
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
