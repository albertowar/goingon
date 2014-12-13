// ****************************************************************************
// <copyright file="News.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace GoingOn.Entities
{
    public class News
    {
        public string Content { get; private set; }

        public News(string content)
        {
            this.Content = content;
        }
    }
}