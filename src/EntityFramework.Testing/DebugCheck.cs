﻿//-----------------------------------------------------------------------------------------------------
// <copyright file="DebugCheck.cs" company="Microsoft Open Technologies, Inc">
//   Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
//   Modified by Scott Xu to be compliance with StyleCop.
// </copyright>
//-----------------------------------------------------------------------------------------------------

namespace EntityFramework.Testing
{
    using System.Diagnostics;

    /// <summary>
    /// Assert value in debug mode
    /// </summary>
    internal class DebugCheck
    {
        /// <summary>
        /// Assert the value is not null.
        /// </summary>
        /// <typeparam name="T">The reference type. </typeparam>
        /// <param name="value">The value. </param>
        [Conditional("DEBUG")]
        public static void NotNull<T>(T value) where T : class
        {
            Debug.Assert(value != null, "The value should not be null.");
        }

        /// <summary>
        /// Assert the value is not null.
        /// </summary>
        /// <typeparam name="T">The value type. </typeparam>
        /// <param name="value">The value. </param>
        [Conditional("DEBUG")]
        public static void NotNull<T>(T? value) where T : struct
        {
            Debug.Assert(value != null, "The value should not be null.");
        }

        /// <summary>
        /// Assert the string is not null nor empty.
        /// </summary>
        /// <param name="value">The value. </param>
        [Conditional("DEBUG")]
        public static void NotEmpty(string value)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(value), "The string should not be null nor empty.");
        }
    }
}
