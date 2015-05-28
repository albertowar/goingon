// ****************************************************************************
// <copyright file="MediaTypeHelperTests.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// MediaTypeHelper tests
// </summary>
// ****************************************************************************

namespace GoingOn.Frontend.Tests.Common
{
    using System;
    using System.Drawing.Imaging;
    using System.Net.Http.Headers;
    using GoingOn.Common.Tests;
    using GoingOn.Frontend.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class MediaTypeHelperTests
    {
        [TestMethod]
        public void TestConvertFromImageFormat()
        {
            Assert.AreEqual(new MediaTypeHeaderValue("image/png"), MediaTypeHelper.ConvertFromImageFormat(ImageFormat.Png));
        }

        [TestMethod]
        public void TestConvertFromImageFormatThrowsException_IfUnsupportedFortmat()
        {

            AssertExtensions.Throws<ArgumentException>(() => MediaTypeHelper.ConvertFromImageFormat(ImageFormat.Bmp));
            AssertExtensions.Throws<ArgumentException>(() => MediaTypeHelper.ConvertFromImageFormat(ImageFormat.Emf));
            AssertExtensions.Throws<ArgumentException>(() => MediaTypeHelper.ConvertFromImageFormat(ImageFormat.Exif));
            AssertExtensions.Throws<ArgumentException>(() => MediaTypeHelper.ConvertFromImageFormat(ImageFormat.Gif));
            AssertExtensions.Throws<ArgumentException>(() => MediaTypeHelper.ConvertFromImageFormat(ImageFormat.Icon));
            AssertExtensions.Throws<ArgumentException>(() => MediaTypeHelper.ConvertFromImageFormat(ImageFormat.Jpeg));
            AssertExtensions.Throws<ArgumentException>(() => MediaTypeHelper.ConvertFromImageFormat(ImageFormat.MemoryBmp));
            AssertExtensions.Throws<ArgumentException>(() => MediaTypeHelper.ConvertFromImageFormat(ImageFormat.Tiff));
            AssertExtensions.Throws<ArgumentException>(() => MediaTypeHelper.ConvertFromImageFormat(ImageFormat.Wmf));
        }

        [TestMethod]
        public void TestConvertToImageFormat()
        {
            Assert.AreEqual(ImageFormat.Png, MediaTypeHelper.ConvertToImageFormat(new MediaTypeHeaderValue("image/png")));
        }

        [TestMethod]
        public void TestConvertToImageFormatThrowsException_IfUnsupportedFortmat()
        {
            AssertExtensions.Throws<ArgumentException>(() => MediaTypeHelper.ConvertToImageFormat(new MediaTypeHeaderValue("image/bmp")));
            AssertExtensions.Throws<ArgumentException>(() => MediaTypeHelper.ConvertToImageFormat(new MediaTypeHeaderValue("image/emf")));
            AssertExtensions.Throws<ArgumentException>(() => MediaTypeHelper.ConvertToImageFormat(new MediaTypeHeaderValue("image/exif")));
            AssertExtensions.Throws<ArgumentException>(() => MediaTypeHelper.ConvertToImageFormat(new MediaTypeHeaderValue("image/gif")));
            AssertExtensions.Throws<ArgumentException>(() => MediaTypeHelper.ConvertToImageFormat(new MediaTypeHeaderValue("image/icon")));
            AssertExtensions.Throws<ArgumentException>(() => MediaTypeHelper.ConvertToImageFormat(new MediaTypeHeaderValue("image/jpeg")));
            AssertExtensions.Throws<ArgumentException>(() => MediaTypeHelper.ConvertToImageFormat(new MediaTypeHeaderValue("image/memorybmp")));
            AssertExtensions.Throws<ArgumentException>(() => MediaTypeHelper.ConvertToImageFormat(new MediaTypeHeaderValue("image/tiff")));
            AssertExtensions.Throws<ArgumentException>(() => MediaTypeHelper.ConvertToImageFormat(new MediaTypeHeaderValue("image/wmf")));
        }
    }
}
