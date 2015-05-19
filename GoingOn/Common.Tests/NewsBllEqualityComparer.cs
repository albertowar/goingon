// ****************************************************************************
// <copyright file="NewsBllEqualityComparer.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// Equality comparer for NewsBll. Two news are the same if they have the same author and title.
// </summary>
// ****************************************************************************

namespace GoingOn.Common.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Model.EntitiesBll;

    [ExcludeFromCodeCoverage]
    public class NewsBllEqualityComparer : IEqualityComparer<NewsBll>
    {
        public bool Equals(NewsBll news1, NewsBll news2)
        {
            return
                string.Equals(news1.Title, news2.Title) &&
                string.Equals(news1.Author, news2.Author, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(NewsBll news)
        {
            return news.Title.GetHashCode() ^
                news.Content.GetHashCode();
        }
    }
}
