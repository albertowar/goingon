namespace GoingOn.Persistence.MemoryStorage
{
    using GoingOn.Models.EntitiesBll;
    using GoingOn.Persistence.MemoryDB;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public class UserMemoryStorage : IUserStorage
    {
        private static List<UserBll> storage = new List<UserBll>();

        public void AddUser(UserBll user)
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

        public UserBll GetUser(string username)
        {
            if (!storage.Any(user => user.Nickname == username))
            {
                throw new PersistenceException("The user does not exist in the database");
            }

            return storage.Where(user => user.Nickname == username).First();
        }

        public IEnumerable<UserBll> GetAllUsers()
        {
            return storage;
        }

        public void DeleteUser(UserBll user)
        {
            if (!storage.Contains(user))
            {
                throw new PersistenceException("The userdoes not exist in the database");
            }

            storage.Remove(user);
        }
    }
}