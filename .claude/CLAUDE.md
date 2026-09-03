# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Обзор

`Calabonga.PagedListCore` — небольшая библиотека постраничной разбивки коллекций для .NET (`netstandard2.1`). Публикуется как NuGet-пакет [Calabonga.PagedListCore](https://www.nuget.org/packages/Calabonga.PagedListCore).
Решение — библиотека + проект unit-тестов (`tests/Calabonga.PagedListCore.Tests`, xUnit, `net10.0`, `IsPackable=false`).

Дополнительные правила проекта подключаются автоматически из `.claude/rules/` (`code-styles.md` — стиль C#, `workflow.md` — рабочий процесс). Не дублируй их содержимое здесь.

## Команды

Файл решения — в формате `.slnx` (XML), лежит в `src/`.

```bash
dotnet restore src/Calabonga.PagedListCore.slnx
dotnet build src/Calabonga.PagedListCore.slnx -c Release
dotnet test  src/Calabonga.PagedListCore.slnx -c Release
dotnet pack  src/Calabonga.PagedListCore.slnx -c Release -o ./Package
```

`dotnet build` при сборке автоматически генерирует `.nupkg` (`GeneratePackageOnBuild=True`). `dotnet pack` по решению собирает **только** пакет библиотеки — тестовый проект помечен `IsPackable=false`.

Запуск одного теста: `dotnet test src/Calabonga.PagedListCore.slnx --filter "FullyQualifiedName~PagedListTests.MiddlePage_HasBothNeighbours"`.

CI (`.github/workflows/main.yml`) на каждый push в `main` собирает Release, пакует и пушит пакет в nuget.org (`--skip-duplicate`). Раннер — `windows-latest`, SDK — .NET 10. Отдельного шага `dotnet test` в CI нет — тесты гоняй локально перед фиксацией.

## Архитектура

Файлы в `src/Calabonga.PagedListCore/`, все в namespace `Calabonga.PagedListCore`:

- **`IPagedList.cs`** — `IPagedList<T>`: контракт страницы (`PageIndex`, `PageSize`, `TotalCount`, `TotalPages`, `Items`, `HasPreviousPage`, `HasNextPage`). Единственный публичный тип, на который завязаны потребители.

- **`PagedListHelper.cs`** — `internal static`, общая арифметика: `EnsureValidArguments` (валидация `pageIndex`/`pageSize`), `GetSkipCount`, `GetTotalPages`. Все вычисляющие конструкторы обязаны идти через него, чтобы пути создания не разошлись.

- **`PagedListOfT.cs`** — `public class PagedList<T> : IPagedList<T>`, основная реализация. Три конструктора:
  - `internal (source, pageIndex, pageSize, totalCount = null)` — принимает **полный** `IEnumerable<T>`/`IQueryable<T>` и сам делает `Skip/Take`. Для `IQueryable` считает `Count()` в БД, если `totalCount` не передан.
  - `internal ()` — пустой результат-заглушка (`PageIndex = 1`, `PageSize = 0`, всё остальное — 0/пусто).
  - `public (source, pageIndex, pageSize, count)` — принимает **уже отобранную** страницу и готовый `count`. Используй, когда пагинация уже выполнена на стороне вызова.
  - `PageIndex` — **1-базовый во всех конструкторах** (первая страница = 1). `Skip` считается как `(pageIndex - 1) * pageSize` внутри, наружу 0-базовое значение не протекает.

- **`PagedList.cs`** — `public static class PagedList`, точка входа фабрики: `Empty<T>()`, `Create<T>(items, pageIndex, pageSize)`,
  `From<TSource,TResult>(source, converter)` и `Create<TSource,TResult>(source, converter)` (последние два — синонимы).

- **`PagedListOtTresultAndTSource.cs`** — `internal class PagedList<TSource, TResult> : IPagedList<TResult>`, адаптер проекции: оборачивает существующий `IPagedList<TSource>`, копирует метаданные страницы и прогоняет `Items` через `Func<IEnumerable<TSource>, IEnumerable<TResult>>`. Создаётся только через фабрику `PagedList.From/Create`.

### Замечания по семантике индексов

- `PageIndex` 1-базовый. Флаги: `HasPreviousPage => PageIndex > 1`, `HasNextPage => PageIndex < TotalPages`. При правке конструкторов держи это соглашение единым во всех путях создания и проверяй оба флага.
- `TotalPages == 0` при `TotalCount <= 0` (см. `PagedListHelper.GetTotalPages`).
- Исторические баги касались именно расчёта страниц (changelog `README.md`, v1.0.4 / v2.0.0 / v3.0.0). Любое изменение `Skip/Take`/`TotalPages`/флагов покрывай тестами для первой, средней, предпоследней, последней, единственной и пустой страницы (`tests/Calabonga.PagedListCore.Tests`).

## Версионирование и changelog

- Версия пакета — `<Version>` в `src/Calabonga.PagedListCore/Calabonga.PagedListCore.csproj`.
- Changelog ведётся в **двух** файлах, держи их синхронными: корневой `README.md`
  и `src/Calabonga.PagedListCore/README.md` (второй упаковывается в NuGet как
  `PackageReadmeFile`).
- Обновляй версию и оба changelog в том же PR, что и функциональные изменения.
