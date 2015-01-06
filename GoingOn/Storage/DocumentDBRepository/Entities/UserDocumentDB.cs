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
        [JsonProperty(PropertyName="nickname")]
        public string Nickname { get; private set; }

        [JsonProperty(PropertyName = "password")]
        public string Password { get; private set; }

        public UserDocumentDB(string nickname)
        {
            this.Nickname = nickname;
            this.Password = string.Empty;
        }

        public UserDocumentDB(string nickname, string password)
        {
            this.Nickname = nickname;
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
                Nickname = userMemory.Nickname,
                Password = userMemory.Password
            };
        }

        public override bool Equals(object anotherUserObject)
        {
            UserDocumentDB anotherUser = anotherUserObject as UserDocumentDB;

            return
                anotherUser != null &&
                string.Equals(this.Nickname, anotherUser.Nickname);
        }

        public override int GetHashCode()
        {
            return
                Nickname.GetHashCode() ^
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
