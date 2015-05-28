// ****************************************************************************
// <copyright file="ImageHelper.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// ImageHelper tests
// </summary>
// ****************************************************************************

namespace GoingOn.Common.Tests
{
    using System;
    using System.Drawing;
    using System.IO;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ImageHelperTests
    {
        [TestMethod]
        public void TestCreateFromStream()
        {
            Image expectedImage = Image.FromFile("goten.png");
            Stream stream = new MemoryStream();
            expectedImage.Save(stream, expectedImage.RawFormat);

            Image createdImage = ImageHelper.CreateFromStream(stream);

            Assert.AreEqual(expectedImage.Size, createdImage.Size);
        }

        [TestMethod]
        public void TestSaveToStream()
        {
            Image expectedImage = Image.FromFile("goten.png");

            Stream stream = new MemoryStream();

            ImageHelper.SaveToStream(expectedImage, stream);
            Image actualImage = ImageHelper.CreateFromStream(stream);

            Assert.AreEqual(expectedImage.Size, actualImage.Size);
        }

        [TestMethod]
        public void TestSaveThumbnailToStream()
        {
            Image image = Image.FromFile("goten.png");

            Stream thumbnailStream = new MemoryStream();

            ImageHelper.SaveThumbnailToSteam(image, thumbnailStream, image.Width / 4, image.Height / 4);
            Image thumbnail = ImageHelper.CreateFromStream(thumbnailStream);

            Assert.AreEqual(image.GetThumbnailImage(image.Width / 4, image.Height / 4, null, IntPtr.Zero).Size, thumbnail.Size);
        }
    }
}
