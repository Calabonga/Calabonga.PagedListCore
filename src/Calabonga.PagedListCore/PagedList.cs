using System;
using System.Collections.Generic;

namespace Calabonga.PagedListCore
{
    /// <summary>
    /// Provides some help methods for <see cref="IPagedList{T}"/> interface.
    /// </summary>
    public static class PagedList
    {
        /// <summary>
        /// Creates an empty of <see cref="IPagedList{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type for paging </typeparam>
        /// <returns>An empty instance of <see cref="IPagedList{T}"/>.</returns>
        public static IPagedList<T> Empty<T>() => new PagedList<T>();

        /// <summary>
        /// Creates a new instance of <see cref="IPagedList{TResult}"/> from source of <see cref="IPagedList{TSource}"/> instance.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="converter">The converter.</param>
        /// <returns>An instance of <see cref="IPagedList{TResult}"/>.</returns>
        public static IPagedList<TResult> From<TSource, TResult>(
            IPagedList<TResult> source,
            Func<IList<TResult>, IEnumerable<TResult>> converter) =>
            new PagedList<TSource, TResult>(source, converter);

        /// <summary>
        /// Creates a new instance of <see cref="IPagedList{TResult}"/> from source of <see cref="IPagedList{TSource}"/> instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="indexFrom"></param>
        public static IPagedList<T> Create<T>(IEnumerable<T> source, int pageIndex, int pageSize, int indexFrom)
        {
            return new PagedList<T>(source, pageIndex, pageSize, indexFrom);
        }

        /// <summary>
        /// Creates a new instance of <see cref="IPagedList{TResult}"/> from source of <see cref="IPagedList{TSource}"/> instance.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="converter"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="indexFrom"></param>
        public static IPagedList<TResult> Create<TSource, TResult>(
            IEnumerable<TResult> source,
            Func<IEnumerable<TResult>, IEnumerable<TResult>> converter,
            int pageIndex,
            int pageSize,
            int indexFrom = 0)
        {
            return new PagedList<TResult>(source, converter, pageIndex, pageSize, indexFrom, indexFrom);
        }
    }
}

