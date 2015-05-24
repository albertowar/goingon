// ****************************************************************************
// <copyright file="MediaTypeHelper.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// News Storage base interface
// </summary>
// ****************************************************************************

namespace GoingOn.Frontend.Common
{
    using System;
    using System.Drawing.Imaging;
    using System.Net.Http.Headers;

    public class MediaTypeHelper
    {
        public static MediaTypeHeaderValue ConvertFromImageFormat(ImageFormat imageFormat)
        {
            if (imageFormat.Equals(ImageFormat.Png))
            {
                return new MediaTypeHeaderValue("image/png");
            }

            throw new ArgumentException("Unsupported ImageFormat.");
        }

        public static ImageFormat ConvertToImageFormat(MediaTypeHeaderValue mediaTypeHeader)
        {
            if (string.Equals("image/png", mediaTypeHeader.MediaType))
            {
                return ImageFormat.Png;
            }

            throw new ArgumentException("Unsupported ContentType.");
        }
    }
}
