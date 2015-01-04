// ****************************************************************************
// <copyright file="NewsBllEqualityComparer.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace Common.Tests
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

        public int GetHashCode(NewsBll obj)
        {
            throw new System.NotImplementedException();
        }
    }
}
