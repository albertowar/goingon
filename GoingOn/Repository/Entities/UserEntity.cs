// ****************************************************************************
// <copyright file="UserEntity.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// Encapsulates the user information that will be stored
// </summary>
// ****************************************************************************

namespace GoingOn.Repository.Entities
{
    using System;
    using GoingOn.Model.EntitiesBll;
    using Microsoft.WindowsAzure.Storage.Table;

    public class UserEntity : TableEntity
    {
        /// <summary>
        /// The password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// The email of the user
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// The date of birth
        /// </summary>
        public DateTime? BirthDate { get; set; }

        /// <summary>
        /// The registration date
        /// </summary>
        public DateTime RegistrationDate { get; set; }

        // TODO: missing Name (real name)

        public static UserEntity FromUserBll(UserBll userBll)
        {
            return new UserEntity
            {
                PartitionKey = userBll.City,
                RowKey = userBll.Nickname,
                Password = userBll.Password,
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
                this.Email.GetHashCode() ^
                this.BirthDate.GetHashCode() ^
                this.RegistrationDate.GetHashCode();
        }

        /// <summary>
        /// This method combines two user entities with the same RowKey (the same name).
        /// It does not allow to update PartitionKey or RowKey. Therefore,
        /// to update the city, it requires to delete and create the user again.
        /// </summary>
        /// <param name="userEntity"></param>
        public void Merge(UserEntity userEntity)
        {
            if (this.Equals(userEntity))
            {
                if (!string.IsNullOrWhiteSpace(userEntity.Password))
                {
                    this.Password = userEntity.Password;
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
