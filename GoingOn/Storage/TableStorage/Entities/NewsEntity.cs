// ****************************************************************************
// <copyright file="NewsEntity.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace GoingOn.Storage.TableStorage.Entities
{
    using System;
    using Microsoft.WindowsAzure.Storage.Table;
    using Model.EntitiesBll;

    public class NewsEntity : TableEntity
    {
        /*
         * The title of the news. Mandatory.
         */
        public string Title { get; set; }

        /*
         * The content of the news. Mandatory.
         */
        public string Content { get; set; }

        /*
         * The author of the news. Mandatory.
         */
        public string Author { get; set; }

        public static NewsEntity FromNewsBll(NewsBll newsBll)
        {
            return new NewsEntity
            {
                PartitionKey = NewsEntity.BuildPartitionkey(newsBll.City, newsBll.Date),
                RowKey = newsBll.Id.ToString(),
                Title = newsBll.Title,
                Content = newsBll.Content,
                Author = newsBll.Author
            };
        }

        public static NewsBll ToNewsBll(NewsEntity newsEntity)
        {
            var pairCityDate = NewsEntity.ExtractFromPartitionKey(newsEntity.PartitionKey);

            return new NewsBll
            {
                Id = Guid.Parse(newsEntity.RowKey),
                City = pairCityDate.Item1,
                Title = newsEntity.Title,
                Content = newsEntity.Content,
                Author = newsEntity.Author,
                Date = pairCityDate.Item2
            };
        }

        public override bool Equals(object anotherNewsObject)
        {
            NewsEntity anotherNews = anotherNewsObject as NewsEntity;

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
            var values = partitionKey.Split(';');

            return new Tuple<string, DateTime>(values[0], DateTime.Parse(values[1]));
        }
    }
}
