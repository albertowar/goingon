// ****************************************************************************
// <copyright file="NewsLinkFactory.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace GoingOn.Links
{
    using GoingOn.Controllers;
    using System.Net.Http;

    public class NewsLinkFactory : LinkFactory<NewsController>
    {
        public NewsLinkFactory(HttpRequestMessage message)
            : base(message)
        {
        }
    }
}