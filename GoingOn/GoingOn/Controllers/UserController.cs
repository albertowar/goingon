// ****************************************************************************
// <copyright file="UserController.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace GoingOn.Controllers
{
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using FrontendEntities = GoingOn.Entities;

    using GoingOn.Validation;

    public class UserController : ApiController
    {
        private readonly IUserStorage storage;
        private readonly IApiInputValidationChecks inputValidation;
        private readonly IApiBusinessLogicValidationChecks businessValidation;

        public UserController(IUserStorage storage, IApiInputValidationChecks inputValidation, IApiBusinessLogicValidationChecks businessValidation)
        {
            this.storage = storage;
            this.inputValidation = inputValidation;
            this.businessValidation = businessValidation;
        }

        // POST api/user
        public HttpResponseMessage Post([FromBody]FrontendEntities.User user)
        {
            if (!this.inputValidation.IsValidUser(user))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The user format is incorrect");
            }

            if (!this.businessValidation.IsValidUser(storage, user))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The user is already registered");
            }

            storage.AddUser(FrontendEntities.User.ToUserBll(user));

            return Request.CreateResponse(HttpStatusCode.OK, "The user was added to the database");
        }
    }
}