// ****************************************************************************
// <copyright file="NewsMemoryEqualityComparer.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace Storage.TableStorage
{
    using System;
    using System.Collections.Generic;

    using Storage.TableStorage.Entities;

    public class NewsEntityEqualityComparer : IEqualityComparer<NewsEntity>
    {
        public bool Equals(NewsEntity news1, NewsEntity news2)
        {
            return
                string.Equals(news1.Title, news2.Title, StringComparison.Ordinal)
                && string.Equals(news1.Author, news2.Author, StringComparison.Ordinal)
                && string.Equals(news1.PartitionKey, news2.PartitionKey, StringComparison.Ordinal);
        }

        public int GetHashCode(NewsEntity obj)
        {
            throw new System.NotImplementedException();
        }
    }
}