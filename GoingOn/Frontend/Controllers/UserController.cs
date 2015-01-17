﻿// ****************************************************************************
// <copyright file="UserController.cs" company="Universidad de Malaga">
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

    using Frontend.Authentication;
    using Frontend.Entities;
    using Frontend.Links;
    using Frontend.Validation;
    using Storage;

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

        [IdentityBasicAuthentication]
        [Authorize]
        // GET api/user/{id}
        public async Task<HttpResponseMessage> Get(string id)
        {
            if (!this.inputValidation.IsValidNickName(id))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The user format is incorrect");
            }

            if (!this.businessValidation.IsValidGetUser(storage, id))
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "The user is not in the database");
            }
                
            var user = UserREST.FromUserBll(await storage.GetUser(id), Request);

            var response = Request.CreateResponse(HttpStatusCode.OK, user);

            return response;
        }

        // POST api/user
        public async Task<HttpResponseMessage> Post([FromBody]User user)
        {
            if (!this.inputValidation.IsValidUser(user))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The user format is incorrect");
            }

            if (!this.businessValidation.IsValidCreateUser(storage, user))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The user is already registered");
            }

            // TODO: validate input of the other parameters

            var userToAdd = Entities.User.ToUserBll(user);
            userToAdd.RegistrationDate = DateTime.Now;

            await storage.AddUser(userToAdd);

            var response = Request.CreateResponse(HttpStatusCode.Created, "The user was added to the database");
            response.Headers.Location = new UserLinkFactory(Request).Self(user.Nickname).Href;

            return response;
        }

        [IdentityBasicAuthentication]
        [Authorize]
        // PATCH api/user/{id}
        public async Task<HttpResponseMessage> Patch(string id, [FromBody]User user)
        {
            if (!this.inputValidation.IsValidNickName(id))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The user format is incorrect");
            }

            if (!this.inputValidation.IsValidUser(user))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The user format is incorrect");
            }

            if (!this.businessValidation.IsAuthorizedUser(User.Identity.Name, id))
            {
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "The user is not authorized to update another user");
            }

            if (!this.businessValidation.IsValidUpdateUser(storage, user))
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "The user is not registered");
            }

            var userToUpdate = Entities.User.ToUserBll(user);
            userToUpdate.RegistrationDate = DateTime.Now;

            await storage.UpdateUser(userToUpdate);

            return Request.CreateResponse(HttpStatusCode.NoContent, "The user was updated");
        }

        [IdentityBasicAuthentication]
        [Authorize]
        // DELETE api/user/{id}
        public async Task<HttpResponseMessage> Delete(string id)
        {
            if (!this.inputValidation.IsValidNickName(id))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The user format is incorrect");
            }

            if (!this.businessValidation.IsAuthorizedUser(User.Identity.Name, id))
            {
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "The user is not authorized to delete another user");
            }

            if (!this.businessValidation.IsValidDeleteUser(storage, id))
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "The user is not registered");
            }

            await storage.DeleteUser(Entities.User.ToUserBll(new User { Nickname = id }));

            return Request.CreateResponse(HttpStatusCode.NoContent, "The user was deleted");
        }
    }
}