// ****************************************************************************
// <copyright file="UserBll.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace Model.EntitiesBll
{
    public class UserBll
    {
        public string Nickname { get; private set; }
        public string Password { get; private set; }

        public UserBll(string nickname, string password)
        {
            Nickname = nickname;
            Password = password;
        }
    }
}