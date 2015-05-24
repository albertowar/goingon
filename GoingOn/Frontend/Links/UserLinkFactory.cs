// ****************************************************************************
// <copyright file="UserLinkFactory.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// User link creator
// </summary>
// ****************************************************************************

namespace GoingOn.Frontend.Links
{
    using System;
    using System.Net.Http;
    using GoingOn.Common;

    public class UserLinkFactory : LinkFactory
    {
        public UserLinkFactory(HttpRequestMessage message) : base(message)
        {
        }

        public override Uri GetUri(params string[] routeValues)
        {
            Uri requestUri = this.request.Headers.Referrer ?? this.request.RequestUri;

            return new Uri(GOUriBuilder.BuildAbsoluteUserUri(
                requestUri.Scheme,
                requestUri.Host,
                requestUri.Port,
                routeValues[0]));
        }
    }
}