// ****************************************************************************
// <copyright file="NewsImageLinkFactory.cs" company="Universidad de Malaga">
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
    using GoingOn.Common;

    public class NewsImageLinkFactory : LinkFactory
    {
        public NewsImageLinkFactory(HttpRequestMessage request) : base(request)
        {
        }

        public override Uri GetUri(params string[] routeValues)
        {
            return new Uri(GOUriBuilder.BuildAbsoluteNewsImageUri(
                this.request.Headers.Referrer.Scheme,
                this.request.Headers.Referrer.Host,
                this.request.Headers.Referrer.Port,
                routeValues[0],
                routeValues[1],
                routeValues[2]));
        }
    }
}
