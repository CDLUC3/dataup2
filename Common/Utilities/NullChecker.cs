﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.Research.DataOnboarding.Utilities
{
    /// <summary>
    /// Class to check all the properties for null check.
    /// </summary>
    /// <typeparam name="T">Generic type.</typeparam>
    public static class NullChecker<T> where T : class
    {
        /// <summary>
        /// Checkers list.
        /// </summary>
        private static readonly List<Func<T, bool>> Checkers;

        /// <summary>
        /// List of names variable.
        /// </summary>
        private static readonly List<string> Names;

        /// <summary>
        /// Initializes static members of the <see cref="NullChecker" /> class.
        /// </summary>
        static NullChecker()
        {
            Checkers = new List<Func<T, bool>>();
            Names = new List<string>();

            try
            {
                // We can't rely on the order of the properties, but we 
                // can rely on the order of the constructor parameters 
                // in an anonymous type - and that there'll only be 
                // one constructor. 
                foreach (string name in typeof(T).GetConstructors()[0]
                                                 .GetParameters()
                                                 .Select(p => p.Name))
                {
                    Names.Add(name);
                    PropertyInfo property = typeof(T).GetProperty(name);
                    ParameterExpression param = Expression.Parameter(typeof(T), "container");
                    Expression propertyAccess = Expression.Property(param, property);
                    Expression nullValue = Expression.Constant(null, property.PropertyType);
                    Expression equality = Expression.Equal(propertyAccess, nullValue);
                    var lambda = Expression.Lambda<Func<T, bool>>(equality, param);
                    Checkers.Add(lambda.Compile());
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Method to check for null.
        /// </summary>
        /// <param name="item">Item to check.</param>
        internal static void Check(T item)
        {
            for (int i = 0; i < Checkers.Count; i++)
            {
                if (Checkers[i](item))
                {
                    throw new ArgumentNullException(Names[i]);
                }
            }
        }
    }
}
