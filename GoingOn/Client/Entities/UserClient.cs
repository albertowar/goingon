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

        public static UserClient FromFrontendUser(User user)
        {
            return new UserClient { Nickname = user.Nickname, Password = user.Password };
        }

        public static User ToFrontendUser(UserClient userClient)
        {
            return new User(userClient.Nickname, userClient.Password);
        }
    }
}
