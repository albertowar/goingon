// ****************************************************************************
// <copyright file="NewsImageController.cs" company="Universidad de Malaga">
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
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Http;
    using GoingOn.Common;
    using GoingOn.Frontend.Common;
    using GoingOn.Frontend.Validation;
    using GoingOn.Storage;

    public class NewsImageController : GoingOnApiController
    {
        private readonly INewsStorage storage;
        private readonly IApiInputValidationChecks inputValidation;
        private readonly IApiBusinessLogicValidationChecks businessValidation;

        public NewsImageController(INewsStorage hotNewsTableStorage, IApiInputValidationChecks inputValidation, IApiBusinessLogicValidationChecks businessValidation)
        {
            this.storage = hotNewsTableStorage;
            this.inputValidation = inputValidation;
            this.businessValidation = businessValidation;
        }

        [Route(GOUriBuilder.PostNewsImageTemplate)]
        [HttpPost]
        public async Task<HttpResponseMessage> Post(string city, string date, string newsId)
        {
            return await this.ValidateExecute(this.ExecutePostAsync, city, date, newsId);
        }

        #region Operations code

        private async Task<HttpResponseMessage> ExecutePostAsync(params object[] parameters)
        {
            byte[] imageBytes = await this.Request.Content.ReadAsByteArrayAsync();

            MemoryStream ms = new MemoryStream(imageBytes);
            //Image returnImage = Image.FromStream(ms);

            string root = HttpContext.Current.Server.MapPath("~/App_Data");
            File.WriteAllBytes(root + "/goku.png", imageBytes);

            HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StreamContent(ms);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");

            return response;
        }

        #endregion

        #region Validation helpers

        public async Task ValidatePostOperation(string city, string date, string id)
        {
            this.inputValidation.ValidateNewsParameters(city, date, id);

            if (!(await this.businessValidation.IsValidGetNews(this.storage, city, DateTime.Parse(date), Guid.Parse(id))))
            {
                throw new BusinessValidationException("The news is not in the database");
            }
        }

        #endregion
    }
}