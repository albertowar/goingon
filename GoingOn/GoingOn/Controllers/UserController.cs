namespace GoingOn.Controllers
{
    using GoingOn.Frontend;
    using GoingOn.Persistence;
    using GoingOn.Persistence.MemoryDB;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web;
    using System.Web.Http;

    public class UserController : ApiController
    {
        // GET api/user
        public IEnumerable<User> Get()
        {
            throw new NotImplementedException();
        }

        // POST api/user
        public HttpResponseMessage Post([FromBody]User user)
        {
            IUserDB storage = new UserMemoryDB();

            try
            {
                storage.AddUser(user);
                return Request.CreateResponse(HttpStatusCode.OK, "The user was added to the database");
            } 
            catch (PersistenceException exception)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, exception.Message);  
            }
        }
    }
}