// ****************************************************************************
// <copyright file="UnityResolver.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// Handles the resolution of the dependency injection
// </summary>
// ****************************************************************************

namespace GoingOn.FrontendWebRole.DependencyInjection
{
    using System;
    using System.Collections.Generic;
    using System.Web.Http.Dependencies;
    using Microsoft.Practices.Unity;

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
            IUnityContainer child = this.container.CreateChildContainer();

            return new UnityResolver(child);
        }

        public void Dispose()
        {
            this.container.Dispose();
        }
    }
}
