// ****************************************************************************
// <copyright file="HotNewsCreator.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace GoingOn.HotNewsCreatorWorkerRole
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Threading.Tasks;

    using GoingOn.Model.EntitiesBll;
    using GoingOn.Storage;
    using GoingOn.Storage.TableStorage;

    using Microsoft.WindowsAzure.Storage;

    public class HotNewsCreator
    {
        private INewsStorage newsStorage;

        private INewsStorage hotNewsStorage;

        public HotNewsCreator()
        {
            string storageConnectionString = ConfigurationManager.AppSettings["StorageConnectionString"];

            string newsTableName = ConfigurationManager.AppSettings["NewsTableName"];
            string hotNewsTableName = ConfigurationManager.AppSettings["HotNewsTableName"];

            this.newsStorage = new NewsTableStorage(storageConnectionString, newsTableName);
            this.hotNewsStorage = new NewsTableStorage(storageConnectionString, hotNewsTableName);
        }

        public async Task RunAsync()
        {
            IEnumerable<NewsBll> hotNews = await this.GetHotNews();

            this.PushHotNews(hotNews);
        }

        public async Task<IEnumerable<NewsBll>> GetHotNews()
        {
            DateTime date = DateTime.Today;

            var news = new List<NewsBll>();

            try
            {
                news = (await this.newsStorage.GetNews("Malaga", date)).ToList();

                while (!news.Any())
                {
                    date = date.AddDays(-1);
                    news = (await this.newsStorage.GetNews("Malaga", date)).ToList();
                }
            }
            catch (Exception)
            {
            }

            return news;
        }

        public void PushHotNews(IEnumerable<NewsBll> hotNewsCollection)
        {
            Parallel.ForEach(hotNewsCollection, async hotNews =>
            {
                try
                {
                    await this.hotNewsStorage.AddNews(hotNews);
                }
                catch (StorageException)
                {
                    
                }
            });
        } 
    }
}
