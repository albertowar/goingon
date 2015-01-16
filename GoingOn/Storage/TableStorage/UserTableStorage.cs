// ****************************************************************************
// <copyright file="UserTableStorage.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

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
        /**
         * Most of the operations (if not all) are not using the PartitionKey to query the database.
         * Since the nickname is unique per user throughout the system, we need query using this value instead.
         * However, I will keep the PartitionKey as it is cause it might be used in future queries.
         **/

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
            // It needs to perform 2 queries, otherwise it would allow to have
            // two users with the same name in different cities

            await Task.Run(() =>
            {
                var table = GetStorageTable();

                if (this.ContainsUser(userBll).Result)
                {
                    throw new Storage.StorageException("The user is already in the database");
                }

                var user = UserEntity.FromUserBll(userBll);

                var insertOperation = TableOperation.Insert(user);

                table.Execute(insertOperation);
            });
        }

        public async Task<UserBll> GetUser(string nickname)
        {
            // Not using Contains so it does not perform two queries

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

                return UserTableStorage.FindUserByNickname(table, userBll.Nickname).Any();
            });
        }

        public async Task UpdateUser(UserBll userBll)
        {
            await Task.Run(() =>
            {
                var table = GetStorageTable();

                var retrievedUser = UserTableStorage.FindUserByNickname(table, userBll.Nickname).FirstOrDefault();

                if (retrievedUser == null)
                {
                    throw new Storage.StorageException("The user is not in the database");
                }

                retrievedUser.Merge(UserEntity.FromUserBll(userBll));

                var insertOrReplaceOperation = TableOperation.InsertOrReplace(retrievedUser);

                table.Execute(insertOrReplaceOperation);
            });
        }

        public async Task DeleteUser(UserBll userBll)
        {
            await Task.Run(() =>
            {
                var table = GetStorageTable();

                var retrievedUser = UserTableStorage.FindUserByNickname(table, userBll.Nickname).FirstOrDefault();

                if (retrievedUser == null)
                {
                    throw new Storage.StorageException("The user is not in the database");
                }

                var deleteOperation = TableOperation.Delete(retrievedUser);

                table.Execute(deleteOperation);
            });
        }

        public async Task DeleteAllUsers()
        {
            await Task.Run(() =>
            {
                var table = GetStorageTable();

                var query = new TableQuery<UserEntity>();

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
