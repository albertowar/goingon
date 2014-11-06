namespace GoingOn.Frontend
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public class User
    {
        public string Nickname { get; private set; }
        public string Password { get; private set; }

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
    }
}