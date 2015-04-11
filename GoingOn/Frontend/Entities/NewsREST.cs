﻿// ****************************************************************************
// <copyright file="NewsREST.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace GoingOn.Frontend.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;

    using GoingOn.Frontend.Links;
    using GoingOn.Model.EntitiesBll;

    public class NewsREST
    {
        public string Title { get; set; }

        public string Content { get; set; }

        public string City { get; set; }

        public DateTime Date { get; set; }

        public string Author { get; private set; }

        public IList<Link> Links { get; private set; }

        public NewsREST(News news, string author, DateTime date)
        {
            this.Title = news.Title;
            this.Content = news.Content;
            this.Author = author;
            this.Date = date;
            this.Links = new List<Link>();
        }

        private NewsREST(News news, string city, DateTime date, string author, Guid id, HttpRequestMessage request)
            : this(news, author, date)
        {
            this.Title = news.Title;
            this.Content = news.Content;
            this.City = city;
            this.Date = date;
            this.Author = author;
            this.Links.Add(new NewsLinkFactory(request).Self(city, date.ToString("yy-MM-dd"), id.ToString()));
            this.Links.Add(new UserLinkFactory(request).Author(author));
        }

        public static NewsREST FromNewsBll(NewsBll newsBll, HttpRequestMessage request)
        {
            return new NewsREST(new News { Title = newsBll.Title, Content = newsBll.Content }, newsBll.City, newsBll.Date, newsBll.Author, newsBll.Id, request);
        }
    }
}