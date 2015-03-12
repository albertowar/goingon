﻿// ****************************************************************************
// <copyright file="NewsClient.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace Client.Entities
{
    using Frontend.Entities;

    public class NewsClient
    {
        /*
         * The title of the news. Mandatory.
         */
        public string Title { get; set; }

        /*
         * The content of the news. Mandatory.
         */
        public string Content { get; set; }

        public static NewsClient FromNewsREST(NewsREST newsREST)
        {
            return new NewsClient { Title = newsREST.News.Title, Content = newsREST.News.Content };
        }
    }
}
