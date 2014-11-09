namespace GoingOn.Frontend.Entities
{
    using GoingOn.Frontend.Entities;
    using GoingOn.Models.EntitiesBll;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public class User
    {
        public string Nickname { get; private set; }
        public string Password { get; private set; }
        //public City City { get; private set; }

        public User(string nickname, string password)
        {
            Nickname = nickname;
            Password = password;
        }

        public override bool Equals(Object userObject)
        {
            var user = userObject as User;

            return user != null && this.Nickname == user.Nickname;
        }

        public override int GetHashCode()
        {
            return Nickname.GetHashCode() ^ Password.GetHashCode();
        }

        public static UserBll ToUserBll(User user)
        {
            return new UserBll(user.Nickname, user.Password);
        }

        public static User FromUserBll(UserBll user)
        {
            return new User(user.Nickname, user.Password);
        }
    }
}