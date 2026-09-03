using System;
using System.Collections.Generic;
using System.Linq;

namespace Calabonga.PagedListCore;

/// <summary>
/// Represents the default implementation of the <see cref="IPagedList{T}"/> interface.
/// Page numbers are 1-based: the first page is <c>1</c>.
/// </summary>
/// <typeparam name="T">The type of the data to page</typeparam>
public class PagedList<T> : IPagedList<T>
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
    /// Gets the items of the current page.
    /// </summary>
    public IList<T> Items { get; }

    /// <summary>
    /// Gets a value indicating whether a page exists before the current one.
    /// </summary>
    public bool HasPreviousPage => PageIndex > 1;

    /// <summary>
    /// Gets a value indicating whether a page exists after the current one.
    /// </summary>
    public bool HasNextPage => PageIndex < TotalPages;

    /// <summary>
    /// Initializes a new instance of the <see cref="PagedList{T}" /> class from a full,
    /// not-yet-paged source. The requested page is sliced out with <c>Skip</c>/<c>Take</c>.
    /// </summary>
    /// <param name="source">The full source collection.</param>
    /// <param name="pageIndex">The 1-based page number to take.</param>
    /// <param name="pageSize">The size of the page.</param>
    /// <param name="totalCount">
    /// Total items in the collection. When <c>null</c>, the count is taken from <paramref name="source"/>.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="pageIndex"/> or <paramref name="pageSize"/> is less than 1.
    /// </exception>
    internal PagedList(IEnumerable<T> source, int pageIndex, int pageSize, int? totalCount = null)
    {
        PagedListHelper.EnsureValidArguments(pageIndex, pageSize);

        PageIndex = pageIndex;
        PageSize = pageSize;

        var skip = PagedListHelper.GetSkipCount(pageIndex, pageSize);

        if (source is IQueryable<T> queryable)
        {
            TotalCount = totalCount ?? queryable.Count();
            Items = queryable.Skip(skip).Take(pageSize).ToList();
        }
        else
        {
            var enumerable = source.ToList();
            TotalCount = totalCount ?? enumerable.Count;
            Items = enumerable.Skip(skip).Take(pageSize).ToList();
        }

        TotalPages = PagedListHelper.GetTotalPages(TotalCount, pageSize);
    }

    /// <summary>
    /// Initializes a new empty instance of the <see cref="PagedList{T}" /> class.
    /// </summary>
    internal PagedList()
    {
        PageIndex = 1;
        PageSize = 0;
        TotalCount = 0;
        TotalPages = 0;
        Items = Array.Empty<T>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PagedList{T}" /> class from an already-paged
    /// slice: <paramref name="source"/> is stored as-is and only the metadata is computed.
    /// </summary>
    /// <param name="source">The items of the current page (already sliced by the caller).</param>
    /// <param name="pageIndex">The 1-based page number of the slice.</param>
    /// <param name="pageSize">The size of the page.</param>
    /// <param name="count">The total number of items across all pages.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="pageIndex"/> or <paramref name="pageSize"/> is less than 1, or <paramref name="count"/> is negative.
    /// </exception>
    public PagedList(
        IEnumerable<T> source,
        int pageIndex,
        int pageSize,
        int count)
    {
        PagedListHelper.EnsureValidArguments(pageIndex, pageSize);

        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count), count, "Total count cannot be negative.");
        }

        PageIndex = pageIndex;
        PageSize = pageSize;
        TotalCount = count;
        TotalPages = PagedListHelper.GetTotalPages(count, pageSize);
        Items = source.ToList();
    }
}
