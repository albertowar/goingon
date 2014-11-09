using GoingOn.Frontend;
using GoingOn.Persistence;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace GoingOn.AcceptanceTests
{
    public abstract class UsersFeature
    {
        public Mock<IUserDB> MockIssueStore;
        public HttpResponseMessage Response;
        //public IssueLinkFactory IssueLinks;
        //public IssueStateFactory StateFactory;
        public IEnumerable<User> FakeIssues;
        public HttpRequestMessage Request { get; private set; }
        public HttpClient Client;

        public UsersFeature()
        {
            MockIssueStore = new Mock<IUserDB>();
            Request = new HttpRequestMessage();
            //IssueLinks = new IssueLinkFactory(Request);
            //StateFactory = new IssueStateFactory(IssueLinks);
            FakeIssues = GetFakeUsers();
            var config = new HttpConfiguration();
            GoingOn.Configure(config, MockIssueStore.Object);
            var server = new HttpServer(config);
            Client = new HttpClient(server);
        }

        private IEnumerable<User> GetFakeUsers()
        {
            var fakeIssues = new List<User>();
            fakeIssues.Add(new Issue { Id = "1", Title = "An issue", Description = "This is an issue", Status = IssueStatus.Open });
            fakeIssues.Add(new Issue { Id = "2", Title = "Another issue", Description = "This is another issue", Status = IssueStatus.Closed });
            return fakeIssues;
        }
    }
}
