using System;
using System.Collections.Generic;
using System.Linq;
using Calabonga.PagedListCore;
using Xunit;

namespace Calabonga.PagedListCore.Tests;

public sealed class PagedListTests
{
    private static IReadOnlyList<int> Range(int count) => Enumerable.Range(1, count).ToList();

    private static IEnumerable<int> AsSource(IEnumerable<int> data, bool asQueryable)
        => asQueryable ? data.AsQueryable() : data.ToList();

    public static TheoryData<bool> SourceKinds() => new() { false, true };

    // 12 items, page size 5 => pages: [1..5], [6..10], [11,12]

    [Theory]
    [MemberData(nameof(SourceKinds))]
    public void FirstPage_HasNoPrevious_HasNext(bool asQueryable)
    {
        var page = PagedList.Create(AsSource(Range(12), asQueryable), 1, 5);

        Assert.Equal(1, page.PageIndex);
        Assert.Equal(5, page.PageSize);
        Assert.Equal(12, page.TotalCount);
        Assert.Equal(3, page.TotalPages);
        Assert.Equal(new[] { 1, 2, 3, 4, 5 }, page.Items);
        Assert.False(page.HasPreviousPage);
        Assert.True(page.HasNextPage);
    }

    [Theory]
    [MemberData(nameof(SourceKinds))]
    public void MiddlePage_HasBothNeighbours(bool asQueryable)
    {
        var page = PagedList.Create(AsSource(Range(12), asQueryable), 2, 5);

        Assert.Equal(2, page.PageIndex);
        Assert.Equal(new[] { 6, 7, 8, 9, 10 }, page.Items);
        Assert.True(page.HasPreviousPage); // regression: was false before the 1-based fix
        Assert.True(page.HasNextPage);
    }

    [Theory]
    [MemberData(nameof(SourceKinds))]
    public void LastPage_HasPrevious_HasNoNext(bool asQueryable)
    {
        var page = PagedList.Create(AsSource(Range(12), asQueryable), 3, 5);

        Assert.Equal(3, page.PageIndex);
        Assert.Equal(new[] { 11, 12 }, page.Items);
        Assert.True(page.HasPreviousPage);
        Assert.False(page.HasNextPage);
    }

    [Theory]
    [MemberData(nameof(SourceKinds))]
    public void SinglePage_HasNoNeighbours(bool asQueryable)
    {
        var page = PagedList.Create(AsSource(Range(3), asQueryable), 1, 5);

        Assert.Equal(1, page.TotalPages);
        Assert.Equal(new[] { 1, 2, 3 }, page.Items);
        Assert.False(page.HasPreviousPage);
        Assert.False(page.HasNextPage);
    }

    [Theory]
    [MemberData(nameof(SourceKinds))]
    public void EmptySource_ProducesEmptyPage(bool asQueryable)
    {
        var page = PagedList.Create(AsSource(Array.Empty<int>(), asQueryable), 1, 5);

        Assert.Equal(1, page.PageIndex);
        Assert.Equal(0, page.TotalCount);
        Assert.Equal(0, page.TotalPages);
        Assert.Empty(page.Items);
        Assert.False(page.HasPreviousPage);
        Assert.False(page.HasNextPage);
    }

    [Theory]
    [MemberData(nameof(SourceKinds))]
    public void PageBeyondLast_IsEmptyButKeepsMetadata(bool asQueryable)
    {
        var page = PagedList.Create(AsSource(Range(12), asQueryable), 5, 5);

        Assert.Equal(5, page.PageIndex);
        Assert.Equal(3, page.TotalPages);
        Assert.Empty(page.Items);
        Assert.True(page.HasPreviousPage);
        Assert.False(page.HasNextPage);
    }

    [Fact]
    public void Empty_Factory_IsInertPage()
    {
        var page = PagedList.Empty<int>();

        Assert.Equal(1, page.PageIndex);
        Assert.Equal(0, page.TotalCount);
        Assert.Equal(0, page.TotalPages);
        Assert.Empty(page.Items);
        Assert.False(page.HasPreviousPage);
        Assert.False(page.HasNextPage);
    }

    [Fact]
    public void PreSlicedCtor_MatchesComputedCtor_ForSameLogicalPage()
    {
        var computed = PagedList.Create(Range(12).ToList(), 2, 5);
        var slice = new PagedList<int>(new[] { 6, 7, 8, 9, 10 }, 2, 5, 12);

        Assert.Equal(computed.PageIndex, slice.PageIndex);
        Assert.Equal(computed.PageSize, slice.PageSize);
        Assert.Equal(computed.TotalCount, slice.TotalCount);
        Assert.Equal(computed.TotalPages, slice.TotalPages);
        Assert.Equal(computed.HasPreviousPage, slice.HasPreviousPage);
        Assert.Equal(computed.HasNextPage, slice.HasNextPage);
        Assert.Equal(computed.Items, slice.Items);
    }

    [Fact]
    public void SuppliedTotalCount_OverridesSourceCount()
    {
        // source holds only the current slice, real total is 12
        var page = new PagedList<int>(new[] { 6, 7, 8, 9, 10 }, 2, 5, count: 12);

        Assert.Equal(12, page.TotalCount);
        Assert.Equal(3, page.TotalPages);
        Assert.True(page.HasNextPage);
    }

    [Fact]
    public void Projection_PreservesMetadataAndConvertsItems()
    {
        var source = PagedList.Create(Range(12).ToList(), 2, 5);

        var projected = PagedList.From<int, string>(source, items => items.Select(i => $"#{i}"));

        Assert.Equal(source.PageIndex, projected.PageIndex);
        Assert.Equal(source.PageSize, projected.PageSize);
        Assert.Equal(source.TotalCount, projected.TotalCount);
        Assert.Equal(source.TotalPages, projected.TotalPages);
        Assert.True(projected.HasPreviousPage);
        Assert.True(projected.HasNextPage);
        Assert.Equal(new[] { "#6", "#7", "#8", "#9", "#10" }, projected.Items);
    }

    [Theory]
    [InlineData(0, 5)]
    [InlineData(-1, 5)]
    [InlineData(1, 0)]
    [InlineData(1, -3)]
    public void Create_RejectsOutOfRangeArguments(int pageIndex, int pageSize)
        => Assert.Throws<ArgumentOutOfRangeException>(() => PagedList.Create(Range(12).ToList(), pageIndex, pageSize));

    [Fact]
    public void PreSlicedCtor_RejectsNegativeCount()
        => Assert.Throws<ArgumentOutOfRangeException>(() => new PagedList<int>(Range(5).ToList(), 1, 5, count: -1));

    [Fact]
    public void PreSlicedCtor_RejectsOutOfRangePageArguments()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new PagedList<int>(Range(5).ToList(), 0, 5, 5));
        Assert.Throws<ArgumentOutOfRangeException>(() => new PagedList<int>(Range(5).ToList(), 1, 0, 5));
    }
}
