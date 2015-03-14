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
    using System.Diagnostics.CodeAnalysis;
    using System.Web.Http;
    using System.Web.Http.Dependencies;

    using Microsoft.Practices.Unity;
    
    using Frontend.Validation;
    using GoingOn.Storage;
    using GoingOn.Storage.TableStorage;

    [ExcludeFromCodeCoverage]
    [GeneratedCode("ASP.NET", "Visual Studio 2013")]
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration configuration)
        {
            // Dependy injection configuration
            var container = new UnityContainer();
            container.RegisterInstance<IUserStorage>(UserTableStorage.GetInstance());
            container.RegisterInstance<INewsStorage>(NewsTableStorage.GetInstance());
            container.RegisterInstance<IApiInputValidationChecks>(new ApiInputValidationChecks(new ApiInputValidationChecks()));
            container.RegisterInstance<IApiBusinessLogicValidationChecks>(new ApiBusinessLogicValidationChecks());
            configuration.DependencyResolver = new UnityResolver(container);

            // Web API routes
            configuration.MapHttpAttributeRoutes();
        }
    }

    #region Dependency injection helpers
    [ExcludeFromCodeCoverage]
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
                return this.container.Resolve(serviceType);
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
                return this.container.ResolveAll(serviceType);
            }
            catch (ResolutionFailedException)
            {
                return new List<object>();
            }
        }

        public IDependencyScope BeginScope()
        {
            var child = this.container.CreateChildContainer();
            return new UnityResolver(child);
        }

        public void Dispose()
        {
            container.Dispose();
        }
    }
    #endregion
}
