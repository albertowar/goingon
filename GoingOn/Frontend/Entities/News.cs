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
        /// <summary>
        /// The title of the news. Mandatory.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The content of the news. Mandatory.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// The city of the news. Mandatory.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// The date of the news. Mandatory.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Used to update news.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="news"></param>
        /// <param name="author"></param>
        /// <returns></returns>
        public static NewsBll ToNewsBll(Guid id, News news, string author)
        {
            return new NewsBll
            {
                Id = id,
                Title = news.Title,
                Content = news.Content,
                Author = author,
                City = news.City,
                Date = news.Date
            };
        }

        /// <summary>
        /// Used to check whether the news exists already.
        /// </summary>
        /// <param name="news"></param>
        /// <param name="author"></param>
        /// <returns></returns>
        public static NewsBll ToNewsBll(News news, string author)
        {
            return new NewsBll
            {
                Title = news.Title,
                Content = news.Content,
                Author = author,
                City = news.City,
                Date = news.Date
            };
        }
    }
}