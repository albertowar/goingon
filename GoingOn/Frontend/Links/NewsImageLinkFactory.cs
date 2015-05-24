// ****************************************************************************
// <copyright file="NewsImageLinkFactory.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// Image link creator
// </summary>
// ****************************************************************************

namespace GoingOn.Frontend.Links
{
    using System;
    using System.Net.Http;
    using GoingOn.Common;

    public class NewsImageLinkFactory : LinkFactory
    {
        public NewsImageLinkFactory(HttpRequestMessage request) : base(request)
        {
        }

        public override Uri GetUri(params string[] routeValues)
        {
            Uri requestUri = this.request.Headers.Referrer ?? this.request.RequestUri;

            return new Uri(GOUriBuilder.BuildAbsoluteNewsImageUri(
                requestUri.Scheme,
                requestUri.Host,
                requestUri.Port,
                routeValues[0],
                routeValues[1],
                routeValues[2]));
        }
    }
}
