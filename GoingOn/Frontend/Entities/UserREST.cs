// ****************************************************************************
// <copyright file="User.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace GoingOn.Frontend.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;

    using GoingOn.Frontend.Links;
    using GoingOn.Model.EntitiesBll;

    public class UserREST
    {
        public User User { get; private set; }

        public DateTime RegistrationDate { get; private set; }

        public IList<Link> Links { get; private set; }

        public UserREST(User user, DateTime registrationDate)
        {
            this.User = user;
            this.RegistrationDate = registrationDate;
            this.Links = new List<Link>();
        }

        private UserREST(User user, DateTime registrationDate, HttpRequestMessage request) : this(user, registrationDate)
        {
            this.Links.Add(new UserLinkFactory(request).Self(user.Nickname));
        }

        public static UserREST FromUserBll(UserBll user, HttpRequestMessage request)
        {
            return new UserREST(
                new User
                {
                    Nickname = user.Nickname, 
                    Password = user.Password, 
                    City = user.City, 
                    Name = user.Name,
                    Email = user.Email,
                    BirthDate = user.BirthDate
                }, 
                user.RegistrationDate,
                request);
        }
    }
}