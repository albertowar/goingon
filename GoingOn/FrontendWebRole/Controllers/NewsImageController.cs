// ****************************************************************************
// <copyright file="NewsImageController.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// Controller for News Image
// </summary>
// ****************************************************************************

namespace GoingOn.FrontendWebRole.Controllers
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using System.Web.Http;
    using GoingOn.Common;
    using GoingOn.Frontend.Common;
    using GoingOn.Frontend.Validation;
    using GoingOn.Repository;
    using GoingOn.XStoreProxy;

    public class NewsImageController : GoingOnApiController
    {
        private readonly INewsRepository newsRepository;
        private readonly IImageRepository _imageBlobRepository;
        private readonly IApiInputValidationChecks inputValidation;
        private readonly IApiBusinessLogicValidationChecks businessValidation;

        // TODO: handle different formats (just PNG for now)
        // Use Image.RawFormat and compare with existing formats

        public NewsImageController(INewsRepository newsRepository, IImageRepository _imageBlobRepository, IApiInputValidationChecks inputValidation, IApiBusinessLogicValidationChecks businessValidation)
        {
            this.newsRepository = newsRepository;
            this._imageBlobRepository = _imageBlobRepository;
            this.inputValidation = inputValidation;
            this.businessValidation = businessValidation;
        }

        [Route(GOUriBuilder.NewsImageTemplate)]
        [HttpGet]
        public async Task<HttpResponseMessage> Get(string city, string date, string newsId)
        {
            return await this.ValidateExecute(this.ExecuteGetAsync, city, date, newsId);
        }

        [Route(GOUriBuilder.NewsImageTemplate)]
        [HttpPost]
        public async Task<HttpResponseMessage> Post(string city, string date, string newsId)
        {
            return await this.ValidateExecute(this.ExecutePostAsync, city, date, newsId);
        }

        [Route(GOUriBuilder.NewsImageTemplate)]
        [HttpDelete]
        public async Task<HttpResponseMessage> Delete(string city, string date, string newsId)
        {
            return await this.ValidateExecute(this.ExecuteDeletetAsync, city, date, newsId);
        }

        #region Operations code

        private async Task<HttpResponseMessage> ExecuteGetAsync(params object[] parameters)
        {
            var city = (string)parameters[0];
            var date = (string)parameters[1];
            var newsId = (string)parameters[2];

            await this.ValidateGetOperation(city, date, newsId);

            Image image = await this._imageBlobRepository.GetNewsImage(city, DateTime.Parse(date), Guid.Parse(newsId));

            var memoryStream = new MemoryStream();
            image.Save(memoryStream, ImageFormat.Png);

            HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);

            response.Content = new StreamContent(memoryStream);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");

            return response;
        }

        private async Task<HttpResponseMessage> ExecutePostAsync(params object[] parameters)
        {
            var city = (string) parameters[0];
            var date = (string) parameters[1];
            var newsId = (string) parameters[2];

            await this.ValidatePostOperation(city, date, newsId);

            byte[] imageBytes = await this.Request.Content.ReadAsByteArrayAsync();

            var memoryStream = new MemoryStream(imageBytes);

            Image image = Image.FromStream(memoryStream);

            await this._imageBlobRepository.CreateNewsImage(city, DateTime.Parse(date), Guid.Parse(newsId), image);

            HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);

            return response;
        }

        private async Task<HttpResponseMessage> ExecuteDeletetAsync(params object[] parameters)
        {
            var city = (string)parameters[0];
            var date = (string)parameters[1];
            var newsId = (string)parameters[2];

            await this.ValidateDeleteOperation(city, date, newsId);

            HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.NoContent);

            return response;
        }

        #endregion

        #region Validation helpers

        private async Task ValidateGetOperation(string city, string date, string id)
        {
            this.inputValidation.ValidateNewsParameters(city, date, id);

            if (!(await this.businessValidation.IsValidGetNews(this.newsRepository, city, DateTime.Parse(date), Guid.Parse(id))))
            {
                throw new BusinessValidationException(HttpStatusCode.NotFound, "The news is not in the database");
            }

            if (!(await this.businessValidation.IsValidGetImageNews(this._imageBlobRepository, city, DateTime.Parse(date), Guid.Parse(id))))
            {
                throw new BusinessValidationException(HttpStatusCode.NotFound, "The image news is not in the database");
            }
        }

        private async Task ValidatePostOperation(string city, string date, string id)
        {
            this.inputValidation.ValidateNewsParameters(city, date, id);

            if (!(await this.businessValidation.IsValidGetNews(this.newsRepository, city, DateTime.Parse(date), Guid.Parse(id))))
            {
                throw new BusinessValidationException(HttpStatusCode.NotFound, "The news is not in the database");
            }
        }

        private async Task ValidateDeleteOperation(string city, string date, string id)
        {
            this.inputValidation.ValidateNewsParameters(city, date, id);

            if (!(await this.businessValidation.IsValidGetNews(this.newsRepository, city, DateTime.Parse(date), Guid.Parse(id))))
            {
                throw new BusinessValidationException(HttpStatusCode.NotFound, "The news is not in the database");
            }

            if (!(await this.businessValidation.IsValidGetImageNews(this._imageBlobRepository, city, DateTime.Parse(date), Guid.Parse(id))))
            {
                throw new BusinessValidationException(HttpStatusCode.NotFound, "The image news is not in the database");
            }
        }

        #endregion
    }
}