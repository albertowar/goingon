// ****************************************************************************
// <copyright file="UserTests.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

using System.Runtime.Serialization;

namespace Client.Entities
{
    [DataContract]
    public class User
    {
        [DataMember]
        public string Nickname { get; set; }

        [DataMember]
        public string Password { get; set; }
    }
}
