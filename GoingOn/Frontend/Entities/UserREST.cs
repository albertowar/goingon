// ****************************************************************************
// <copyright file="User.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace Frontend.Entities
{
    using System.Collections.Generic;
    using System.Net.Http;

    using Frontend.Links;
    using Model.EntitiesBll;

    public class UserREST
    {
        public User User { get; private set; }
        public IList<Link> Links { get; private set; }

        public UserREST(User user)
        {
            this.User = user;
            Links = new List<Link>();
        }

        private UserREST(User user, HttpRequestMessage request) : this(user)
        {
            Links.Add(new UserLinkFactory(request).Self(user.Nickname));
        }

        public static UserREST FromUserBll(UserBll user, HttpRequestMessage request)
        {
            return new UserREST(new User(user.Nickname, user.Password, user.City), request);
        }
    }
}