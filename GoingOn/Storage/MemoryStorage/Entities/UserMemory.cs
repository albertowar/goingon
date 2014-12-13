// ****************************************************************************
// <copyright file="UserMemory.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace MemoryStorage.Entities
{
    using Model.EntitiesBll;

    public class UserMemory
    {
        public string Nickname { get; private set; }
        public string Password { get; private set; }

        public UserMemory(string nickname, string password)
        {
            Nickname = nickname;
            Password = password;
        }

        public static UserMemory FromUserBll(UserBll userBll)
        {
            return new UserMemory(userBll.Nickname, userBll.Password);
        }

        public static UserBll ToUserBll(UserMemory userMemory)
        {
            return new UserBll(userMemory.Nickname, userMemory.Password);
        }

        public override bool Equals(object anotherUserObject)
        {
            UserMemory anotherUser = anotherUserObject as UserMemory;

            return
                anotherUser != null &&
                string.Equals(this.Nickname, anotherUser.Nickname);
        }

        public override int GetHashCode()
        {
            return 
                Nickname.GetHashCode() ^ 
                Password.GetHashCode();
        }
    }
}
