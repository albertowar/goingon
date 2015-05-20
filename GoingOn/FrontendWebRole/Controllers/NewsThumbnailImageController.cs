// ****************************************************************************
// <copyright file="NewsThumbnailImageController.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// Controller for News Thumbnail Image Controller
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

    public class NewsThumbnailImageController : GoingOnApiController
    {
        private readonly INewsRepository newsRepository;
        private readonly IImageRepository newsImageBlobRepository;
        private readonly IApiInputValidationChecks inputValidation;
        private readonly IApiBusinessLogicValidationChecks businessValidation;

        public NewsThumbnailImageController(INewsRepository newsRepository, IImageRepository newsImageBlobRepository, IApiInputValidationChecks inputValidation, IApiBusinessLogicValidationChecks businessValidation)
        {
            this.newsRepository = newsRepository;
            this.newsImageBlobRepository = newsImageBlobRepository;
            this.inputValidation = inputValidation;
            this.businessValidation = businessValidation;
        }

        [Route(GOUriBuilder.NewsImageTemplate)]
        [HttpGet]
        public async Task<HttpResponseMessage> Get(string city, string date, string newsId)
        {
            return await this.ValidateExecute(this.ExecuteGetAsync, city, date, newsId);
        }

        #region Operations code

        private async Task<HttpResponseMessage> ExecuteGetAsync(params object[] parameters)
        {
            var city = (string)parameters[0];
            var date = (string)parameters[1];
            var newsId = (string)parameters[2];

            await this.ValidateGetOperation(city, date, newsId);

            Image image = await this.newsImageBlobRepository.GetNewsThumbnailImage(city, DateTime.Parse(date), Guid.Parse(newsId));

            var memoryStream = new MemoryStream();
            image.Save(memoryStream, ImageFormat.Png);

            HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);

            response.Content = new StreamContent(memoryStream);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");

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

            if (!(await this.businessValidation.IsValidGetImageNews(this.newsImageBlobRepository, city, DateTime.Parse(date), Guid.Parse(id))))
            {
                throw new BusinessValidationException(HttpStatusCode.NotFound, "The image news is not in the database");
            }
        }

        #endregion
    }
}