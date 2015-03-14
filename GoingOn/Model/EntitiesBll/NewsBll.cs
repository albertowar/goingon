// ****************************************************************************
// <copyright file="NewsBll.cs" company="Universidad de Malaga">
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

    public class NewsBll
    {
        /*
         * Id. Mandatory and unique.
         */
        public Guid Id { get; set; } 

        /*
         * The title of the news. Mandatory.
         */
        public string Title { get; set; }

        /*
         * The content of the news. Mandatory.
         */
        public string Content { get; set; }

        /*
         * The city of the news. Mandatory.
         */
        public string City { get; set; }

        /*
         * The author of the news. Mandatory.
         */
        public string Author { get; set; }

        /*
         * The creation date of the news. Mandatory.
         */
        public DateTime Date { get; set; }
    }
}
