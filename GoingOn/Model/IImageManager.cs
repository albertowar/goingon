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
    using System.Drawing;
    using System.IO;

    public interface IImageManager
    {
        Image CreateFromStream(Stream stream);

        void SaveToSteam(Image image, Stream stream);

        void SaveThumbnailToSteam(Image image, Stream stream, int width, int height);
    }
}
