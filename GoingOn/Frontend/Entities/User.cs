// ****************************************************************************
// <copyright file="User.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// Encapsulats the user information as the user would create it
// </summary>
// ****************************************************************************

namespace GoingOn.Frontend.Entities
{
    using System;
    using GoingOn.Model.EntitiesBll;

    public class User
    {
        public string Nickname { get; set; }

        public string Password { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public DateTime? BirthDate { get; set; }

        public override bool Equals(Object userObject)
        {
            var user = userObject as User;

            return 
                user != null && 
                string.Equals(this.Nickname, user.Nickname, StringComparison.Ordinal);
        }

        public override int GetHashCode()
        {
            return 
                Nickname.GetHashCode() ^ 
                Password.GetHashCode() ^
                Name.GetHashCode() ^
                Email.GetHashCode() ^
                BirthDate.GetHashCode();
        }

        public static UserBll ToUserBll(string city, User user)
        {
            return new UserBll 
            {
                Nickname = user.Nickname,
                Password = user.Password,
                City = city,
                Name = user.Name,
                Email = user.Email,
                BirthDate = user.BirthDate
            };
        }
    }
}