namespace Frontend.Entities
{
    using Frontend.Entities;
    using GoingOn.Links;
    using Model.EntitiesBll;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Web;

    public class User
    {
        public string Nickname { get; private set; }
        public string Password { get; private set; }
        //public City City { get; private set; }
        public IList<Link> Links { get; private set; }

        public User(string nickname, string password)
        {
            Nickname = nickname;
            Password = password;
            Links = new List<Link>();
        }

        private User(string nickname, string password, HttpRequestMessage request) : this(nickname, password)
        {
            Links.Add(new UserLinkFactory(request).Self(nickname));
        }

        public override bool Equals(Object userObject)
        {
            var user = userObject as User;

            return user != null && this.Nickname == user.Nickname;
        }

        public override int GetHashCode()
        {
            return Nickname.GetHashCode() ^ Password.GetHashCode();
        }

        public static UserBll ToUserBll(User user)
        {
            return new UserBll(user.Nickname, user.Password);
        }

        public static User FromUserBll(UserBll user)
        {
            return new User(user.Nickname, user.Password);
        }

        public static User FromUserBll(UserBll user, HttpRequestMessage request)
        {
            return new User(user.Nickname, user.Password, request);
        }
    }
}