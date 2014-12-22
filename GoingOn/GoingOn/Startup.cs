// ****************************************************************************
// <copyright file="Startup.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

using GoingOn.Links;
using Microsoft.Owin;

[assembly: OwinStartup(typeof(GoingOn.Startup))]

namespace GoingOn
{
    using System.CodeDom.Compiler;
    using System.Diagnostics.CodeAnalysis;
    using System.Web.Http;
    using System.Web.Mvc;

    using Microsoft.Practices.Unity;

    using GoingOn.Validation;
    using MemoryStorage;
    
    using Owin;

    [ExcludeFromCodeCoverage]
    [GeneratedCode("ASP.NET", "Visual Studio 2013")]
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
            var configuration = new HttpConfiguration();
            WebApiConfig.Register(configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

            // Dependy injection configuration
            var container = new UnityContainer();
            container.RegisterInstance<IUserStorage>(UserMemoryStorage.GetInstance());
            container.RegisterInstance<IApiInputValidationChecks>(new ApiInputValidationChecks());
            container.RegisterInstance<IApiBusinessLogicValidationChecks>(new ApiBusinessLogicValidationChecks());
            configuration.DependencyResolver = new UnityResolver(container);

            app.UseWebApi(configuration);
        }
    }
}
