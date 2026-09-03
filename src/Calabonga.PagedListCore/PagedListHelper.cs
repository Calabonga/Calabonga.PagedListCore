using System;

namespace Calabonga.PagedListCore;

/// <summary>
/// Shared arithmetic for paged list construction. Page numbers are 1-based.
/// </summary>
internal static class PagedListHelper
{
    /// <summary>
    /// Validates the paging arguments common to every computed <see cref="IPagedList{T}"/>.
    /// </summary>
    /// <param name="pageIndex">The 1-based page number.</param>
    /// <param name="pageSize">The page size.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="pageIndex"/> is less than 1, or <paramref name="pageSize"/> is less than 1.
    /// </exception>
    internal static void EnsureValidArguments(int pageIndex, int pageSize)
    {
        if (pageIndex < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(pageIndex), pageIndex, "Page index must be 1 or greater.");
        }

        if (pageSize < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(pageSize), pageSize, "Page size must be 1 or greater.");
        }
    }

    /// <summary>
    /// Number of items to skip to reach the first item of <paramref name="pageIndex"/>.
    /// </summary>
    internal static int GetSkipCount(int pageIndex, int pageSize) => (pageIndex - 1) * pageSize;

    /// <summary>
    /// Total number of pages for the given totals. Returns 0 when there are no items.
    /// </summary>
    internal static int GetTotalPages(int totalCount, int pageSize)
        => totalCount <= 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);
}
