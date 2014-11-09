namespace GoingOn.Controllers
{
    using GoingOn.Frontend;
    using GoingOn.Frontend.Entities;
    using GoingOn.Models.EntitiesBll;
    using GoingOn.Persistence;
    using GoingOn.Persistence.MemoryDB;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web;
    using System.Web.Http;
    using FrontendEntities = GoingOn.Frontend.Entities;

    public class UserController : ApiController
    {
        private IUserStorage storage; 

        public UserController(IUserStorage storage)
        {
            this.storage = storage;
        }

        // GET api/user
        /// <summary>
        /// This call will just be used for testing purposes
        /// </summary>
        /// <returns>A list with all the registered users</returns>
        public IEnumerable<FrontendEntities.User> GetAllUsers()
        {
            return storage.GetAllUsers().Select(userBll => FrontendEntities.User.FromUserBll(userBll));
        }

        // POST api/user
        public HttpResponseMessage Post([FromBody]FrontendEntities.User user)
        {
            try
            {
                storage.AddUser(FrontendEntities.User.ToUserBll(user));
                return Request.CreateResponse(HttpStatusCode.OK, "The user was added to the database");
            } 
            catch (PersistenceException exception)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, exception.Message);  
            }
        }
    }
}