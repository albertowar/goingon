// ****************************************************************************
// <copyright file="WebApiConfig.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// WebApiConfig class
// </summary>
// ****************************************************************************

namespace GoingOn.FrontendWebRole
{
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Web.Http;
    using System.Web.Http.Filters;
    
    using Microsoft.Practices.Unity;
    
    using GoingOn.Frontend.Validation;
    using GoingOn.FrontendWebRole.DependencyInjection;
    using GoingOn.Repository;

    [ExcludeFromCodeCoverage]
    [GeneratedCode("ASP.NET", "Visual Studio 2013")]
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration configuration)
        {
            string connectionString = ConfigurationManager.AppSettings["StorageConnectionString"];
            string userTableName = ConfigurationManager.AppSettings["UserTableName"];
            string newsTableName = ConfigurationManager.AppSettings["NewsTableName"];
            string hotNewsTableName = ConfigurationManager.AppSettings["HotNewsTableName"];

            var userTableStorage = new UserTableRepository(connectionString, userTableName);
            var newsTableStorage = new NewsTableRepository(connectionString, newsTableName);
            var hotNewsTableStorage = new NewsTableRepository(connectionString, hotNewsTableName);

            // Dependy injection configuration
            var container = new UnityContainer();
            container.RegisterInstance<IUserRepository>(userTableStorage);
            container.RegisterInstance<INewsRepository>(newsTableStorage);
            container.RegisterInstance<IHotNewsRepository>(hotNewsTableStorage);
            container.RegisterInstance<IApiInputValidationChecks>(new ApiInputValidationChecks(new ApiInputValidationChecks()));
            container.RegisterInstance<IApiBusinessLogicValidationChecks>(new ApiBusinessLogicValidationChecks());
            configuration.DependencyResolver = new UnityResolver(container);

            // Register the filter injector
            List<IFilterProvider> providers = configuration.Services.GetFilterProviders().ToList();

            IFilterProvider defaultprovider = providers.Single(i => i is ActionDescriptorFilterProvider);
            
            configuration.Services.Remove(typeof(IFilterProvider), defaultprovider);
            configuration.Services.Add(typeof(IFilterProvider), new UnityFilterProvider(container));

            // Web API routes
            configuration.MapHttpAttributeRoutes();
        }
    }
}
