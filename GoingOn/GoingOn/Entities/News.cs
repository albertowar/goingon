// ****************************************************************************
// <copyright file="News.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace GoingOn.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;

    using Newtonsoft.Json;

    using GoingOn.Links;
    using Model.EntitiesBll;

    public class News
    {
        public string Title { get; set; }
        public string Content { get; set; }
        
        public DateTime Date { get; set; }

        [JsonIgnore]
        public string Author { get; set; }

        public IList<Link> Links { get; private set; }

        public News(string title, string content)
        {
            this.Title = title;
            this.Content = content;
            this.Links = new List<Link>();
        }

        private News(Guid id, string title, string content, DateTime date, string author, HttpRequestMessage request) : this(title, content)
        {
            this.Date = date;
            this.Author = author;
            Links.Add(new NewsLinkFactory(request).Self(id.ToString()));
            Links.Add(new UserLinkFactory(request).Author(author));
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

        public static News FromNewsBll(NewsBll newsBll, HttpRequestMessage request)
        {
            return new News(newsBll.Id, newsBll.Title, newsBll.Content, newsBll.Date, newsBll.Author, request); 
        }
    }
}