// ****************************************************************************
// <copyright file="UserDocumentDBStorage.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace Storage.DocumentDBStorage
{
    using System;
    using System.Configuration;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using Microsoft.Azure.Documents.Linq;

    using Model.EntitiesBll;
    using Storage.DocumentDBRepository.Entities;

    public class UserDocumentDBStorage : IUserStorage
    {
        private const string CollectionId = "UserStorage";

        // Configuration info
        private static readonly string Endpoint = ConfigurationManager.AppSettings["Endpoint"];
        private static readonly string AuthorizationKey = ConfigurationManager.AppSettings["AuthorizationKey"];
        private static readonly string DatabaseId = ConfigurationManager.AppSettings["DatabaseId"];

        // Singleton pattern
        private static UserDocumentDBStorage instance;

        private static Database database;
        private static Database Database
        {
            get
            {
                if (database == null)
                {
                    ReadOrCreateDatabase().Wait();
                }

                return database;
            }
        }

        private static DocumentCollection collection;
        private static DocumentCollection Collection
        {
            get
            {
                if (collection == null)
                {
                    ReadOrCreateCollection(Database.SelfLink).Wait();
                }

                return collection;
            }
        }

        #region Singleton pattern

        private UserDocumentDBStorage()
        {
        }

        public static UserDocumentDBStorage GetInstance()
        {
            if (UserDocumentDBStorage.instance == null)
            {
                UserDocumentDBStorage.instance = new UserDocumentDBStorage();
            }

            return UserDocumentDBStorage.instance;
        }

        #endregion

        public async Task AddUser(UserBll userBll)
        {
            Uri endpointUri = new Uri(Endpoint);

            using (var client = new DocumentClient(endpointUri, AuthorizationKey))
            {
                await client.CreateDocumentAsync(Collection.SelfLink, UserDocumentDB.FromUserBll(userBll));
            }
        }

        public async Task<UserBll> GetUser(string nickname)
        {
            Uri endpointUri = new Uri(Endpoint);

            using (var client = new DocumentClient(endpointUri, AuthorizationKey))
            {
                var user = await Task.Run(() =>
                    client.CreateDocumentQuery<UserDocumentDB>(Collection.DocumentsLink)
                        .Where(d => d.ID == nickname)
                        .AsEnumerable()
                        .FirstOrDefault());

                return UserDocumentDB.ToUserBll(user);
            }
        }

        public async Task<bool> ContainsUser(UserBll userBll)
        {
            Uri endpointUri = new Uri(Endpoint);

            using (var client = new DocumentClient(endpointUri, AuthorizationKey))
            {
                return await Task.Run(() =>
                    client.CreateDocumentQuery<UserDocumentDB>(Collection.DocumentsLink)
                        .Where(d => d.ID == UserDocumentDB.FromUserBll(userBll).ID)
                        .AsEnumerable()
                        .Any());
            }
        }

        public async Task UpdateUser(UserBll userBll)
        {
            Uri endpointUri = new Uri(Endpoint);

            using (var client = new DocumentClient(endpointUri, AuthorizationKey))
            {
                var newUser = UserDocumentDB.FromUserBll(userBll);

                var currentUserDocument = client.CreateDocumentQuery<Document>(Collection.DocumentsLink)
                    .Where(d => d.Id == newUser.ID)
                    .AsEnumerable().FirstOrDefault();

                if (currentUserDocument != null)
                {
                    await client.ReplaceDocumentAsync(currentUserDocument.SelfLink, newUser);
                }
            }
        }

        public async Task DeleteUser(UserBll userBll)
        {
            Uri endpointUri = new Uri(Endpoint);

            using (var client = new DocumentClient(endpointUri, AuthorizationKey))
            {
                var userToDelete = UserDocumentDB.FromUserBll(userBll);

                var currentUserDocument = client.CreateDocumentQuery<Document>(Collection.DocumentsLink)
                    .Where(d => d.Id == userToDelete.ID)
                    .AsEnumerable().FirstOrDefault();

                if (currentUserDocument != null)
                {
                    await client.DeleteDatabaseAsync(currentUserDocument.SelfLink);
                }
            }
        }

        public async Task DeleteStorage()
        {
            Uri endpointUri = new Uri(Endpoint);

            using (var client = new DocumentClient(endpointUri, AuthorizationKey))
            {
                if (database != null)
                {
                    await client.DeleteDatabaseAsync(database.SelfLink);
                }
            }
        }

        #region Helper methods

        public static async Task ReadOrCreateCollection(string databaseLink)
        {
            Uri endpointUri = new Uri(Endpoint);

            using (var client = new DocumentClient(endpointUri, AuthorizationKey))
            {
                var collections = client.CreateDocumentCollectionQuery(databaseLink)
                              .Where(col => col.Id == CollectionId).ToArray();

                if (collections.Any())
                {
                    collection = collections.First();
                }
                else
                {
                    collection = await client.CreateDocumentCollectionAsync(databaseLink,
                        new DocumentCollection { Id = CollectionId });
                }
            }
        }

        public static async Task ReadOrCreateDatabase()
        {
            Uri endpointUri = new Uri(Endpoint);

            using (var client = new DocumentClient(endpointUri, AuthorizationKey))
            {
                var databases = client.CreateDatabaseQuery()
                            .Where(db => db.Id == DatabaseId).ToArray();

                if (databases.Any())
                {
                    database = databases.First();
                }
                else
                {
                    database = new Database { Id = DatabaseId };
                    database = await client.CreateDatabaseAsync(database);
                }
            }
        }

        #endregion
    }
}
