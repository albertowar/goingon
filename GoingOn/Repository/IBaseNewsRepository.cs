// ****************************************************************************
// <copyright file="INewsRepository.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// News Storage base interface
// </summary>
// ****************************************************************************

namespace GoingOn.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GoingOn.Model.EntitiesBll;

    public interface IBaseNewsRepository
    {
        /// <summary>
        /// Add a news.
        /// </summary>
        /// <param name="newsEntity">The news to add.</param>
        /// <returns></returns>
        Task AddNews(NewsBll newsBll);

        /// <summary>
        /// Retrieve a collection of news.
        /// </summary>
        /// <param name="city">The city where the news happened.</param>
        /// <param name="date">The date when the news happened.</param>
        /// <returns></returns>
        Task<IEnumerable<NewsBll>> ListNews(string city, DateTime date);

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
