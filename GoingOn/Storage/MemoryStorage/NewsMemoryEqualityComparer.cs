// ****************************************************************************
// <copyright file="NewsMemoryEqualityComparer.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace Storage.MemoryStorage
{
    using System;
    using System.Collections.Generic;

    using Storage.MemoryStorage.Entities;

    public class NewsMemoryEqualityComparer : IEqualityComparer<NewsMemory>
    {
        public bool Equals(NewsMemory news1, NewsMemory news2)
        {
            var date1 = news1.Date;
            var date2 = news2.Date;

            return
                string.Equals(news1.Title, news2.Title, StringComparison.Ordinal) &&
                string.Equals(news1.Author, news2.Author, StringComparison.Ordinal) &&
                date1.Year.Equals(date2.Year) &&
                date1.Month.Equals(date2.Month) &&
                date1.Day.Equals(date2.Day) &&
                date1.Hour.Equals(date2.Hour);
        }

        public int GetHashCode(NewsMemory obj)
        {
            throw new System.NotImplementedException();
        }
    }
}