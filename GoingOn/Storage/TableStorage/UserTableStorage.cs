// ****************************************************************************
// <copyright file="UserTableStorage.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace GoingOn.Storage.TableStorage
{
    using System.Configuration;
    using System.Linq;
    using System.Threading.Tasks;

    using GoingOn.Model.EntitiesBll;
    using GoingOn.Storage;
    using GoingOn.Storage.TableStorage.Entities;

    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;
    
    using StorageException = GoingOn.Storage.StorageException;

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
        private readonly CloudStorageAccount storageAccount = CloudStorageAccount.Parse(StorageConnectionString);

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
                var table = this.GetStorageTable();

                if (this.ContainsUser(userBll).Result)
                {
                    throw new StorageException("The user is already in the database");
                }

                var user = UserEntity.FromUserBll(userBll);

                var insertOperation = TableOperation.Insert(user);

                table.Execute(insertOperation);
            });
        }

        public async Task<UserBll> GetUser(string nickname)
        {
            // Not using Contains so it does not perform two queries

            var table = this.GetStorageTable();

            var retrievedUserSegment = await UserTableStorage.FindUserByNickname(table, nickname);

            var firstUser = retrievedUserSegment.Results.FirstOrDefault();

            if (firstUser == null)
            {
                throw new StorageException("The user is not in the database");
            }

            return UserEntity.ToUserBll(firstUser);
        }

        public async Task<bool> ContainsUser(UserBll userBll)
        {
            var table = this.GetStorageTable();

            var userFound = await UserTableStorage.FindUserByNickname(table, userBll.Nickname);

            return userFound.Results.Any();
        }

        public async Task UpdateUser(UserBll userBll)
        {
            var table = this.GetStorageTable();

            var retrievedUserSegment = await UserTableStorage.FindUserByNickname(table, userBll.Nickname);

            var firstUser = retrievedUserSegment.Results.FirstOrDefault();

            if (firstUser == null)
            {
                throw new StorageException("The user is not in the database");
            }

            firstUser.Merge(UserEntity.FromUserBll(userBll));

            var insertOrReplaceOperation = TableOperation.InsertOrReplace(firstUser);

            await table.ExecuteAsync(insertOrReplaceOperation);
        }

        public async Task DeleteUser(UserBll userBll)
        {
            var table = this.GetStorageTable();

            var retrievedUserSegment = await UserTableStorage.FindUserByNickname(table, userBll.Nickname);

            var firstUser = retrievedUserSegment.Results.FirstOrDefault();

            if (firstUser == null)
            {
                throw new StorageException("The user is not in the database");
            }

            var deleteOperation = TableOperation.Delete(firstUser);

            await table.ExecuteAsync(deleteOperation);
        }

        public async Task DeleteAllUsers()
        {
            var table = this.GetStorageTable();

            var query = new TableQuery<UserEntity>();

            var usersSegment = await table.ExecuteQuerySegmentedAsync(query, null);

            Parallel.ForEach(usersSegment.Results, async user =>
            {
                await table.ExecuteAsync(TableOperation.Delete(user));
            });
        }

        #region Helper methods

        private CloudTable GetStorageTable()
        {
            var tableClient = this.storageAccount.CreateCloudTableClient();

            return tableClient.GetTableReference(TableName);
        }

        private static Task<TableQuerySegment<UserEntity>> FindUserByNickname(CloudTable table, string nickname)
        {
            var query = new TableQuery<UserEntity>()
                .Where(TableQuery.GenerateFilterCondition(
                    "RowKey",
                    QueryComparisons.Equal,
                    nickname));

            return table.ExecuteQuerySegmentedAsync(query, null);
        }

        #endregion
    }
}
