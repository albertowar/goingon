// ****************************************************************************
// <copyright file="NewsConverter.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace NewsFinderWorkerRole
{
    using Entities;
    using GoingOn.Client.Entities;

    public class NewsConverter
    {
        public static NewsClient ToNewsClient(Article article)
        {
            return new NewsClient
            {
                Title = article.Title,
                Content = article.Summary
            };
        }
    }
}
