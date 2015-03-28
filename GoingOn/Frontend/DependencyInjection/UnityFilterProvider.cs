// ****************************************************************************
// <copyright file="WebApiConfig.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace GoingOn.Frontend.DependencyInjection
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;

    using Microsoft.Practices.Unity;

    public class UnityFilterProvider : IFilterProvider
    {
        private readonly IUnityContainer container;

        private readonly ActionDescriptorFilterProvider defaultProvider = new ActionDescriptorFilterProvider();

        public UnityFilterProvider(IUnityContainer container)
        {
            this.container = container;
        }

        public IEnumerable<FilterInfo> GetFilters(HttpConfiguration configuration, HttpActionDescriptor actionDescriptor)
        {
            IEnumerable<FilterInfo> attributes = this.defaultProvider.GetFilters(configuration, actionDescriptor);

            IEnumerable<FilterInfo> filterInfos = attributes as FilterInfo[] ?? attributes.ToArray();

            foreach (FilterInfo attr in filterInfos)
            {
                this.container.BuildUp(attr.Instance.GetType(), attr.Instance);
            }

            return filterInfos;
        }
    }
}
