# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Обзор

`Calabonga.PagedListCore` — небольшая библиотека постраничной разбивки коллекций для .NET (`netstandard2.1`). Публикуется как NuGet-пакет [Calabonga.PagedListCore](https://www.nuget.org/packages/Calabonga.PagedListCore).
Решение состоит из одного проекта; тестового проекта в решении нет.

Дополнительные правила проекта подключаются автоматически из `.claude/rules/` (`code-styles.md` — стиль C#, `workflow.md` — рабочий процесс). Не дублируй их содержимое здесь.

## Команды

Файл решения — в формате `.slnx` (XML), лежит в `src/`.

```bash
dotnet restore src/Calabonga.PagedListCore.slnx
dotnet build src/Calabonga.PagedListCore.slnx -c Release
dotnet pack  src/Calabonga.PagedListCore.slnx -c Release -o ./Package
```

`dotnet build` при сборке автоматически генерирует `.nupkg` (`GeneratePackageOnBuild=True`).

CI (`.github/workflows/main.yml`) на каждый push в `main` собирает Release, пакует и пушит пакет в nuget.org (`--skip-duplicate`). Раннер — `windows-latest`, SDK — .NET 10. Тесты в CI не запускаются (тестового проекта нет).

Проекта с unit-тестами в решении нет. Если задача требует тестов — спроси пользователя, нужно ли добавить тестовый проект (он в NuGet-пакет и в CI не входит).

## Архитектура

Четыре файла в `src/Calabonga.PagedListCore/`, все в namespace `Calabonga.PagedListCore`:

- **`IPagedList.cs`** — `IPagedList<T>`: контракт страницы  (`PageIndex`, `PageSize`, `TotalCount`, `TotalPages`, `Items`, `HasPreviousPage`, `HasNextPage`). Единственный публичный тип, на который завязаны потребители.

- **`PagedListOfT.cs`** — `public class PagedList<T> : IPagedList<T>`, основная реализация. Два пути создания:
  - `internal` конструкторы принимают **полный** `IEnumerable<T>`/`IQueryable<T>` и сами делают `Skip/Take`. Для `IQueryable` считают `Count()` в БД, если `totalCount` не передан. Здесь `PageIndex` сохраняется как `pageIndex - 1` (переданный индекс считается 1-базовым, хранится 0-базовым). 
  - `public` конструктор `(source, pageIndex, pageSize, count)` принимает **уже отобранную** страницу и готовый `count`; `PageIndex` сохраняется как есть, без сдвига. Используй его, когда пагинация уже выполнена на стороне вызова.

- **`PagedList.cs`** — `public static class PagedList`, точка входа фабрики: `Empty<T>()`, `Create<T>(items, pageIndex, pageSize)`,
  `From<TSource,TResult>(source, converter)` и `Create<TSource,TResult>(source, converter)` (последние два — синонимы).

- **`PagedListOtTresultAndTSource.cs`** — `internal class PagedList<TSource, TResult> : IPagedList<TResult>`, адаптер проекции: оборачивает существующий `IPagedList<TSource>` (или сырой источник) и прогоняет его `Items` через `Func<IEnumerable<TSource>, IEnumerable<TResult>>`, сохраняя метаданные страницы. Создаётся только через фабрику `PagedList.From/Create`.

### Замечания по семантике индексов

- `HasPreviousPage => PageIndex > 1`, `HasNextPage => PageIndex + 1 < TotalPages`.
  Эти выражения предполагают конкретную базу индексации; при любой правке
  конструкторов `PagedList<T>` проверяй согласованность `PageIndex` во всех трёх
  путях создания (два `internal` + один `public`) и в обоих флагах.
- Исторические баги в этой библиотеке касались именно расчёта страниц
  (см. changelog в `README.md`, разделы v1.0.4 и v2.0.0). Любое изменение логики
  `Skip/Take`/`TotalPages` покрывай тестами вручную для первой, средней и
  последней страницы.

## Версионирование и changelog

- Версия пакета — `<Version>` в `src/Calabonga.PagedListCore/Calabonga.PagedListCore.csproj`.
- Changelog ведётся в **двух** файлах, держи их синхронными: корневой `README.md`
  и `src/Calabonga.PagedListCore/README.md` (второй упаковывается в NuGet как
  `PackageReadmeFile`).
- Обновляй версию и оба changelog в том же PR, что и функциональные изменения.
