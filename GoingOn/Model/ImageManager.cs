// ****************************************************************************
// <copyright file="IImageManager.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// News Storage base interface
// </summary>
// ****************************************************************************

namespace GoingOn.Model
{
    using System;
    using System.Drawing;
    using System.IO;

    public class ImageManager : IImageManager
    {
        public Image CreateFromStream(Stream stream)
        {
            return Image.FromStream(stream);
        }

        public void SaveToSteam(Image image, Stream stream)
        {
            image.Save(stream, image.RawFormat);
            stream.Position = 0;
        }

        public void SaveThumbnailToSteam(Image image, Stream stream, int width, int height)
        {
            Image thumbnail = image.GetThumbnailImage(40, 40, () => false, IntPtr.Zero);
            thumbnail.Save(stream, thumbnail.RawFormat);
            stream.Position = 0;
        }
    }
}
