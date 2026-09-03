# Calabonga.PagedListCore

PagedList implementation for .NET Core (netstandard2.1). Nuget [Calabonga.PagedListCore](https://www.nuget.org/packages/Calabonga.PagedListCore)

## v3.0.0

* **Breaking:** `PageIndex` is now 1-based in every constructor (the internal `Skip`/`Take` path previously exposed a 0-based value).
* `HasPreviousPage` / `HasNextPage` off-by-one fixed — the second page now reports `HasPreviousPage == true`, the second-to-last reports `HasNextPage == true`.
* Argument validation added: `pageIndex >= 1`, `pageSize >= 1`, `count >= 0` throw `ArgumentOutOfRangeException`.
* Removed the unreachable `PagedList<TSource, TResult>` constructor with the leftover `indexFrom` parameter.
* Unit test project added (`tests/Calabonga.PagedListCore.Tests`).

## v2.0.0

* `IndexFrom` removed
* `PageIndex` calculation refactored

## v1.0.4

* Bux for `ToPagedList()` fixed. Only first page always generated correcty.
* Some method summaries added/updated.

## v1.0.3

* Converter created for deserialization with `System.Text.Json`.
* Some type fixed

## v1.0.2

* Redundant parameter removed from `Create` extensions.

## v1.0.1

* `TotalCount` parameter as nullable added
* `Create<T>` for static `PagedList` extension added

## v1.0.0

* Extensions updated
* Fix some typo

## v1.0.0-beta.1

* `IPagedList<T>` (and other things) were moved from nuget-package [Calabonga.UnitOfWork](https://github.com/Calabonga/UnitOfWork)
* Additional public methods of new instance creation added.

