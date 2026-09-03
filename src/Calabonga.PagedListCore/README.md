# Calabonga.PagedListCore

PagedList implementation for .NET Core (netstandard2.1).

## v3.0.0

* **Breaking:** `PageIndex` is now 1-based in every constructor.
* `HasPreviousPage` / `HasNextPage` off-by-one fixed.
* Argument validation added: `pageIndex >= 1`, `pageSize >= 1`, `count >= 0`.
* Removed the unreachable `PagedList<TSource, TResult>` constructor with the leftover `indexFrom` parameter.

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