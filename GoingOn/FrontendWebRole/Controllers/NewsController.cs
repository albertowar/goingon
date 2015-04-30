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
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;

    using GoingOn.Common;
    using GoingOn.Frontend.Authentication;
    using GoingOn.Frontend.Common;
    using GoingOn.Frontend.Entities;
    using GoingOn.Frontend.Links;
    using GoingOn.Frontend.Validation;
    using GoingOn.Storage;
    using GoingOn.Storage.TableStorage.Entities;

    public class NewsController : ApiController
    {
        private readonly INewsStorage storage;
        private readonly IApiInputValidationChecks inputValidation;
        private readonly IApiBusinessLogicValidationChecks businessValidation;

        public NewsController(INewsStorage newsTableStorage, IApiInputValidationChecks inputValidation, IApiBusinessLogicValidationChecks businessValidation)
        {
            this.storage = newsTableStorage;
            this.inputValidation = inputValidation;
            this.businessValidation = businessValidation;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="city"></param>
        /// <param name="date"></param>
        /// <param name="newsId"></param>
        /// <returns></returns>
        [Route(GOUriBuilder.GetNewsTemplate)]
        [HttpGet]
        public async Task<HttpResponseMessage> Get(string city, string date, string newsId)
        {
            try
            {
                await this.ValidateGetOperation(city, date, newsId);

                var news = NewsREST.FromNewsBll(await this.storage.GetNews(city, DateTime.Parse(date), Guid.Parse(newsId)), this.Request);

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="city"></param>
        /// <param name="date"></param>
        /// <param name="news"></param>
        /// <returns></returns>
        [IdentityBasicAuthentication]
        [Authorize]
        [Route(GOUriBuilder.PostNewsTemplate)]
        [HttpPost]
        public async Task<HttpResponseMessage> Post(string city, string date, [FromBody]News news)
        {
            try
            {
                var nickname = this.User.Identity.Name;

                await this.ValidatePostOperation(city, date, news, nickname);

                Guid newsId = Guid.NewGuid();

                await this.storage.AddNews(NewsEntity.FromNewsBll(News.ToNewsBll(newsId, news, city, nickname, DateTime.Parse(date))));

                var response = this.Request.CreateResponse(HttpStatusCode.Created, "The news was added to the database");
                response.Headers.Location = new NewsLinkFactory(this.Request).Self(city, date, newsId.ToString()).Href;

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="city"></param>
        /// <param name="date"></param>
        /// <param name="newsId"></param>
        /// <param name="news"></param>
        /// <returns></returns>
        [IdentityBasicAuthentication]
        [Authorize]
        [Route(GOUriBuilder.PatchNewsTemplate)]
        [HttpPatch]
        public async Task<HttpResponseMessage> Patch(string city, string date, string newsId, [FromBody]News news)
        {
            try
            {
                await this.ValidatePatchOperation(city, date, newsId, news);

                await this.storage.UpdateNews(News.ToNewsBll(Guid.Parse(newsId), news, city, this.User.Identity.Name, DateTime.Parse(date)));

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="city"></param>
        /// <param name="date"></param>
        /// <param name="newsId"></param>
        /// <returns></returns>
        [IdentityBasicAuthentication]
        [Authorize]
        [Route(GOUriBuilder.DeleteNewsTemplate)]
        [HttpDelete]
        public async Task<HttpResponseMessage> Delete(string city, string date, string newsId)
        {
            try
            {
                await this.ValidateDeleteOperation(city, date, newsId);

                await this.storage.DeleteNews(city, DateTime.Parse(date), Guid.Parse(newsId));

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
                throw new InputValidationException("The news newsId format is incorrect");
            }

            if (!(await this.businessValidation.IsValidGetNews(this.storage, city, DateTime.Parse(date), Guid.Parse(id))))
            {
                throw new BusinessValidationException("The news is not in the database");
            }
        }

        public async Task ValidatePostOperation(string city, string date, News news, string nickname)
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

            if (!await this.businessValidation.IsValidCreateNews(this.storage, news, city, nickname, DateTime.Parse(date)))
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
                throw new InputValidationException("The news newsId format is incorrect");
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
                throw new InputValidationException("The news newsId format is incorrect");
            }

            if (!await this.businessValidation.IsValidDeleteNews(this.storage, city, DateTime.Parse(date), Guid.Parse(id), this.User.Identity.Name))
            {
                throw new BusinessValidationException("The news does not exist");
            }
        }

        #endregion
    }
}
