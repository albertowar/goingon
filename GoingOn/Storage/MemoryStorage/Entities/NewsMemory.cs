// ****************************************************************************
// <copyright file="NewsMemory.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace Storage.MemoryStorage.Entities
{
    using System;
    using Model.EntitiesBll;

    public class NewsMemory
    {
        public Guid Id { get; private set; } 
        public string Title { get; private set; }
        public string Content { get; private set; }
        public string Author { get; private set; }
        public DateTime Date { get; private set; }
        public int Rating { get; private set; }

        public NewsMemory(Guid id)
        {
            this.Id = id;
        }

        public NewsMemory(Guid id, string title, string content, string author, DateTime date, int rating)
        {
            this.Id = id;
            this.Title = title;
            this.Content = content;
            this.Author = author;
            this.Date = date;
            this.Rating = rating;
        }

        public static NewsMemory FromNewsBll(NewsBll newsBll)
        {
            return new NewsMemory(newsBll.Id, newsBll.Title, newsBll.Content, newsBll.Author, newsBll.Date, newsBll.Rating);
        }

        public static NewsBll ToNewsBll(NewsMemory newsMemory)
        {
            return new NewsBll
            {
                Id = newsMemory.Id,
                Title = newsMemory.Title,
                Content = newsMemory.Content,
                Author = newsMemory.Author,
                Date = newsMemory.Date,
                Rating = newsMemory.Rating
            };
        }

        public override bool Equals(object anotherNewsObject)
        {
            NewsMemory anotherNews = anotherNewsObject as NewsMemory;

            return
                anotherNews != null &&
                this.Id.Equals(anotherNews.Id);
        }

        public override int GetHashCode()
        {
            return
                Id.GetHashCode();
        }

        public void Merge(NewsMemory newsMemory)
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
