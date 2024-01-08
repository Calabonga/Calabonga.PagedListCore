# Calabonga.PagedListCore

PagedList implementation for .NET Core (netstandard2.0).

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