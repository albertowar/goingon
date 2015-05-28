// ****************************************************************************
// <copyright file="User.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// Encapsulates the user information as the server would return it
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
        /// <summary>
        /// The nickname of the user.
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// The password of the user.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// The name of the user.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The city where the user is registered to.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// The email of the user.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// The birthdate of the user.
        /// </summary>
        public DateTime? BirthDate { get; set; }

        /// <summary>
        /// Related information to the user.
        /// </summary>
        public IList<Link> Links { get; private set; }

        public UserREST(User user)
        {
            this.Nickname = user.Nickname;
            this.Password = user.Password;
            this.Name = user.Name;
            this.City = user.City;
            this.Email = user.Email;
            this.BirthDate = user.BirthDate;
            this.Links = new List<Link>();
        }

        private UserREST(User user, HttpRequestMessage request) : this(user)
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
                    Name = user.Name,
                    Email = user.Email,
                    BirthDate = user.BirthDate
                }, 
                request);
        }

        public static User ToUser(UserREST user)
        {
            return new User
            {
                Nickname = user.Nickname,
                Password = user.Password,
                Name = user.Name,
                Email = user.Email,
                BirthDate = user.BirthDate
            };
        }
    }
}