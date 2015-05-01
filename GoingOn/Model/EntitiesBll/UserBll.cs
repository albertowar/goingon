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
        /// 
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? BirthDate { get; set; }

        /**
         * The registration date of the user. Mandatory.
         */
        public DateTime RegistrationDate { get; set; }
    }
}