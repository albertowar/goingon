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
    using System;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;

    using GoingOn.Model.EntitiesBll;
    using GoingOn.Storage;
    using GoingOn.Storage.TableStorage.Entities;

    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;

    public class UserTableStorage : IUserStorage
    {
        /**
         * Most of the operations (if not all) are not using the PartitionKey to query the database.
         * Since the nickname is unique per user throughout the system, we need query using this value instead.
         * However, I will keep the PartitionKey as it is cause it might be used in future queries.
         **/

        // Configuration info
        private readonly string tableName;
        private readonly CloudStorageAccount storageAccount;

        public UserTableStorage(string connectionString, string tableName)
        {
            this.tableName = tableName;

            try
            {
                this.storageAccount = CloudStorageAccount.Parse(connectionString);
            }
            catch (Exception e)
            {
                throw new AzureTableStorageException(string.Format("The storage account could not be created. Erro: {0}", e.Message));
            }
        }

        public async Task AddUser(UserBll userBll)
        {
            CloudTable table = this.GetStorageTable();

            await table.ExecuteAsync(TableOperation.Insert(UserEntity.FromUserBll(userBll)));
        }

        public async Task<UserBll> GetUser(string nickname)
        {
            var table = this.GetStorageTable();

            var retrievedUserSegment = await UserTableStorage.FindUserByNickname(table, nickname);

            var firstUser = retrievedUserSegment.Results.FirstOrDefault();

            if (firstUser == null)
            {
                throw new AzureTableStorageException("The user is not in the database");
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
                throw new AzureTableStorageException("The user is not in the database");
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
                throw new AzureTableStorageException("The user is not in the database");
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

            return tableClient.GetTableReference(this.tableName);
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
