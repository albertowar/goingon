// ****************************************************************************
// <copyright file="LinkFactory.cs" company="Universidad de Malaga">
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

    public abstract class LinkFactory
    {
        protected readonly HttpRequestMessage request;

        protected LinkFactory(HttpRequestMessage request) 
        {
            this.request = request;
        }

        public abstract Uri GetUri(params string[] routeValues);

        public Link Self(params string[] routeValues)
        {
            return new Link
            {
                Rel = Rels.self.ToString(),
                Href = this.GetUri(routeValues)
            };
        }

        public Link Author(params string[] routeValues)
        {
            return new Link
            {
                Rel = Rels.author.ToString(),
                Href = this.GetUri(routeValues)
            };
        }
    }
}