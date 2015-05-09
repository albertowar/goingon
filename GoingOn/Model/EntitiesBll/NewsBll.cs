// ****************************************************************************
// <copyright file="NewsBll.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// News business logic
// </summary>
// ****************************************************************************

namespace GoingOn.Model.EntitiesBll
{
    using System;

    public class NewsBll
    {
        /// <summary>
        /// The id of the news
        /// </summary>
        public Guid Id { get; set; } 

        /// <summary>
        /// The title of the news
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The content of the news
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// The city of the news
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// The author of the news
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// The date that the news ocurred
        /// </summary>
        public DateTime Date { get; set; }
    }
}
