// ****************************************************************************
// <copyright file="NewsClient.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

using Frontend.Entities;

namespace Client.Entities
{
    public class NewsClient
    {
        public string Title { get; set; }

        public string Content { get; set; }

        public static NewsClient FromFrontendNews(News news)
        {
            return new NewsClient { Title = news.Title, Content = news.Content };
        }

        public static News ToFrontendNews(NewsClient newsClient)
        {
            return new News(newsClient.Title, newsClient.Content);
        }
    }
}
