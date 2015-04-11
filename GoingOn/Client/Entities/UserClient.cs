// ****************************************************************************
// <copyright file="UserClient.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace GoingOn.Client.Entities
{
    using GoingOn.Frontend.Entities;

    public class UserClient
    {
        public string Nickname { get; set; }

        public string Password { get; set; }

        public string City { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public static UserClient FromUserREST(UserREST userREST)
        {
            return new UserClient { Nickname = userREST.User.Nickname, Password = userREST.User.Password, City = userREST.User.City};
        }

        public static User ToUserClientUser(UserClient userClient)
        {
            return new User { Nickname = userClient.Nickname, Password = userClient.Password, City = userClient.City };
        }
    }
}
