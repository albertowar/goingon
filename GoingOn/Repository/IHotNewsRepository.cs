// ****************************************************************************
// <copyright file="INewsStorage.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// NewsStorage interface
// </summary>
// ****************************************************************************

namespace GoingOn.Repository
{
    using System;
    using System.Threading.Tasks;

    public interface IHotNewsRepository : IBaseNewsRepository
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="city"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        Task<bool> ContainsAnyHotNews(string city, DateTime date);
    }
}