// ****************************************************************************
// <copyright file="NewsImageBlobStorage.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace GoingOn.Storage.BlobStorage
{
    using System;
    using System.Drawing;
    using System.Threading.Tasks;

    public class NewsImageBlobStorage : IImageStorage
    {
        public Task<Image> GetNewsImage(string city, DateTime date, Guid id)
        {
            throw new NotImplementedException();
        }

        public Task CreateNewsImage(string city, DateTime date, Guid id, Image image)
        {
            throw new NotImplementedException();
        }

        public Task DeleteNewsImage(string city, DateTime date, Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
