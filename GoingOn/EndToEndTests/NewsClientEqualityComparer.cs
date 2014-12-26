// ****************************************************************************
// <copyright file="UserClientEqualityComparer.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Client.Entities;

namespace EndToEndTests
{
    [ExcludeFromCodeCoverage]
    public class NewsClientEqualityComparer : IEqualityComparer<NewsClient>
    {
        public bool Equals(NewsClient x, NewsClient y)
        {
            return string.Equals(x.Title, y.Title, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(NewsClient obj)
        {
            throw new System.NotImplementedException();
        }
    }
}
