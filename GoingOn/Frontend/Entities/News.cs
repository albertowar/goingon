// ****************************************************************************
// <copyright file="News.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// Encapsulates the news information as the user would create it
// </summary>
// ****************************************************************************

namespace GoingOn.Frontend.Entities
{
    using System;
    using GoingOn.Model.EntitiesBll;

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
        /// Used to update news.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="news"></param>
        /// <param name="city"></param>
        /// <param name="author"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public static NewsBll ToNewsBll(Guid id, News news, string city, string author, DateTime date)
        {
            return new NewsBll
            {
                Id = id,
                Title = news.Title,
                Content = news.Content,
                City = city,
                Author = author,
                Date = date
            };
        }

        /// <summary>
        /// Used to check whether the news exists already.
        /// </summary>
        /// <param name="news"></param>
        /// <param name="city"></param>
        /// <param name="author"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public static NewsBll ToNewsBll(News news, string city, string author, DateTime date)
        {
            return new NewsBll
            {
                Title = news.Title,
                Content = news.Content,
                City = city,
                Author = author,
                Date = date
            };
        }
    }
}