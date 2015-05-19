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
            return new Uri(GOUriBuilder.BuildAbsoluteUserUri(
                this.request.Headers.Referrer.Scheme,
                this.request.Headers.Referrer.Host,
                this.request.Headers.Referrer.Port,
                routeValues[0]));
        }
    }
}