// ****************************************************************************
// <copyright file="UserMemoryStorage.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace Storage.MemoryStorage
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Model.EntitiesBll;
    using Storage.MemoryStorage.Entities;

    public class UserMemoryStorage : IUserStorage
    {
        // Singleton pattern
        private static UserMemoryStorage instance;

        private readonly List<UserMemory> storage;

        private UserMemoryStorage()
        {
            this.storage = new List<UserMemory>();
        }

        public Task AddUser(UserBll userBll)
        {
            storage.Add(UserMemory.FromUserBll(userBll));

            return Task.FromResult(0);
        }

        public Task<UserBll> GetUser(string nickname)
        {
            if (storage.Any(user => user.Equals(new UserMemory(nickname))))
            {
                return Task.FromResult(UserMemory.ToUserBll(storage.First(user => user.Equals(new UserMemory(nickname)))));
            }

            return Task.FromResult<UserBll>(null);
        }

        public Task<bool> ContainsUser(UserBll userBll)
        {
            return Task.FromResult(storage.Contains(UserMemory.FromUserBll(userBll)));
        }

        public Task UpdateUser(UserBll userBll)
        {
            var userMemory = UserMemory.FromUserBll(userBll);

            var foundUser = storage.Find(user => user.Equals(userMemory));

            if (foundUser != null)
            {
                foundUser.Merge(userMemory);
            }

            return Task.FromResult(0);
        }

        public Task DeleteUser(UserBll userBll)
        {
            storage.Remove(UserMemory.FromUserBll(userBll));

            return Task.FromResult(0);
        }

        public Task DeleteAllUser()
        {
            storage.Clear();

            return Task.FromResult(0);
        }

        public static UserMemoryStorage GetInstance()
        {
            if (UserMemoryStorage.instance == null)
            {
                UserMemoryStorage.instance = new UserMemoryStorage();
            }

            return UserMemoryStorage.instance;
        }
    }
}