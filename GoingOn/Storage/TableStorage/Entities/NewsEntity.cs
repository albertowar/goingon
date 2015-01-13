// ****************************************************************************
// <copyright file="NewsEntity.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace Storage.TableStorage.Entities
{
    using System;

    using Microsoft.WindowsAzure.Storage.Table;

    using Model.EntitiesBll;

    public class NewsEntity : TableEntity
    {
        // Constants
        public const string City = "World";

        public string Title { get; set; }
        public string Content { get; set; }
        public string Author { get; set; }
        public DateTime Date { get; set; }
        public int Rating { get; set; }

        public NewsEntity() { }

        public NewsEntity(Guid id)
        {
            this.PartitionKey = City;
            this.RowKey = id.ToString();
        }

        public NewsEntity(Guid id, string title, string content, string author, DateTime date, int rating)
        {
            this.PartitionKey = City;
            this.RowKey = id.ToString();
            this.Title = title;
            this.Content = content;
            this.Author = author;
            this.Date = date;
            this.Rating = rating;
        }

        public static NewsEntity FromNewsBll(NewsBll newsBll)
        {
            return new NewsEntity(newsBll.Id, newsBll.Title, newsBll.Content, newsBll.Author, newsBll.Date, newsBll.Rating);
        }

        public static NewsBll ToNewsBll(NewsEntity newsEntity)
        {
            return new NewsBll
            {
                Id = Guid.Parse(newsEntity.RowKey),
                Title = newsEntity.Title,
                Content = newsEntity.Content,
                Author = newsEntity.Author,
                Date = newsEntity.Date,
                Rating = newsEntity.Rating
            };
        }

        public override bool Equals(object anotherNewsObject)
        {
            NewsEntity anotherNews = anotherNewsObject as NewsEntity;

            return
                anotherNews != null &&
                this.RowKey.Equals(anotherNews.RowKey);
        }

        public override int GetHashCode()
        {
            return
                this.RowKey.GetHashCode();
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

                this.Date = newsMemory.Date;
            }
        }
    }
}
