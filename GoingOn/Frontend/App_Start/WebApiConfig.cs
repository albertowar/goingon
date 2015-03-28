// ****************************************************************************
// <copyright file="WebApiConfig.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace GoingOn.Frontend
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Web.Http;
    using System.Web.Http.Dependencies;
    using System.Web.Http.Filters;
    using GoingOn.Frontend.DependencyInjection;
    using Microsoft.Practices.Unity;
    
    using GoingOn.Frontend.Validation;
    using GoingOn.Storage;
    using GoingOn.Storage.TableStorage;

    [ExcludeFromCodeCoverage]
    [GeneratedCode("ASP.NET", "Visual Studio 2013")]
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration configuration)
        {
            string connectionString = ConfigurationManager.AppSettings["StorageConnectionString"];
            string userTableName = ConfigurationManager.AppSettings["UserTableName"];
            string newsTableName = ConfigurationManager.AppSettings["NewsTableName"];

            var userTableStorage = new UserTableStorage(connectionString, userTableName);
            var newsTableStorage = new NewsTableStorage(connectionString, newsTableName);

            // Dependy injection configuration
            var container = new UnityContainer();
            container.RegisterInstance<IUserStorage>(userTableStorage);
            container.RegisterInstance<INewsStorage>(newsTableStorage);
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
