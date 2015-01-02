// ****************************************************************************
// <copyright file="UserCompleteEqualityComparer.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using GoingOn.Entities;

namespace Common.Tests
{
    [ExcludeFromCodeCoverage]
    public class NewsCompleteEqualityComparer : IEqualityComparer<News>
    {
        public bool Equals(News news1, News news2)
        {
            return
                string.Equals(news1.Title, news2.Title) &&
                string.Equals(news1.Content, news2.Content);
        }

        public int GetHashCode(News obj)
        {
            throw new System.NotImplementedException();
        }
    }
}
