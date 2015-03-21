// ****************************************************************************
// <copyright file="UserLinkFactory.cs" company="Universidad de Malaga">
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

    public class UserLinkFactory : LinkFactory
    {
        public UserLinkFactory(HttpRequestMessage message) : base(message)
        {
        }

        public override Uri GetUri(params string[] routeValues)
        {
            return new Uri(GOUriBuilder.BuildAbsoluteUserUri(
                this.request.RequestUri.Scheme,
                this.request.RequestUri.Host, 
                routeValues[0]));
        }
    }
}