// ****************************************************************************
// <copyright file="NewsController.cs" company="Universidad de Malaga">
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
    using GoingOn.Model.EntitiesBll;
    using GoingOn.Storage;

    public class HotNewsController : ApiController
    {
        private readonly IHotNewsStorage storage;
        private readonly IApiInputValidationChecks inputValidation;
        private readonly IApiBusinessLogicValidationChecks businessValidation;

        public HotNewsController(IHotNewsStorage hotNewsTableStorage, IApiInputValidationChecks inputValidation, IApiBusinessLogicValidationChecks businessValidation)
        {
            this.storage = hotNewsTableStorage;
            this.inputValidation = inputValidation;
            this.businessValidation = businessValidation;
        }

        [Route(GOUriBuilder.GetHotNewsTemplate)]
        [HttpGet]
        public async Task<HttpResponseMessage> Get(string city)
        {
            try
            {
                await this.ValidateGetOperation(city);

                List<NewsREST> retrievedNews = (await this.storage.GetNews(city, DateTime.Today)).Select(news => NewsREST.FromNewsBll(news, this.Request)).ToList();

                if (!retrievedNews.Any())
                {
                    retrievedNews = (await this.storage.GetNews(city, DateTime.Now.AddDays(-1))).Select(news => NewsREST.FromNewsBll(news, this.Request)).ToList();
                }

                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retrievedNews);

                return response;
            }
            catch (InputValidationException inputValidationException)
            {
                return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, inputValidationException.Message);
            }
        }

        #region Validation code

        public async Task ValidateGetOperation(string city)
        {
            if (!this.inputValidation.IsValidCity(city))
            {
                throw new InputValidationException("The city format is incorrect");
            }
        }

        #endregion
    }
}