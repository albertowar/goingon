// ****************************************************************************
// <copyright file="NewsBll.cs" company="Universidad de Malaga">
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

    public class NewsBll
    {
        public Guid Id { get; set; } 
        public string Title { get; set; }
        public string Content { get; set; }
        public string Author { get; set; }
        public DateTime Date { get; set; }
        public int Rating { get; set; }
    }
}
