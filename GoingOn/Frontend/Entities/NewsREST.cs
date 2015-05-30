// ****************************************************************************
// <copyright file="NewsREST.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// Encapsulates the news as the server will return it
// </summary>
// ****************************************************************************

namespace GoingOn.Frontend.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;

    using GoingOn.Frontend.Links;
    using GoingOn.Model.EntitiesBll;
    using Newtonsoft.Json;

    public class NewsREST
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
        public string Author { get; private set; }

        /// <summary>
        /// Related information to the news.
        /// </summary>
        public IList<Link> Links { get; private set; }

        [JsonConstructor]
        public NewsREST(string title, string content, string author, IList<Link> links)
        {
            this.Title = title;
            this.Content = content;
            this.Author = author;
            this.Links = links;
        }

        public NewsREST(News news, string author)
        {
            this.Title = news.Title;
            this.Content = news.Content;
            this.Author = author;
            this.Links = new List<Link>();
        }

        private NewsREST(News news, string city, DateTime date, Guid id, string author, HttpRequestMessage request)
            : this(news, author)
        {
            this.Title = news.Title;
            this.Content = news.Content;
            this.Author = author;
            this.Links.Add(new NewsLinkFactory(request).Self(city, date.ToString("yyyy-MM-dd"), id.ToString()));
            this.Links.Add(new UserLinkFactory(request).Author(author));
        }

        public static NewsREST FromNewsBll(NewsBll newsBll, HttpRequestMessage request)
        {
            return new NewsREST(new News { Title = newsBll.Title, Content = newsBll.Content }, newsBll.City, newsBll.Date, newsBll.Id, newsBll.Author, request);
        }

        public static News ToNews(NewsREST newsRest)
        {
            return new News
            {
                Title = newsRest.Title, 
                Content = newsRest.Content
            };
        }
    }
}