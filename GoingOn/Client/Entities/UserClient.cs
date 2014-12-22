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
    using FrontendEntities = GoingOn.Entities;

    public class UserClient
    {
        public UserClient()
        {
            
        }

        public UserClient(string nickname, string password)
        {
            this.Nickname = nickname;
            this.Password = password;
        }

        public string Nickname { get; set; }

        public string Password { get; set; }

        public static UserClient FromFrontendUser(FrontendEntities.User user)
        {
            return new UserClient(user.Nickname, user.Password);
        }

        public static FrontendEntities.User ToFrontendUser(UserClient userClient)
        {
            return new FrontendEntities.User(userClient.Nickname, userClient.Password);
        }
    }
}
