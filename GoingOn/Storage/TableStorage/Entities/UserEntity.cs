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
    using System;

    using Microsoft.WindowsAzure.Storage.Table;
    using Model.EntitiesBll;

    public class UserEntity : TableEntity
    {
        public string Password { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public DateTime? BirthDate { get; set; }

        public DateTime RegistrationDate { get; set; }

        public static UserEntity FromUserBll(UserBll userBll)
        {
            return new UserEntity
            {
                PartitionKey = userBll.City,
                RowKey = userBll.Nickname,
                Password = userBll.Password,
                Name = userBll.Name,
                Email = userBll.Email,
                RegistrationDate = userBll.RegistrationDate
            };
        }

        public static UserBll ToUserBll(UserEntity userEntity)
        {
            return new UserBll
            {
                Nickname = userEntity.RowKey,
                Password = userEntity.Password,
                City = userEntity.PartitionKey,
                Name = userEntity.Name,
                Email = userEntity.Email,
                BirthDate = userEntity.BirthDate,
                RegistrationDate = userEntity.RegistrationDate
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
                Password.GetHashCode() ^
                Name.GetHashCode() ^
                Email.GetHashCode() ^
                BirthDate.GetHashCode() ^
                RegistrationDate.GetHashCode();
        }

        public void Merge(UserEntity userEntity)
        {
            if (this.Equals(userEntity))
            {
                if (!string.IsNullOrWhiteSpace(userEntity.PartitionKey))
                {
                    this.PartitionKey = userEntity.PartitionKey;
                }

                if (!string.IsNullOrWhiteSpace(userEntity.Password))
                {
                    this.Password = userEntity.Password;
                }

                if (!string.IsNullOrWhiteSpace(userEntity.Name))
                {
                    this.Name = userEntity.Name;
                }

                if (!string.IsNullOrWhiteSpace(userEntity.Email))
                {
                    this.Email = userEntity.Email;
                }

                if (userEntity.BirthDate != null)
                {
                    this.BirthDate = userEntity.BirthDate;
                }
            }
        }
    }
}
