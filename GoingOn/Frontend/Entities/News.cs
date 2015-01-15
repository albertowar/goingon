// ****************************************************************************
// <copyright file="News.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace Frontend.Entities
{
    using System;

    using Model.EntitiesBll;

    public class News
    {
        public string Title { get; set; }

        public string Content { get; set; }

        public News(string title, string content)
        {
            this.Title = title;
            this.Content = content;
        }

        public static NewsBll ToNewsBll(News news, Guid id, string author, DateTime date)
        {
            return new NewsBll
            {
                Id = id,
                Title = news.Title,
                Content = news.Content,
                Author = author,
                Date = date,
                Rating = 0
            };;
        }

        public static NewsBll ToNewsBll(Guid id, News news, string author)
        {
            return new NewsBll
            {
                Id = id,
                Title = news.Title,
                Content = news.Content,
                Author = author,
                Date = DateTime.UtcNow,
                Rating = 0
            };;
        }

        public static NewsBll ToNewsBll(News news, string author)
        {
            return new NewsBll
            {
                Title = news.Title,
                Content = news.Content,
                Author = author,
                Date = DateTime.UtcNow,
                Rating = 0
            };;
        }
    }
}