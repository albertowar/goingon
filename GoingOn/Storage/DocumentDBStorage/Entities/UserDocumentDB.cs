// ****************************************************************************
// <copyright file="UserDocumentDB.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace Storage.DocumentDBRepository.Entities
{
    using Model.EntitiesBll;
    using Newtonsoft.Json;

    public class UserDocumentDB
    {
        [JsonProperty(PropertyName="id")]
        public string ID { get; private set; }

        [JsonProperty(PropertyName = "password")]
        public string Password { get; private set; }

        [JsonConstructor]
        public UserDocumentDB(string id)
        {
            this.ID = id;
            this.Password = string.Empty;
        }

        [JsonConstructor]
        public UserDocumentDB(string id, string password)
        {
            this.ID = id;
            this.Password = password;
        }

        public static UserDocumentDB FromUserBll(UserBll userBll)
        {
            return new UserDocumentDB(userBll.Nickname, userBll.Password);
        }

        public static UserBll ToUserBll(UserDocumentDB userMemory)
        {
            return new UserBll
            {
                Nickname = userMemory.ID,
                Password = userMemory.Password
            };
        }

        public override bool Equals(object anotherUserObject)
        {
            UserDocumentDB anotherUser = anotherUserObject as UserDocumentDB;

            return
                anotherUser != null &&
                string.Equals(this.ID, anotherUser.ID);
        }

        public override int GetHashCode()
        {
            return
                ID.GetHashCode() ^
                Password.GetHashCode();
        }

        public void Merge(UserDocumentDB userMemory)
        {
            if (this.Equals(userMemory))
            {
                if (!string.IsNullOrWhiteSpace(userMemory.Password))
                {
                    this.Password = userMemory.Password;
                }
            }
        }
    }
}
