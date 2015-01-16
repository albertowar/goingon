// ****************************************************************************
// <copyright file="UserTableStorage.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;

namespace Storage.TableStorage
{
    using System.Configuration;
    using System.Threading.Tasks;

    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;

    using Model.EntitiesBll;
    using Storage.TableStorage.Entities;

    using TableStorageException = Microsoft.WindowsAzure.Storage.StorageException;

    public class UserTableStorage : IUserStorage
    {
        // Configuration info
        private static readonly string TableName = ConfigurationManager.AppSettings["UserTableName"];
        private static readonly string StorageConnectionString = ConfigurationManager.AppSettings["StorageConnectionString"];

        // Singleton pattern
        private static UserTableStorage instance;

        // Retrieve the storage account from the connection string.
        private static readonly CloudStorageAccount StorageAccount = CloudStorageAccount.Parse(StorageConnectionString);

        private UserTableStorage()
        {
        }

        public static UserTableStorage GetInstance()
        {
            if (UserTableStorage.instance == null)
            {
                UserTableStorage.instance = new UserTableStorage();
            }

            return UserTableStorage.instance;
        }

        public async Task AddUser(UserBll userBll)
        {
            await Task.Run(() =>
            {
                try
                {
                    var table = GetStorageTable();

                    var user = UserEntity.FromUserBll(userBll);

                    var insertOperation = TableOperation.Insert(user);

                    table.Execute(insertOperation);
                }
                catch (StorageException)
                {
                    throw new Storage.StorageException("The user cannot be added to the database");
                }
            });
        }

        public async Task<UserBll> GetUser(string nickname)
        {
            return await Task.Run(() =>
            {
                var table = GetStorageTable();

                var retrievedUser = UserTableStorage.FindUserByNickname(table, nickname).FirstOrDefault();

                if (retrievedUser == null)
                {
                    throw new Storage.StorageException("The user is not in the database");
                }

                return UserEntity.ToUserBll(retrievedUser);
            });
        }

        public async Task<bool> ContainsUser(UserBll userBll)
        {
            return await Task.Run(() =>
            {
                var table = GetStorageTable();

                if (userBll.City != null)
                {
                    var retrieveOperation = TableOperation.Retrieve<UserEntity>(userBll.City, userBll.Nickname);

                    var retrievedResult = table.Execute(retrieveOperation);

                    return retrievedResult.Result != null;
                }
                else
                {
                    return UserTableStorage.FindUserByNickname(table, userBll.Nickname).Any();
                }
            });
        }

        public async Task UpdateUser(UserBll userBll)
        {
            await Task.Run(() =>
            {
                var table = GetStorageTable();

                var retrieveOperation = TableOperation.Retrieve<UserEntity>(userBll.City, userBll.Nickname);

                var retrievedResult = table.Execute(retrieveOperation);

                var updateEntity = retrievedResult.Result as UserEntity;

                if (updateEntity != null)
                {
                    updateEntity.Merge(UserEntity.FromUserBll(userBll));

                    var insertOrReplaceOperation = TableOperation.InsertOrReplace(updateEntity);

                    table.Execute(insertOrReplaceOperation);
                }
            });
        }

        public async Task DeleteUser(UserBll userBll)
        {
            await Task.Run(() =>
            {
                var table = GetStorageTable();

                var retrieveOperation = TableOperation.Retrieve<UserEntity>(userBll.City, userBll.Nickname);

                var retrievedResult = table.Execute(retrieveOperation);

                var deleteEntity = retrievedResult.Result as UserEntity;

                if (deleteEntity != null)
                {
                    var deleteOperation = TableOperation.Delete(deleteEntity);

                    table.Execute(deleteOperation);
                }
            });
        }

        public async Task DeleteAllUsers(string city)
        {
            await Task.Run(() =>
            {
                var table = GetStorageTable();

                var query = new TableQuery<UserEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, city));

                foreach (var entity in table.ExecuteQuery(query))
                {
                    var deleteOperation = TableOperation.Delete(entity);
                    table.Execute(deleteOperation);
                }
            });
        }

        #region Helper methods

        private CloudTable GetStorageTable()
        {
            var tableClient = StorageAccount.CreateCloudTableClient();

            return tableClient.GetTableReference(TableName);
        }

        private static IEnumerable<UserEntity> FindUserByNickname(CloudTable table, string nickname)
        {
            var query = new TableQuery<UserEntity>()
                .Where(TableQuery.GenerateFilterCondition(
                    "RowKey",
                    QueryComparisons.Equal,
                    nickname));

            return table.ExecuteQuery(query);
        }

        #endregion
    }
}
