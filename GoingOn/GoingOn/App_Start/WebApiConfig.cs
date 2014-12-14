// ****************************************************************************
// <copyright file="WebApiConfig.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace GoingOn
{
    using Microsoft.Practices.Unity;
    using System;
    using System.Collections.Generic;
    using System.Web.Http;
    using System.Web.Http.Dependencies;

    using GoingOn.Validation;
    using MemoryStorage;

    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration configuration)
        {
            // Dependy injection configuration
            var container = new UnityContainer();
            container.RegisterType<IUserStorage, UserMemoryStorage>(new HierarchicalLifetimeManager());
            container.RegisterType<IApiInputValidationChecks, ApiInputValidationChecks>(new HierarchicalLifetimeManager());
            container.RegisterType<IApiBusinessLogicValidationChecks, ApiBusinessLogicValidationChecks>(new HierarchicalLifetimeManager());
            configuration.DependencyResolver = new UnityResolver(container);

            // Web API routes
            configuration.MapHttpAttributeRoutes();

            configuration.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }

    #region Dependency injection helpers
    public class UnityResolver : IDependencyResolver
    {
        protected IUnityContainer container;

        public UnityResolver(IUnityContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            this.container = container;
        }

        public object GetService(Type serviceType)
        {
            try
            {
                return container.Resolve(serviceType);
            }
            catch (ResolutionFailedException)
            {
                return null;
            }
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            try
            {
                return container.ResolveAll(serviceType);
            }
            catch (ResolutionFailedException)
            {
                return new List<object>();
            }
        }

        public IDependencyScope BeginScope()
        {
            var child = container.CreateChildContainer();
            return new UnityResolver(child);
        }

        public void Dispose()
        {
            container.Dispose();
        }
    }
    #endregion
}
