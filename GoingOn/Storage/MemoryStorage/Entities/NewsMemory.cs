// ****************************************************************************
// <copyright file="NewsMemory.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace MemoryStorage.Entities
{
    using System;
    using Model.EntitiesBll;

    public class NewsMemory
    {
        public string Title { get; private set; }
        public string Content { get; private set; }
        public UserMemory Author { get; private set; }
        public TimeSpan Date { get; private set; }
        public int Rating { get; private set; }

        public NewsMemory(string title, string content, UserMemory author, TimeSpan date, int rating)
        {
            Title = title;
            Content = content;
            Author = author;
            Date = date;
            Rating = rating;
        }

        public static NewsMemory FromNewsBll(NewsBll newsBll)
        {
            return new NewsMemory(newsBll.Title, newsBll.Content, UserMemory.FromUserBll(newsBll.Author), newsBll.Date, newsBll.Rating);
        }

        public static NewsBll ToNewsBll(NewsMemory newsMemory)
        {
            return new NewsBll(newsMemory.Title, newsMemory.Content, UserMemory.ToUserBll(newsMemory.Author), newsMemory.Date, newsMemory.Rating);
        }

        public override bool Equals(object anotherNewsObject)
        {
            NewsMemory anotherNews = anotherNewsObject as NewsMemory;

            return
                anotherNews != null &&
                string.Equals(Title, anotherNews.Title) &&
                Author.Equals(anotherNews.Author);
        }

        public override int GetHashCode()
        {
            return 
                Title.GetHashCode() ^
                Content.GetHashCode() ^
                Author.GetHashCode() ^
                Date.GetHashCode() ^
                Rating.GetHashCode();
        }
    }
}
