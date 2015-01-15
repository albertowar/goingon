// ****************************************************************************
// <copyright file="UserEntity.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace Storage.TableStorage.Entities
{
    using Microsoft.WindowsAzure.Storage.Table;
    using Model.EntitiesBll;

    public class UserEntity : TableEntity
    {
        public string Password { get; set; }

        public static UserEntity FromUserBll(UserBll userBll)
        {
            return new UserEntity
            {
                PartitionKey = userBll.City,
                RowKey = userBll.Nickname,
                Password = userBll.Password
            };
        }

        public static UserBll ToUserBll(UserEntity userEntity)
        {
            return new UserBll
            {
                Nickname = userEntity.RowKey,
                Password = userEntity.Password,
                City = userEntity.PartitionKey
            };
        }

        public override bool Equals(object anotherUserObject)
        {
            var anotherUser = anotherUserObject as UserEntity;

            return
                anotherUser != null &&
                string.Equals(this.PartitionKey, anotherUser.PartitionKey) &&
                string.Equals(this.RowKey, anotherUser.RowKey);
        }

        public override int GetHashCode()
        {
            return
                PartitionKey.GetHashCode() ^
                RowKey.GetHashCode() ^ 
                Password.GetHashCode();
        }

        public void Merge(UserEntity userEntity)
        {
            if (this.Equals(userEntity))
            {
                if (!string.IsNullOrWhiteSpace(userEntity.Password))
                {
                    this.Password = userEntity.Password;
                }
            }
        }
    }
}
