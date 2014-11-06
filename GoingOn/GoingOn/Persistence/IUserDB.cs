using GoingOn.Frontend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GoingOn.Persistence
{
    public interface IUserDB
    {
        void AddUser(User user);
        User GetUser(string username);
        void DeleteUser(User user);
    }
}