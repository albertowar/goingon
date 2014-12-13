// ****************************************************************************
// <copyright file="NewsControllerTest.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace GoingOn.Tests.Controllers
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using GoingOn.Controllers;

    [TestClass]
    public class NewsControllerTest
    {
        [TestMethod]
        public void Get()
        {
            // Arrange
            NewsController controller = new NewsController();

            // Act
            //IEnumerable<string> result = controller.Get();

            // Assert
            //Assert.IsNotNull(result);
            //Assert.AreEqual(2, result.Count());
            //Assert.AreEqual("value1", result.ElementAt(0));
            //Assert.AreEqual("value2", result.ElementAt(1));
        }

        [TestMethod]
        public void GetById()
        {
            // Arrange
            NewsController controller = new NewsController();

            // Act
            //string result = controller.Get(5);

            // Assert
            //Assert.AreEqual("value", result);
        }

        [TestMethod]
        public void Post()
        {
            // Arrange
            NewsController controller = new NewsController();

            // Act
            //controller.Post("value");

            // Assert
        }

        [TestMethod]
        public void Put()
        {
            // Arrange
            NewsController controller = new NewsController();

            // Act
            //controller.Put(5, "value");

            // Assert
        }

        [TestMethod]
        public void Delete()
        {
            // Arrange
            NewsController controller = new NewsController();

            // Act
            //controller.Delete(5);

            // Assert
        }
    }
}
