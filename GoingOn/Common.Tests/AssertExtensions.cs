// ****************************************************************************
// <copyright file="NewsMemoryStorage.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// Extension class to the assertions.
// </summary>
// ****************************************************************************

namespace GoingOn.Common.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [ExcludeFromCodeCoverage]
    public class AssertExtensions
    {
        public static void Throws<T>(Action func) where T : Exception
        {
            var wasExceptionThrown = false;

            try
            {
                func.Invoke();
            }
            catch (T)
            {
                wasExceptionThrown = true;
            }
            catch (AggregateException aggregateException)
            {
                var exception = aggregateException.InnerException;
                wasExceptionThrown = exception.GetType() == typeof(T);
            }

            if (!wasExceptionThrown)
            {
                throw new AssertFailedException(
                    String.Format("An exception of type {0} was expected, but not thrown", typeof(T))
                    );
            }
        }

        public static void Throws<T>(Action func, string message) where T : Exception
        {
            var wasExceptionThrown = false;

            try
            {
                func.Invoke();
            }
            catch (T exception)
            {
                wasExceptionThrown = string.Equals(message, exception.Message);
            }
            catch (AggregateException aggregateException)
            {
                var exception = aggregateException.GetBaseException();
                wasExceptionThrown = 
                    exception.GetType() == typeof(T) &&
                    string.Equals(message, exception.Message);
            }

            if (!wasExceptionThrown)
            {
                throw new AssertFailedException(
                    String.Format("An exception of type {0} was expected, but not thrown", typeof(T))
                    );
            }
        }
    }
}
