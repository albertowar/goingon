// ****************************************************************************
// <copyright file="INewsStorage.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// News Storage interface
// </summary>
// ****************************************************************************

namespace GoingOn.Storage
{
    using System;
    using System.Threading.Tasks;
    using GoingOn.Model.EntitiesBll;
    using GoingOn.Storage.TableStorage.Entities;

    public interface INewsStorage : INewsStorageBase
    {
        /// <summary>
        /// Retrieve a news.
        /// </summary>
        /// <param name="city">The city where the news happened.</param>
        /// <param name="date">The date when the news happened.</param>
        /// <param name="id">The id of the news.</param>
        /// <returns></returns>
        Task<NewsBll> GetNews(string city, DateTime date, Guid id);

        /// <summary>
        /// Checks whether there is a news with the same PartitionKey and RowKey.
        /// </summary>
        /// <param name="city">The city where the news happened.</param>
        /// <param name="date">The date when the news happened.</param>
        /// <param name="id">The id of the news.</param>
        /// <returns></returns>
        Task<bool> ContainsNews(string city, DateTime date, Guid id);

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
        /// Checks whether there is a news with the same PartitionKey (City,Date) and
        /// similar information (title, content).
        /// Used in create operation to determine whether there was a news
        /// with the same content created before.
        /// </summary>
        /// <param name="newsEntity">The news to check</param>
        /// <returns></returns>
        Task<bool> ContainsNewsCheckContent(NewsEntity newsEntity);
    }
}