using System;
using System.Collections.Generic;

namespace Calabonga.PagedListCore;

/// <summary>
/// An <see cref="IPagedList{TResult}"/> projection over an existing <see cref="IPagedList{TSource}"/>:
/// page metadata is copied verbatim and only the items are converted. Page numbers are 1-based.
/// </summary>
/// <typeparam name="TSource">The type of the source items.</typeparam>
/// <typeparam name="TResult">The type of the projected items.</typeparam>
internal class PagedList<TSource, TResult> : IPagedList<TResult>
{
    /// <summary>
    /// Gets the 1-based index of the current page.
    /// </summary>
    public int PageIndex { get; }

    /// <summary>
    /// Gets the size of the page.
    /// </summary>
    public int PageSize { get; }

    /// <summary>
    /// Gets the total count of items across all pages.
    /// </summary>
    public int TotalCount { get; }

    /// <summary>
    /// Gets the total number of pages.
    /// </summary>
    public int TotalPages { get; }

    /// <summary>
    /// Gets the projected items of the current page.
    /// </summary>
    public IList<TResult> Items { get; }

    /// <summary>
    /// Gets a value indicating whether a page exists before the current one.
    /// </summary>
    public bool HasPreviousPage => PageIndex > 1;

    /// <summary>
    /// Gets a value indicating whether a page exists after the current one.
    /// </summary>
    public bool HasNextPage => PageIndex < TotalPages;

    /// <summary>
    /// Initializes a new instance of the <see cref="PagedList{TSource, TResult}" /> class
    /// by projecting the items of <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The source paged list.</param>
    /// <param name="converter">Projects the current page items to <typeparamref name="TResult"/>.</param>
    public PagedList(IPagedList<TSource> source, Func<IEnumerable<TSource>, IEnumerable<TResult>> converter)
    {
        PageIndex = source.PageIndex;
        PageSize = source.PageSize;
        TotalCount = source.TotalCount;
        TotalPages = source.TotalPages;
        Items = new List<TResult>(converter(source.Items));
    }
}
