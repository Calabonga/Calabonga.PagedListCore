## Правила рабочего процесса

- Всегда создавай отдельную ветку Git перед внесением изменений.  `main` — основная ветка, публикуется в NuGet через GitHub Actions при push.
- Допустимые префиксы веток: `feature/`, `bugfix/`, `hotfix/`.
- Формат коммитов: `type: description` (`feat`, `fix`, `refactor`, `test`, `docs`, `style`, `perf`, `build`, `chore`, `revert`).
- Атомарные коммиты — одно логическое изменение на коммит.
- Перед созданием нового класса проверь, нет ли файла с таким же именем в решении.
- Сборка и тесты:
  - `dotnet build src/Calabonga.PagedListCore.slnx -c Release`
  - `dotnet test src/Calabonga.PagedListCore.slnx -c Release` — после каждой реализации и обязательно перед фиксацией изменений.
- Версию пакета (`<Version>` в `Calabonga.PagedListCore.csproj`) и changelog в `README.md` обновляй в том же PR, что и функциональные изменения.
- CI (`.github/workflows/main.yml`) на push в `main`: restore → build → **test** → pack → push в nuget.org. Тесты гейтят публикацию: упавший `dotnet test` останавливает пакет.
- Тестовый проект (`IsPackable=false`) в NuGet-пакет не входит — `dotnet pack` по решению собирает только пакет библиотеки.