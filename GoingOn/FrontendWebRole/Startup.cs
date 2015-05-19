// ****************************************************************************
// <copyright file="Startup.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// Startup class for E2E tests
// </summary>
// ****************************************************************************

using GoingOn.FrontendWebRole;
using Microsoft.Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace GoingOn.FrontendWebRole
{
    using System.CodeDom.Compiler;
    using System.Diagnostics.CodeAnalysis;
    using System.Web.Http;
    using System.Web.Mvc;
    using Owin;

    /// <summary>
    /// OWIN startup class is just used for testing purposes
    /// </summary>
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

            app.UseWebApi(configuration);
        }
    }
}
