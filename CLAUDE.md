# CLAUDE.md

Guidance for working in the NimblePros **eShopOnWeb** reference app. Monolithic ASP.NET Core (currently **.NET 10**) demonstrating Clean Architecture. Solution: `eShopOnWeb.slnx`.


## Clean Architecture layers

Dependencies point inward: `Web`/`PublicApi` → `Infrastructure` → `ApplicationCore`. `ApplicationCore` depends on nothing else.

- **`src/ApplicationCore`** — the domain core. Entities, aggregates, domain services, interfaces (`IRepository`, etc.), specifications, and domain exceptions. **No infrastructure or UI concerns here.** Business rules live here.
- **`src/Infrastructure`** — implementation details: EF Core `DbContext`s (catalog + identity), repositories, data migrations, external integrations. Implements interfaces defined in `ApplicationCore`.
- **`src/Web`** — the MVC + Razor Pages storefront. UI-specific services (e.g. view-model services that map domain types to view models / `SelectListItem`), pages, controllers. UI-only logic — no business rules.
- **`src/PublicApi`** — minimal-API endpoints consumed by the Blazor admin.
- **`src/BlazorAdmin` / `src/BlazorShared`** — Blazor WebAssembly admin UI and its shared DTOs.
- **`src/eShopWeb.AppHost` / `eShopWeb.AspireServiceDefaults`** — .NET Aspire orchestration/host.

Rule of thumb: if it's a business rule, it belongs in `ApplicationCore`; if it talks to a database or external system, `Infrastructure`; if it only shapes data for display, the relevant UI project.

## How catalog filters are structured

Filtering uses the **Ardalis.Specification** pattern (`Specification<CatalogItem>`), not inline LINQ in services.

- **`CatalogFilterSpecification(int? brandId, int? typeId)`** — filters the full result set (used for the total count). Null filter = "match all".
- **`CatalogFilterPaginatedSpecification(int skip, int take, int? brandId, int? typeId)`** — same `Where` plus `Skip`/`Take`; `take == 0` is treated as "all".
- Both live in `src/ApplicationCore/Specifications/`.

Flow: `Web/Services/CatalogViewModelService.GetCatalogItems(...)` builds both specs, calls `_itemRepository.ListAsync(paginatedSpec)` for the page and `CountAsync(filterSpec)` for the total, then projects to `CatalogIndexViewModel`. `CachedCatalogViewModelService` decorates it with caching (see `Web/Extensions/CacheHelpers.cs`). The same specs back the paged `PublicApi` endpoint (`CatalogItemListPagedEndpoint`).

**When adding a filter dimension (e.g. price range):** add/extend the `Where` in *both* specs so the count and the page stay consistent, thread the new parameter through `ICatalogViewModelService` → `CatalogViewModelService` → `Index.cshtml.cs`, and keep the "null means no filter" convention.

## Branch & commit naming

- **Branches:** `<username>/<kebab-case-feature>` — e.g. `sheamusmcg/catalog-price-range-filter`, `ardalis/mediator-migration`. Branch from `main`.
- **Commits:** imperative, sentence-case, no trailing period — e.g. `Add price-range filter to catalog page`, `Revise price-range control requirements and criteria`. Dependabot/merge commits are machine-generated; don't imitate them.
- **PRs:** target `main`; add tests for new code and update docs when APIs change (see `CONTRIBUTING.md`).

## Build & test commands

```bash
# Restore & build the whole solution
dotnet build eShopOnWeb.slnx

# Run the test suites (each is an independent project)
dotnet test tests/UnitTests/UnitTests.csproj
dotnet test tests/IntegrationTests/IntegrationTests.csproj
dotnet test tests/FunctionalTests/FunctionalTests.csproj
dotnet test tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj

# Run everything at once
dotnet test eShopOnWeb.slnx
```

### End-to-end (Playwright) tests

These drive the running app (`src/Web`) at `https://localhost:5001`. Browsers must be installed once:

```bash
dotnet build tests/EndToEndTests/EndToEndTests.csproj
pwsh tests/EndToEndTests/bin/Debug/net10.0/playwright.ps1 install
dotnet test tests/EndToEndTests/EndToEndTests.csproj
```

Failure artifacts (screenshot, trace, video) land under `tests/EndToEndTests/TestResults/PlaywrightArtifacts/`. See `tests/EndToEndTests/README.md` and the Playwright skills in `.claude/skills/` for test-authoring conventions (prefer role-based locators, assert user-visible behavior, no arbitrary sleeps).

### Running the app locally

```bash
# In-memory DB: add "UseOnlyInMemoryDatabase": true to src/Web/appsettings.json, then:
cd src/Web && dotnet run --launch-profile https   # https://localhost:5001
# Admin (Blazor) also needs PublicApi running: cd src/PublicApi && dotnet run
```
