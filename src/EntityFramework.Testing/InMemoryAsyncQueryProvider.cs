﻿//-----------------------------------------------------------------------------------------------------
// <copyright file="InMemoryAsyncQueryProvider.cs" company="Microsoft Open Technologies, Inc">
//   Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
//   Modified by Scott Xu to be compliance with StyleCop.
// </copyright>
//-----------------------------------------------------------------------------------------------------

namespace EntityFramework.Testing
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
#if !NET40
    using System.Data.Entity.Infrastructure;
#endif
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
#if !NET40
    using System.Threading;
    using System.Threading.Tasks;
#endif

    /// <summary>
    /// Represents in-memory async query provider
    /// </summary>
#if !NET40
    public class InMemoryAsyncQueryProvider : IQueryProvider, IDbAsyncQueryProvider
#else
    public class InMemoryAsyncQueryProvider : IQueryProvider
#endif
    {
        /// <summary>
        /// The method to create query.
        /// </summary>
        private static readonly MethodInfo CreateQueryMethod
            = typeof(InMemoryAsyncQueryProvider).GetDeclaredMethods().Single(m => m.IsGenericMethodDefinition && m.Name == "CreateQuery");

        /// <summary>
        /// The method to execute the query.
        /// </summary>
        private static readonly MethodInfo ExecuteMethod
            = typeof(InMemoryAsyncQueryProvider).GetDeclaredMethods().Single(m => m.IsGenericMethodDefinition && m.Name == "Execute");

        /// <summary>
        /// The query provider.
        /// </summary>
        private readonly IQueryProvider provider;

        /// <summary>
        /// The include action.
        /// </summary>
        private readonly Action<string, IEnumerable> include;

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryAsyncQueryProvider"/> class.
        /// </summary>
        /// <param name="provider">The query provider.</param>
        /// <param name="include">The Include action.</param>
        public InMemoryAsyncQueryProvider(IQueryProvider provider, Action<string, IEnumerable> include = null)
        {
            this.provider = provider;
            this.include = include;
        }

        /// <summary>
        /// Create query-able object.
        /// </summary>
        /// <param name="expression">The expression tree.</param>
        /// <returns>The query-able object.</returns>
        public IQueryable CreateQuery(Expression expression)
        {
            return (IQueryable)CreateQueryMethod
                .MakeGenericMethod(TryGetElementType(expression.Type))
                .Invoke(this, new object[] { expression });
        }

        /// <summary>
        /// Create generic query-able object.
        /// </summary>
        /// <typeparam name="TElement">The element.</typeparam>
        /// <param name="expression">The expression tree.</param>
        /// <returns>The generic query-able object.</returns>
        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new InMemoryAsyncQueryable<TElement>(this.provider.CreateQuery<TElement>(expression), this.include);
        }

        /// <summary>
        /// Execute the query.
        /// </summary>
        /// <param name="expression">The expression tree.</param>
        /// <returns>The result.</returns>
        public object Execute(Expression expression)
        {
            return ExecuteMethod.MakeGenericMethod(expression.Type).Invoke(this, new object[] { expression });
        }

        /// <summary>
        /// Execute the query.
        /// </summary>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="expression">The expression tree.</param>
        /// <returns>The result.</returns>
        public TResult Execute<TResult>(Expression expression)
        {
            return this.provider.Execute<TResult>(expression);
        }

#if !NET40
        /// <summary>
        /// Execute the query asynchronously.
        /// </summary>
        /// <param name="expression">The expression tree.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The result task.</returns>
        public Task<object> ExecuteAsync(Expression expression, CancellationToken cancellationToken)
        {
            return Task.FromResult(this.Execute(expression));
        }

        /// <summary>
        /// Execute the query asynchronously.
        /// </summary>
        /// <typeparam name="TResult">The result type</typeparam>
        /// <param name="expression">The expression tree.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The result task.</returns>
        public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            return Task.FromResult(this.Execute<TResult>(expression));
        }
#endif
        /// <summary>
        /// Try get element type.
        /// </summary>
        /// <param name="type">The expression type.</param>
        /// <returns>The element type.</returns>
        private static Type TryGetElementType(Type type)
        {
            if (!type.IsGenericTypeDefinition())
            {
                var interfaceImpl = type.GetInterfaces()
                    .Union(new[] { type })
                    .FirstOrDefault(t => t.IsGenericType() && t.GetGenericTypeDefinition() == typeof(IEnumerable<>));

                if (interfaceImpl != null)
                {
                    return interfaceImpl.GetGenericArguments().Single();
                }
            }

            return type;
        }
    }
}
