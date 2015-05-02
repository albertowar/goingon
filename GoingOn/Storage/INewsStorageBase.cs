// ****************************************************************************
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
    using GoingOn.Model.EntitiesBll;
    using GoingOn.Storage.TableStorage.Entities;

    public interface INewsStorageBase
    {
        /// <summary>
        /// Add a news.
        /// </summary>
        /// <param name="newsEntity">The news to add.</param>
        /// <returns></returns>
        Task AddNews(NewsEntity newsEntity);

        /// <summary>
        /// Retrieve a news.
        /// </summary>
        /// <param name="city">The city where the news happened.</param>
        /// <param name="date">The date when the news happened.</param>
        /// <returns></returns>
        Task<IEnumerable<NewsBll>> GetNews(string city, DateTime date);

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
