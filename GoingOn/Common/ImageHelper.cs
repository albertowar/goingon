// ****************************************************************************
// <copyright file="ImageHelper.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// Helper class to deal with images
// </summary>
// ****************************************************************************

namespace GoingOn.Common
{
    using System;
    using System.Drawing;
    using System.IO;

    public class ImageHelper 
    {
        public static Image CreateFromStream(Stream stream)
        {
            return Image.FromStream(stream);
        }

        public static void SaveToStream(Image image, Stream stream)
        {
            image.Save(stream, image.RawFormat);
            stream.Position = 0;
        }

        public static void SaveThumbnailToSteam(Image image, Stream stream, int width, int height)
        {
            Image thumbnail = image.GetThumbnailImage(width, height, () => false, IntPtr.Zero);
            thumbnail.Save(stream, image.RawFormat);
            stream.Position = 0;
        }
    }
}
