using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GoingOn.Models.EntitiesBll
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