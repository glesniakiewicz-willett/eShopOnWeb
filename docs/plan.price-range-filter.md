# Price-Range Filter — Implementation Plan

## Goal
Add a price-range filter to the catalog page, mirroring the existing brand/type
filters. The range is **two-sided** (`priceMin` / `priceMax`) and **either side may
be used on its own** — both are nullable `decimal`, so min-only, max-only, both, or
neither are all valid. `CatalogItem.Price` already exists, so there is **no entity or
database change**.

## Data flow (same chain as brand/type)
`Index.cshtml` form → `IndexModel.OnGet` binding → `CatalogIndexViewModel` →
`CatalogViewModelService` (+ `CachedCatalogViewModelService` cache key) →
`CatalogFilterSpecification` / `CatalogFilterPaginatedSpecification` →
`EfRepository` → SQL.

## ApplicationCore
- **`Specifications/CatalogFilterSpecification.cs`** — add `decimal? priceMin,
  decimal? priceMax` params; extend the `Where`.
- **`Specifications/CatalogFilterPaginatedSpecification.cs`** — add the same two
  params and the **identical** `Where` clause (keeps `.Skip/.Take`).

The query clause follows the existing "null means no filter" idiom:

```
(!priceMin.HasValue || i.Price >= priceMin.Value) &&
(!priceMax.HasValue || i.Price <= priceMax.Value)
```

Both specifications must carry the **same** `Where`, or the paged list and the total
count diverge.

## Infrastructure
- **No changes.** `EfRepository<T>` / `RepositoryBase<T>` translate the specification
  to SQL automatically; `Price` is already a mapped column.

## Web
- **`Interfaces/ICatalogViewModelService.cs`** — add `decimal? priceMin, decimal?
  priceMax` to `GetCatalogItems`.
- **`Services/CatalogViewModelService.cs`** — accept the two params, pass them into
  both specifications, and echo them onto the returned view model.
- **`Services/CachedCatalogViewModelService.cs`** — thread the two params through to
  the cache-key call.
- **`Extensions/CacheHelpers.cs`** — extend the key template
  `"items-{0}-{1}-{2}-{3}"` with two more slots. **Mandatory**: without it, different
  price ranges collide on a stale cached page.
- **`ViewModels/CatalogIndexViewModel.cs`** — add `PriceMinApplied` /
  `PriceMaxApplied` (nullable `decimal`).
- **`Pages/Index.cshtml.cs`** — pass the two view-model values into the service call.
- **`Pages/Index.cshtml`** — two `<input type="number" step="0.01" min="0">` fields
  (Min / Max) inside the existing GET filter form. Each is optional, supporting
  one-sided ranges.

## Testing
- **Unit** (`tests/UnitTests/.../Specifications/`) — for *both* specs, cover:
  min-only, max-only, both (in-range vs out-of-range), neither. These also guard
  against the two specs drifting.
- **Functional** (`tests/FunctionalTests/.../HomePageOnGet.cs`) — request the catalog
  page with `PriceMinApplied` / `PriceMaxApplied` query params and assert the page
  responds and respects the bounds.
- **Cache key** — assert two different price ranges produce different keys.
- **Regression** — existing brand/type tests must still pass unchanged.

## Risk checkpoints
1. **Specs drifting** — identical clause applied to both + per-spec unit tests.
2. **Cache key** — updated template + cache-key test.
3. **Layer boundaries** — ApplicationCore stays EF-free. ✅
4. **Migrations** — none (reuses existing `Price` column). ✅

## UI decision
Two optional number inputs (Min / Max). Either may be left blank, which maps to a
`null` bound and therefore an open-ended range on that side.
