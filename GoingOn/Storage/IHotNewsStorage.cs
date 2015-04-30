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
    using Model.EntitiesBll;

    public interface IHotNewsStorage
    {
        /// <summary>
        /// Retrieve a news.
        /// </summary>
        /// <param name="city">The city where the news happened.</param>
        /// <param name="date">The date when the news happened.</param>
        /// <returns></returns>
        Task<IEnumerable<NewsBll>> GetNews(string city, DateTime date);
    }
}