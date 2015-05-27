// ****************************************************************************
// <copyright file="NewsController.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// News controller class
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
    using GoingOn.Repository;

    /// <summary>
    /// 
    /// </summary>
    public class NewsController : GoingOnApiController
    {
        private readonly INewsRepository repository;
        private readonly IApiInputValidationChecks inputValidation;
        private readonly IApiBusinessLogicValidationChecks businessValidation;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newsTableRepository"></param>
        /// <param name="inputValidation"></param>
        /// <param name="businessValidation"></param>
        public NewsController(INewsRepository newsTableRepository, IApiInputValidationChecks inputValidation, IApiBusinessLogicValidationChecks businessValidation)
        {
            this.repository = newsTableRepository;
            this.inputValidation = inputValidation;
            this.businessValidation = businessValidation;
        }

        /// <summary>
        /// Get the news.
        /// </summary>
        /// <param name="city">The city of the news.</param>
        /// <param name="date">The date when the news was published.</param>
        /// <param name="newsId">The identifier of the news.</param>
        /// <returns></returns>
        [Route(GOUriBuilder.GetNewsTemplate)]
        [HttpGet]
        public async Task<HttpResponseMessage> Get(string city, string date, string newsId)
        {
            return await this.ValidateExecute(this.ExecuteGetAsync, city, date, newsId);
        }

        /// <summary>
        /// Creates a news.
        /// </summary>
        /// <param name="city">The city of the news.</param>
        /// <param name="date">The date when the news was published.</param>
        /// <param name="news">The news to create.</param>
        /// <returns></returns>
        [IdentityBasicAuthentication]
        [Authorize]
        [Route(GOUriBuilder.PostNewsTemplate)]
        [HttpPost]
        public async Task<HttpResponseMessage> Post(string city, string date, [FromBody]News news)
        {
            // TODO: validate if the user is the owner of the vote

            return await this.ValidateExecute(this.ExecutePostAsync, city, date, news);
        }

        /// <summary>
        /// Updates the news.
        /// </summary>
        /// <param name="city">The city of the news.</param>
        /// <param name="date">The date when the news was published.</param>
        /// <param name="newsId">The identifier of the news.</param>
        /// <param name="news">The news to update.</param>
        /// <returns></returns>
        [IdentityBasicAuthentication]
        [Authorize]
        [Route(GOUriBuilder.PatchNewsTemplate)]
        [HttpPatch]
        public async Task<HttpResponseMessage> Patch(string city, string date, string newsId, [FromBody]News news)
        {
            // TODO: validate if the user is the owner of the vote

            return await this.ValidateExecute(this.ExecutePatchAsync, city, date, newsId, news);
        }

        /// <summary>
        /// Deletes the news.
        /// </summary>
        /// <param name="city">The city of the news.</param>
        /// <param name="date">The date when the news was published.</param>
        /// <param name="newsId">The identifier of the news.</param>
        /// <returns></returns>
        [IdentityBasicAuthentication]
        [Authorize]
        [Route(GOUriBuilder.DeleteNewsTemplate)]
        [HttpDelete]
        public async Task<HttpResponseMessage> Delete(string city, string date, string newsId)
        {
            // TODO: validate if the user is the owner of the vote

            return await this.ValidateExecute(this.ExecuteDeleteAsync, city, date, newsId);
        }

        #region Operations code

        private async Task<HttpResponseMessage> ExecuteGetAsync(params object[] parameters)
        {
            var city = (string)parameters[0];
            var date = (string)parameters[1];
            var newsId = (string)parameters[2];

            await this.ValidateGetNewsOperation(city, date, newsId);

            NewsREST news = NewsREST.FromNewsBll(await this.repository.GetNews(city, DateTime.Parse(date), Guid.Parse(newsId)), this.Request);

            HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, news);

            return response;
        }

        private async Task<HttpResponseMessage> ExecutePostAsync(params object[] parameters)
        {
            var city = (string)parameters[0];
            var date = (string)parameters[1];
            var news = (News)parameters[2];

            string nickname = this.User.Identity.Name;

            await this.ValidatePostNewsOperation(city, date, news, nickname);

            Guid newsId = Guid.NewGuid();

            await this.repository.AddNews(News.ToNewsBll(newsId, news, city, nickname, DateTime.Parse(date)));

            HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.Created, "The news was added to the database");
            response.Headers.Location = new NewsLinkFactory(this.Request).Self(city, date, newsId.ToString()).Href;

            return response;
        }

        private async Task<HttpResponseMessage> ExecutePatchAsync(params object[] parameters)
        {
            var city = (string)parameters[0];
            var date = (string)parameters[1];
            var newsId = (string)parameters[2];
            var news = (News)parameters[3];

            await this.ValidatePatchNewsOperation(city, date, newsId, news);

            await this.repository.UpdateNews(News.ToNewsBll(Guid.Parse(newsId), news, city, this.User.Identity.Name, DateTime.Parse(date)));

            HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, "The news was added to the database");

            return response;
        }

        private async Task<HttpResponseMessage> ExecuteDeleteAsync(params object[] parameters)
        {
            string city = (string)parameters[0];
            string date = (string)parameters[1];
            string newsId = (string)parameters[2];

            await this.ValidateDeleteNewsOperation(city, date, newsId);

            await this.repository.DeleteNews(city, DateTime.Parse(date), Guid.Parse(newsId));

            return this.Request.CreateResponse(HttpStatusCode.NoContent, "The news was deleted");
        }

        #endregion

        #region Validation code

        public async Task ValidateGetNewsOperation(string city, string date, string id)
        {
            this.inputValidation.ValidateNewsParameters(city, date, id);

            if (!(await this.businessValidation.IsValidGetNews(this.repository, city, DateTime.Parse(date), Guid.Parse(id))))
            {
                throw new BusinessValidationException(HttpStatusCode.NotFound, "The news is not in the database");
            }
        }

        public async Task ValidatePostNewsOperation(string city, string date, News news, string nickname)
        {
            this.inputValidation.ValidateDiaryEntryParameters(city, date);

            if (!this.inputValidation.IsValidNews(news))
            {
                throw new InputValidationException(HttpStatusCode.BadRequest, "The news format is incorrect");
            }

            if (!await this.businessValidation.IsValidCreateNews(this.repository, news, city, nickname, DateTime.Parse(date)))
            {
                throw new BusinessValidationException(HttpStatusCode.BadRequest, "The news is already created");
            }
        }

        public async Task ValidatePatchNewsOperation(string city, string date, string id, News news)
        {
            this.inputValidation.ValidateNewsParameters(city, date, id);

            if (!this.inputValidation.IsValidNews(news))
            {
                throw new InputValidationException(HttpStatusCode.BadRequest, "The news format is incorrect");
            }

            if (!await this.businessValidation.IsValidUpdateNews(this.repository, city, DateTime.Parse(date), Guid.Parse(id), this.User.Identity.Name))
            {
                throw new BusinessValidationException(HttpStatusCode.NotFound, "The news does not exist");
            }
        }

        public async Task ValidateDeleteNewsOperation(string city, string date, string id)
        {
            this.inputValidation.ValidateNewsParameters(city, date, id);

            if (!await this.businessValidation.IsValidDeleteNews(this.repository, city, DateTime.Parse(date), Guid.Parse(id), this.User.Identity.Name))
            {
                throw new BusinessValidationException(HttpStatusCode.NotFound, "The news does not exist");
            }
        }

        #endregion
    }
}
