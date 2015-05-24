// ****************************************************************************
// <copyright file="ImageHelper.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// News Storage base interface
// </summary>
// ****************************************************************************

namespace GoingOn.Common
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;

    public class ImageHelper 
    {
        public static Image CreateFromStream(Stream stream)
        {
            return Image.FromStream(stream);
        }

        public static void SaveToSteam(Image image, Stream stream)
        {
            image.Save(stream, image.RawFormat);
            stream.Position = 0;
        }

        public static void SaveThumbnailToSteam(Image image, Stream stream, int width, int height)
        {
            Image thumbnail = image.GetThumbnailImage(40, 40, () => false, IntPtr.Zero);
            thumbnail.Save(stream, thumbnail.RawFormat);
            stream.Position = 0;
        }
    }
}
