// ****************************************************************************
// <copyright file="INewsImageRepository.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// Image Storage interface
// </summary>
// ****************************************************************************

namespace GoingOn.Repository
{
    using System;
    using System.Drawing;
    using System.Threading.Tasks;

    public interface INewsImageRepository
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
        /// Retrieve the news thumbnail image.
        /// </summary>
        /// <param name="city">The city where the news happened.</param>
        /// <param name="date">The date when the news happened.</param>
        /// <param name="id">The id of the news.</param>
        /// <returns></returns>
        Task<Image> GetNewsThumbnailImage(string city, DateTime date, Guid id);

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

        /// <summary>
        /// Returns true if the image exists.
        /// </summary>
        /// <param name="city">The city where the news happened.</param>
        /// <param name="date">The date when the news happened.</param>
        /// <param name="id">The id of the news.</param>
        Task<bool> ContainsImage(string city, DateTime date, Guid id);

        /// <summary>
        /// Returns true if the image exists.
        /// </summary>
        /// <param name="city">The city where the news happened.</param>
        /// <param name="date">The date when the news happened.</param>
        /// <param name="id">The id of the news.</param>
        Task<bool> ContainsImageThumbnail(string city, DateTime date, Guid id);
    }
}
