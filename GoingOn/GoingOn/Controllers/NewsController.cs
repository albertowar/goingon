namespace GoingOn.Controllers
{
    using GoingOn.Frontend;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;

    public class NewsController : ApiController
    {
        // GET api/news
        public IEnumerable<News> Get()
        {
            return new News[] { new News("News 1"), new News("News 2") };
        }
    }
}
