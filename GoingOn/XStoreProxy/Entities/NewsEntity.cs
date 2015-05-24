// ****************************************************************************
// <copyright file="NewsEntity.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// Encapsulates the news information that will be stored
// </summary>
// ****************************************************************************

namespace GoingOn.XStoreProxy.Entities
{
    using System;
    using GoingOn.Model.EntitiesBll;
    using Microsoft.WindowsAzure.Storage.Table;

    public class NewsEntity : TableEntity
    {
        /// <summary>
        /// The title of the news.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The content of the news.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// The author of the news.
        /// </summary>
        public string Author { get; set; }

        public static NewsEntity FromNewsBll(NewsBll newsBll)
        {
            return new NewsEntity
            {
                PartitionKey = NewsEntity.BuildPartitionkey(newsBll.City, newsBll.Date),
                RowKey = NewsEntity.BuildRowKey(newsBll.Id),
                Title = newsBll.Title,
                Content = newsBll.Content,
                Author = newsBll.Author
            };
        }

        public static NewsBll ToNewsBll(NewsEntity newsEntity)
        {
            Tuple<string, DateTime> pairCityDate = NewsEntity.ExtractFromPartitionKey(newsEntity.PartitionKey);
            string id = NewsEntity.ExtractFromRowKey(newsEntity.RowKey);

            return new NewsBll
            {
                Id = Guid.Parse(id),
                City = pairCityDate.Item1,
                Title = newsEntity.Title,
                Content = newsEntity.Content,
                Author = newsEntity.Author,
                Date = pairCityDate.Item2
            };
        }

        public override bool Equals(object anotherNewsObject)
        {
            var anotherNews = anotherNewsObject as NewsEntity;

            return
                anotherNews != null 
                && string.Equals(this.PartitionKey, anotherNews.PartitionKey)
                && this.RowKey.Equals(anotherNews.RowKey);
        }

        public override int GetHashCode()
        {
            return
                this.PartitionKey.GetHashCode() 
                ^ this.RowKey.GetHashCode();
        }

        public void Merge(NewsEntity newsMemory)
        {
            if (this.Equals(newsMemory))
            {
                if (newsMemory.Title != null)
                {
                    this.Title = newsMemory.Title;
                }

                if (newsMemory.Content != null)
                {
                    this.Content = newsMemory.Content;
                }
            }
        }

        public static string BuildPartitionkey(string city, DateTime date)
        {
            return string.Format("{0};{1}", city, date.ToString("yyyy-MM-dd"));
        }

        public static Tuple<string, DateTime> ExtractFromPartitionKey(string partitionKey)
        {
            string[] values = partitionKey.Split(';');

            return new Tuple<string, DateTime>(values[0], DateTime.Parse(values[1]));
        }

        public static string BuildRowKey(Guid id)
        {
            return string.Format("NEWS;{0}", id);
        }

        public static string ExtractFromRowKey(string rowKey)
        {
            string[] values = rowKey.Split(';');

            return values[1];
        }
    }
}
