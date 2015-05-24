// ****************************************************************************
// <copyright file="NewsLinkFactory.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// News link creator
// </summary>
// ****************************************************************************

namespace GoingOn.Frontend.Links
{
    using System;
    using System.Net.Http;

    using GoingOn.Common;

    public class NewsLinkFactory : LinkFactory
    {
        public NewsLinkFactory(HttpRequestMessage message)
            : base(message)
        {
        }

        public override Uri GetUri(params string[] routeValues)
        {
            Uri requestUri = this.request.Headers.Referrer ?? this.request.RequestUri;

            return new Uri(GOUriBuilder.BuildAbsoluteNewsUri(
                requestUri.Scheme,
                requestUri.Host,
                requestUri.Port,
                routeValues[0],
                routeValues[1],
                routeValues[2]));
        }
    }
}