// ****************************************************************************
// <copyright file="NewsREST.cs" company="Universidad de Malaga">
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
    using System.Collections.Generic;
    using System.Net.Http;

    using Frontend.Links;
    using Model.EntitiesBll;

    public class NewsREST
    {
        public News News { get; private set; }

        public string Author { get; private set; }

        public IList<Link> Links { get; private set; }

        public NewsREST(News news, string author)
        {
            this.News = news;
            this.Author = author;
            this.Links = new List<Link>();
        }

        private NewsREST(News news, string author, Guid id, HttpRequestMessage request)
            : this(news, author)
        {
            this.News = news;
            this.Author = author;
            Links.Add(new NewsLinkFactory(request).Self(id.ToString()));
            Links.Add(new UserLinkFactory(request).Author(author));
        }

        public static NewsREST FromNewsBll(NewsBll newsBll, HttpRequestMessage request)
        {
            return new NewsREST(new News(newsBll.Title, newsBll.Content, newsBll.Date), newsBll.Author, newsBll.Id, request);
        }
    }
}