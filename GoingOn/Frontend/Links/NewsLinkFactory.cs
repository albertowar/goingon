// ****************************************************************************
// <copyright file="NewsLinkFactory.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace GoingOn.Frontend.Links
{
    using System;
    using System.Net.Http;

    public class NewsLinkFactory : LinkFactory
    {
        public NewsLinkFactory(HttpRequestMessage message)
            : base(message)
        {
        }

        public override Uri GetUri(params string[] routeValues)
        {
            return new Uri(base.urlHelper.Route("GetNews", new { city = routeValues[0], date = routeValues[1], newsId = routeValues[2] })); 
        }
    }
}