// ****************************************************************************
// <copyright file="GoingOnApiController.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace GoingOn.FrontendWebRole.Controllers
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;
    using GoingOn.Frontend.Common;

    public abstract class GoingOnApiController : ApiController
    {
        public virtual async Task<HttpResponseMessage> ValidateExecute(Func<object[], Task<HttpResponseMessage>> operation, params object[] parameters)
        {
            try
            {
                return await operation(parameters);
            }
            catch (InputValidationException inputValidationException)
            {
                return this.Request.CreateErrorResponse(inputValidationException.StatusCode, inputValidationException.Message);
            }
            catch (BusinessValidationException businessValidationException)
            {
                return this.Request.CreateErrorResponse(businessValidationException.StatusCode, businessValidationException.Message);
            }
        }
    }
}