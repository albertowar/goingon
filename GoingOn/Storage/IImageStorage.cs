// ****************************************************************************
// <copyright file="IImageStorage.cs" company="Universidad de Malaga">
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
    using System.Drawing;
    using System.Threading.Tasks;

    public interface IImageStorage
    {
        /// <summary>
        /// Retrieve the news image.
        /// </summary>
        /// <param name="city">The city where the news happened.</param>
        /// <param name="date">The date when the news happened.</param>
        /// <param name="id">The id of the news.</param>
        /// <returns></returns>
        Task<Image> GetNewsImage(string city, DateTime date, Guid id);

        /// <summary>
        /// Create a news simage.
        /// </summary>
        /// <param name="city">The city where the news happened.</param>
        /// <param name="date">The date when the news happened.</param>
        /// <param name="id">The id of the news.</param>
        Task CreateNewsImage(string city, DateTime date, Guid id, Image image);

        /// <summary>
        /// Delete the news image.
        /// </summary>
        /// <param name="city">The city where the news happened.</param>
        /// <param name="date">The date when the news happened.</param>
        /// <param name="id">The id of the news.</param>
        Task DeleteNewsImage(string city, DateTime date, Guid id);
    }
}
