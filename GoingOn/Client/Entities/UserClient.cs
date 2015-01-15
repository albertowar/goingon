// ****************************************************************************
// <copyright file="UserClient.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace Client.Entities
{
    using Frontend.Entities;

    public class UserClient
    {
        public string Nickname { get; set; }

        public string Password { get; set; }

        public static UserClient FromUserREST(UserREST userREST)
        {
            return new UserClient { Nickname = userREST.User.Nickname, Password = userREST.User.Password };
        }

        public static User ToUserClientUser(UserClient userClient)
        {
            return new User(userClient.Nickname, userClient.Password);
        }
    }
}
