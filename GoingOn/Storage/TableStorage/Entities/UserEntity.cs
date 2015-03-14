// ****************************************************************************
// <copyright file="UserEntity.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace GoingOn.Storage.TableStorage.Entities
{
    using System;
    using Microsoft.WindowsAzure.Storage.Table;
    using Model.EntitiesBll;

    public class UserEntity : TableEntity
    {
        /**
         * The password of the user. Mandatory.
         */
        public string Password { get; set; }

        /**
         * The real name of the user. Optional.
         */
        public string Name { get; set; }

        /**
         * The e-mail address of the user. Optional.
         */
        public string Email { get; set; }

        /**
         * The birth date of the user. Optional.
         */
        public DateTime? BirthDate { get; set; }

        /**
         * The registration date of the user. Mandatory.
         */
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
                RegistrationDate = userBll.RegistrationDate,
                BirthDate = userBll.BirthDate
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
                string.Equals(this.RowKey, anotherUser.RowKey);
        }

        public override int GetHashCode()
        {
            return
                this.PartitionKey.GetHashCode() ^
                this.RowKey.GetHashCode() ^ 
                this.Password.GetHashCode() ^
                this.Name.GetHashCode() ^
                this.Email.GetHashCode() ^
                this.BirthDate.GetHashCode() ^
                this.RegistrationDate.GetHashCode();
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
