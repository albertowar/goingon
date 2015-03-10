﻿// ****************************************************************************
// <copyright file="NewsController.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace Frontend.Controllers
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;

    using Common;
    using Frontend.Entities;
    using Frontend.Links;
    using Frontend.Validation;
    using Storage;

    public class NewsController : ApiController, INewsController
    {
        private readonly INewsStorage storage;
        private readonly IApiInputValidationChecks inputValidation;
        private readonly IApiBusinessLogicValidationChecks businessValidation;

        public NewsController(INewsStorage storage, IApiInputValidationChecks inputValidation, IApiBusinessLogicValidationChecks businessValidation)
        {
            this.storage = storage;
            this.inputValidation = inputValidation;
            this.businessValidation = businessValidation;
        }

        public async Task<HttpResponseMessage> Get(string city, string date, string id)
        {
            try
            {
                await this.ValidateGetOperation(city, date, id);

                var news = NewsREST.FromNewsBll(await this.storage.GetNews(city, DateTime.Parse(date), Guid.Parse(id)), this.Request);

                var response = this.Request.CreateResponse(HttpStatusCode.OK, news);

                return response;
            }
            catch (InputValidationException inputValidationException)
            {
                return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, inputValidationException.Message);
            }
            catch (BusinessValidationException businessValidationException)
            {
                return this.Request.CreateErrorResponse(HttpStatusCode.NotFound, businessValidationException.Message);
            }
        }

        public async Task<HttpResponseMessage> Post(string city, string date, [FromBody]News news)
        {
            try
            {
                var nickname = this.User.Identity.Name;

                this.ValidatePostOperation(city, date, news, nickname);

                Guid newsId = Guid.NewGuid();

                await this.storage.AddNews(News.ToNewsBll(newsId, news, city, nickname, DateTime.Parse(date)));

                var response = this.Request.CreateResponse(HttpStatusCode.Created, "The news was added to the database");
                response.Headers.Location = new NewsLinkFactory(this.Request).Self(newsId.ToString()).Href;

                return response;
            }
            catch (InputValidationException inputValidationException)
            {
                return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, inputValidationException.Message);
            }
            catch (BusinessValidationException businessValidationException)
            {
                return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, businessValidationException.Message);
            }
        }

        public async Task<HttpResponseMessage> Patch(string city, string date, string id, [FromBody]News news)
        {
            try
            {
                await this.ValidatePatchOperation(city, date, id, news);

                await this.storage.UpdateNews(News.ToNewsBll(Guid.Parse(id), news, city, this.User.Identity.Name, DateTime.Parse(date)));

                var response = this.Request.CreateResponse(HttpStatusCode.OK, "The news was added to the database");

                return response;
            }
            catch (InputValidationException inputValidationException)
            {
                return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, inputValidationException.Message);
            }
            catch (BusinessValidationException businessValidationException)
            {
                return this.Request.CreateErrorResponse(HttpStatusCode.NotFound, businessValidationException.Message);
            }
        }

        public async Task<HttpResponseMessage> Delete(string city, string date, string id)
        {
            try
            {
                await this.ValidateDeleteOperation(city, date, id);

                await this.storage.DeleteNews(city, DateTime.Parse(date), Guid.Parse(id));

                return this.Request.CreateResponse(HttpStatusCode.NoContent, "The news was deleted");
            }
            catch (InputValidationException inputValidationException)
            {
                return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, inputValidationException.Message);
            }
            catch (BusinessValidationException businessValidationException)
            {
                return this.Request.CreateErrorResponse(HttpStatusCode.NotFound, businessValidationException.Message);
            }
        }

        #region Validation code

        public async Task ValidateGetOperation(string city, string date, string id)
        {
            if (!this.inputValidation.IsValidCity(city))
            {
                throw new InputValidationException("The city format is incorrect");
            }

            if (!this.inputValidation.IsValidNewsDate(date))
            {
                throw new InputValidationException("The date format is incorrect");
            }

            if (!this.inputValidation.IsValidNewsId(id))
            {
                throw new InputValidationException("The news id format is incorrect");
            }

            if (!await this.businessValidation.IsValidGetNews(this.storage, city, DateTime.Parse(date), Guid.Parse(id)))
            {
                throw new BusinessValidationException("The news is not in the database");
            }
        }

        public void ValidatePostOperation(string city, string date, News news, string nickname)
        {
            if (!this.inputValidation.IsValidCity(city))
            {
                throw new InputValidationException("The city format is incorrect");
            }

            if (!this.inputValidation.IsValidNewsDate(date))
            {
                throw new InputValidationException("The date format is incorrect");
            }

            if (!this.inputValidation.IsValidNews(news))
            {
                throw new InputValidationException("The news format is incorrect");
            }

            if (!this.businessValidation.IsValidCreateNews(this.storage, news, city, nickname, DateTime.Parse(date)))
            {
                throw new BusinessValidationException("The news is already created");
            }
        }

        public async Task ValidatePatchOperation(string city, string date, string id, News news)
        {
            if (!this.inputValidation.IsValidCity(city))
            {
                throw new InputValidationException("The city format is incorrect");
            }

            if (!this.inputValidation.IsValidNewsDate(date))
            {
                throw new InputValidationException("The date format is incorrect");
            }

            if (!this.inputValidation.IsValidNewsId(id))
            {
                throw new InputValidationException("The news id format is incorrect");
            }

            if (!this.inputValidation.IsValidNews(news))
            {
                throw new InputValidationException("The news format is incorrect");
            }

            if (!await this.businessValidation.IsValidUpdateNews(this.storage, city, DateTime.Parse(date), Guid.Parse(id), this.User.Identity.Name))
            {
                throw new BusinessValidationException("The news does not exist");
            }
        }

        public async Task ValidateDeleteOperation(string city, string date, string id)
        {
            if (!this.inputValidation.IsValidCity(city))
            {
                throw new InputValidationException("The city format is incorrect");
            }

            if (!this.inputValidation.IsValidNewsDate(date))
            {
                throw new InputValidationException("The date format is incorrect");
            }

            if (!this.inputValidation.IsValidNewsId(id))
            {
                throw new InputValidationException("The news id format is incorrect");
            }

            if (!await this.businessValidation.IsValidDeleteNews(this.storage, city, DateTime.Parse(date), Guid.Parse(id), this.User.Identity.Name))
            {
                throw new BusinessValidationException("The news does not exist");
            }
        }

        #endregion
    }
}
