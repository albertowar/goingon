// ****************************************************************************
// <copyright file="HotNewsController.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// HotNews controller class
// </summary>
// ****************************************************************************

namespace GoingOn.FrontendWebRole.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;
    using GoingOn.Common;
    using GoingOn.Frontend.Common;
    using GoingOn.Frontend.Entities;
    using GoingOn.Frontend.Validation;
    using GoingOn.Repository;

    public class HotNewsController : GoingOnApiController
    {
        private readonly IHotNewsRepository repository;
        private readonly IApiInputValidationChecks inputValidation;
        private readonly IApiBusinessLogicValidationChecks businessValidation;

        public HotNewsController(IHotNewsRepository hotNewsTableRepository, IApiInputValidationChecks inputValidation, IApiBusinessLogicValidationChecks businessValidation)
        {
            this.repository = hotNewsTableRepository;
            this.inputValidation = inputValidation;
            this.businessValidation = businessValidation;
        }

        [Route(GOUriBuilder.GetHotNewsTemplate)]
        [HttpGet]
        public async Task<HttpResponseMessage> Get(string city)
        {
            return await this.ValidateExecute(this.ExecuteGetAsync, city);
        }

        #region Operations code

        private async Task<HttpResponseMessage> ExecuteGetAsync(params object[] parameters)
        {
            var city = (string)parameters[0];

            DateTime day = DateTime.Now;

            await this.ValidateGetOperation(city, day);

            List<NewsREST> retrievedNews =
                        (await this.repository.ListNews(city, day)).Select(
                            news => NewsREST.FromNewsBll(news, this.Request)).Take(10).ToList();

            HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retrievedNews);

            return response;
        }

        #endregion

        #region Validation code

        public async Task ValidateGetOperation(string city, DateTime date)
        {
            if (!this.inputValidation.IsValidCity(city))
            {
                throw new InputValidationException(HttpStatusCode.BadRequest, "The city format is incorrect");
            }

            if (!await this.businessValidation.IsValidGetHotNews(this.repository, city, date))
            {
                throw new BusinessValidationException(HttpStatusCode.NotFound, string.Format("There are not HotNews in {0} at {1}", city, date));
            }
        }

        #endregion
    }
}