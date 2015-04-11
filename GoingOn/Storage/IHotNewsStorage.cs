﻿// ****************************************************************************
// <copyright file="INewsStorage.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace GoingOn.Storage
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Model.EntitiesBll;

    public interface IHotNewsStorage
    {
        /// <summary>
        /// Add a news.
        /// </summary>
        /// <param name="newsBll">The news to add.</param>
        /// <returns></returns>
        Task AddNews(NewsBll newsBll);

        /// <summary>
        /// Retrieve a news.
        /// </summary>
        /// <param name="city">The city where the news happened.</param>
        /// <param name="date">The date when the news happened.</param>
        /// <param name="id">The id of the news.</param>
        /// <returns></returns>
        Task<NewsBll> GetNews(string city, DateTime date, Guid id);

        /// <summary>
        /// Retrieve a news.
        /// </summary>
        /// <param name="city">The city where the news happened.</param>
        /// <param name="date">The date when the news happened.</param>
        /// <returns></returns>
        Task<IEnumerable<NewsBll>> GetNews(string city, DateTime date);

        /// <summary>
        /// Checks whether there is a news matching these values.
        /// </summary>
        /// <param name="city">The city where the news happened.</param>
        /// <param name="date">The date when the news happened.</param>
        /// <param name="id">The id of the news.</param>
        /// <returns></returns>
        Task<bool> Exists(string city, DateTime date, Guid id);

        /// <summary>
        /// Checks whether there is a news matching these values.
        /// Used in update and delete operations to determine whether the user
        /// has permissions over the news.
        /// </summary>
        /// <param name="city">The city where the news happened.</param>
        /// <param name="date">The date when the news happened.</param>
        /// <param name="id">The id of the news.</param>
        /// <param name="author">The author of the news.</param>
        /// <returns></returns>
        Task<bool> IsAuthorOf(string city, DateTime date, Guid id, string author);

        /// <summary>
        /// Checks whether there is a news matching these values.
        /// Used in create operation to determine whether there was a news
        /// with the same content created before.
        /// </summary>
        /// <param name="news">The news to check.</param>
        /// <returns></returns>
        Task<bool> ContainsNews(NewsBll news);

        /// <summary>
        /// Update a news.
        /// </summary>
        /// <param name="newsBll">Update the news.</param>
        /// <returns></returns>
        Task UpdateNews(NewsBll newsBll);

        /// <summary>
        /// Delete a news.
        /// </summary>
        /// <param name="city">The city where the news happened.</param>
        /// <param name="date">The date when the news happened.</param>
        /// <param name="id">The id of the news.</param>
        /// <returns></returns>
        Task DeleteNews(string city, DateTime date, Guid id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="city"></param>
        /// <returns></returns>
        Task DeleteAllNews(string city);
    }
}