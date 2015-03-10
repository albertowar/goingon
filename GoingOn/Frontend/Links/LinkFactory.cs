// ****************************************************************************
// <copyright file="LinkFactory.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace Frontend.Links
{
    using System;
    using System.Net.Http;
    using System.Web.Http.Routing;

    public class LinkFactory
    {
        private readonly UrlHelper urlHelper;
        private readonly string controllerName;
        private const string DefaultApi = "DefaultApi";

        protected LinkFactory(HttpRequestMessage request, Type controllerType) 
        {
            this.urlHelper = new UrlHelper(request);
            this.controllerName = GetControllerName(controllerType);
        }

        protected Link GetLink<TController>(string rel, object id, string action, string route = DefaultApi)
        {
            var uri = GetUri(new
            {
                controller = GetControllerName(
                    typeof(TController)),
                id,
                action
            }, route);

            return new Link { Action = action, Href = uri, Rel = rel };
        }

        private string GetControllerName(Type controllerType) 
        {
            var name = controllerType.Name;
            return name.Substring(0, name.Length - "controller".Length).ToLower();
        }

        protected Uri GetUri(object routeValues, string route = DefaultApi)
        {
            return new Uri(this.urlHelper.Link(route, routeValues));
        }

        public Link Self(string id, string route = DefaultApi)
        {
            return new Link
            {
                Rel = Rels.Self,
                Href = this.GetUri(
                    new { controller = this.controllerName, id = id }, route)
            };
        }

        public Link Author(string id, string route = DefaultApi)
        {
            return new Link
            {
                Rel = Rels.Author,
                Href = this.GetUri(
                    new { controller = this.controllerName, id = id }, route)
            };
        }

        public class Rels
        {
            public const string Self = "self";
            public const string Author = "author";
        }
    }

    public abstract class LinkFactory<TController> : LinkFactory
    {
        public LinkFactory(HttpRequestMessage request) :
            base(request, typeof(TController)) { }
    }
}