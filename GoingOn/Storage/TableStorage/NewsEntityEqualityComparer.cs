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
            var date1 = news1.Date;
            var date2 = news2.Date;

            return
                string.Equals(news1.Title, news2.Title, StringComparison.Ordinal) &&
                string.Equals(news1.Author, news2.Author, StringComparison.Ordinal) &&
                (!date1.HasValue || 
                !date2.HasValue ||
                date1.Value.Year.Equals(date2.Value.Year) &&
                date1.Value.Month.Equals(date2.Value.Month) &&
                date1.Value.Day.Equals(date2.Value.Day) &&
                date1.Value.Hour.Equals(date2.Value.Hour));
        }

        public int GetHashCode(NewsEntity obj)
        {
            throw new System.NotImplementedException();
        }
    }
}