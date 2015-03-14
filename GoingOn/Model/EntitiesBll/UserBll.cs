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
        /**
         * The nickname of the user. Mandatory and unique.
         */
        public string Nickname { get; set; }

        /**
         * The password of the user. Mandatory.
         */
        public string Password { get; set; }

        /**
         * The city where the user belongs to. Mandatory.
         */
        public string City { get; set; }

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
    }
}