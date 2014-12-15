// ****************************************************************************
// <copyright file="UserMemoryStorage.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace MemoryStorage
{
    using System.Collections.Generic;
    using System.Linq;

    using MemoryStorage.Entities;
    using Model.EntitiesBll;

    public class UserMemoryStorage : IUserStorage
    {
        // Singleton pattern
        private static UserMemoryStorage instance;

        private readonly List<UserMemory> storage;

        private UserMemoryStorage()
        {
            this.storage = new List<UserMemory>();
        }

        public void AddUser(UserBll userBll)
        {
            storage.Add(UserMemory.FromUserBll(userBll));
        }

        public UserBll GetUser(string nickname)
        {
            if (storage.Any(user => user.Equals(new UserMemory(nickname))))
            {
                return UserMemory.ToUserBll(storage.First(user => user.Equals(new UserMemory(nickname))));
            }

            return null;
        }

        public bool ContainsUser(UserBll userBll)
        {
            return storage.Contains(UserMemory.FromUserBll(userBll));
        }

        public void DeleteUser(UserBll userBll)
        {
            storage.Remove(UserMemory.FromUserBll(userBll));
        }

        public void DeleteAllUser()
        {
            storage.Clear();;
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