// ****************************************************************************
// <copyright file="UserBll.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace Model.EntitiesBll
{
    using System;

    public class UserBll
    {
        public string Nickname { get; set; }

        public string Password { get; set; }

        public string City { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public DateTime? BirthDate { get; set; }

        public DateTime RegistrationDate { get; set; }
    }
}