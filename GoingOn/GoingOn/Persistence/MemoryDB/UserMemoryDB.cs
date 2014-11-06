namespace GoingOn.Persistence
{
    using GoingOn.Frontend;
    using GoingOn.Persistence.MemoryDB;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public class UserMemoryDB : IUserDB
    {
        private static List<User> storage = new List<User>();

        public void AddUser(User user)
        {
            if (user == null)
            {
                throw new PersistenceException("Invalid user");
            }

            if (storage.Contains(user))
            {
                throw new PersistenceException("The user is already in the database");
            }

            storage.Add(user);
        }

        public User GetUser(string username)
        {
            if (!storage.Any(user => user.Nickname == username))
            {
                throw new PersistenceException("The user does not exist in the database");
            }

            return storage.Where(user => user.Nickname == username).First();
        }

        public void DeleteUser(User user)
        {
            if (!storage.Contains(user))
            {
                throw new PersistenceException("The userdoes not exist in the database");
            }

            storage.Remove(user);
        }
    }
}