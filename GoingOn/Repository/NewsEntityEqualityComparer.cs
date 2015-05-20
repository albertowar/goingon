// ****************************************************************************
// <copyright file="NewsMemoryEqualityComparer.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// Equality comparer for news in the Storage. It checks title, author and partition key (city).
// </summary>
// ****************************************************************************

namespace GoingOn.Repository
{
    using System;
    using System.Collections.Generic;
    using GoingOn.Repository.Entities;

    public class NewsEntityEqualityComparer : IEqualityComparer<NewsEntity>
    {
        public bool Equals(NewsEntity news1, NewsEntity news2)
        {
            return
                string.Equals(news1.Title, news2.Title, StringComparison.Ordinal)
                && string.Equals(news1.Author, news2.Author, StringComparison.Ordinal)
                && string.Equals(news1.PartitionKey, news2.PartitionKey, StringComparison.Ordinal);
        }

        public int GetHashCode(NewsEntity newsEntity)
        {
            return newsEntity.Title.GetHashCode() ^
                newsEntity.Author.GetHashCode() ^
                newsEntity.PartitionKey.GetHashCode();
        }
    }
}