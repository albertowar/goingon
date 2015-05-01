// ****************************************************************************
// <copyright file="UserBll.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace GoingOn.Model.EntitiesBll
{
    using System;

    public class UserBll
    {
        /// <summary>
        /// The nickname of the user
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// The password of the user
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// The city of the user
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// The name of the user
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The email of the user
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// The birthdate of the user
        /// </summary>
        public DateTime? BirthDate { get; set; }

        /// <summary>
        /// The registration date of the user
        /// </summary>
        public DateTime RegistrationDate { get; set; }
    }
}