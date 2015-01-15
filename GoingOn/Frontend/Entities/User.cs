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
    using System;

    using Model.EntitiesBll;

    public class User
    {
        public string Nickname { get; set; }

        public string Password { get; set; }

        public string City { get; set; }

        public override bool Equals(Object userObject)
        {
            var user = userObject as User;

            return 
                user != null && 
                this.Nickname == user.Nickname;
        }

        public override int GetHashCode()
        {
            return 
                Nickname.GetHashCode() ^ 
                Password.GetHashCode() ^
                City.GetHashCode();
        }

        public static UserBll ToUserBll(User user)
        {
            return new UserBll 
            {
                Nickname = user.Nickname,
                Password = user.Password,
                City = user.City
            };
        }

        public static User FromUserBll(UserBll user)
        {
            return new User
            {
                Nickname = user.Nickname,
                Password = user.Password,
                City = user.City
            };
        }
    }
}