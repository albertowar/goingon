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
        public string Title { get; private set; }
        public string Content { get; private set; }
        public UserBll Author { get; private set; }
        public TimeSpan Date { get; private set; }
        public int Rating { get; private set; }

        public NewsBll(string title, string content, UserBll author, TimeSpan date, int rating)
        {
            Title = title;
            Content = content;
            Author = author;
            Date = date;
            Rating = rating;
        }
    }
}
