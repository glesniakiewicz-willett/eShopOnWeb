---
name: open-pr
description: Open a pull request against my fork from the current branch. Refuses on main or with uncommitted changes, summarizes the diff by layer, runs affected test projects, drafts a PR description from .claude/pr-template.md, flags repo-specific risks, and waits for approval before pushing. Targets my fork only.
disable-model-invocation: true
metadata:
  author: glesniakiewicz
  version: "1.0"
  fork: glesniakiewicz-willett/eShopOnWeb
---

# open-pr

Prepare and open a pull request from the current branch against **my fork only**
(`glesniakiewicz-willett/eShopOnWeb`, base `main`). Execute the steps below **in
order**. Do not skip ahead, and do not push until the human approves (step 6).

Read-only git commands are pre-approved. Anything that mutates the repo or remote
(`git push`, `git commit`, `git add`, `gh pr create`) must NOT run before step 6.

## Step 1 — Preconditions (refuse if violated)

Run `git rev-parse --abbrev-ref HEAD` and `git status --short --branch`.

Refuse and STOP, stating exactly why, if either holds:

- **On `main`.** Say: "Refusing: you're on `main`. Create a feature branch first
  (`git switch -c feature/...`)." Do not continue.
- **Uncommitted changes** (any staged, unstaged, or untracked entry in
  `git status --short`). Say: "Refusing: working tree is not clean. Commit or stash
  these first:" and list the paths. Do not continue.

Only proceed to step 2 if the branch is not `main` AND the tree is clean.

## Step 2 — Summarize the diff by layer

Diff the branch against base: `git diff --stat main...HEAD` and
`git diff main...HEAD`. Group the changed files by architectural layer, not
file-by-file:

- **ApplicationCore** — `src/ApplicationCore/**` (entities, specifications, interfaces)
- **Infrastructure** — `src/Infrastructure/**` (EF Core, data, migrations)
- **Web** — `src/Web/**` (Razor pages, view models, services, caching)
- **PublicApi** — `src/PublicApi/**`
- **BlazorAdmin / BlazorShared** — `src/BlazorAdmin/**`, `src/BlazorShared/**`
- **Tests** — `tests/**`
- **Build / infra / docs** — everything else (Dockerfiles, `infra/`, `docs/`, props)

For each layer present, give a one-line summary of what changed.

## Step 3 — Run the affected test projects

Map the changed layers to test projects and run only the affected ones:

| Changed layer | Run |
| --- | --- |
| ApplicationCore | `tests/UnitTests/UnitTests.csproj`, `tests/IntegrationTests/IntegrationTests.csproj` |
| Infrastructure | `tests/IntegrationTests/IntegrationTests.csproj`, `tests/UnitTests/UnitTests.csproj` |
| Web | `tests/FunctionalTests/FunctionalTests.csproj`, `tests/UnitTests/UnitTests.csproj` |
| PublicApi | `tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj` |

Run each with `dotnet test <project>` and report the real observed result
(pass/fail + counts) for each. Do NOT run `tests/EndToEndTests` (Playwright, slow)
unless the human explicitly asks. If a test project fails to build or run, report
that honestly rather than guessing. Record exact commands + outcomes for step 4.

## Step 4 — Draft the PR description

Use the template at `.claude/pr-template.md`. Fill all four sections — **Summary**,
**Changes**, **Testing**, **Risks**:

- **Changes** comes from the step-2 layer summary.
- **Testing** must separate **Tests run (this session)** — only the step-3 commands
  you actually executed, with observed results — from **Tests recommended (not yet
  run)**. Never present a recommended test as if it passed.

## Step 5 — Flag repo-specific risks

In the **Risks** section, explicitly check each of these and state the finding
(call out the risk, or note "checked — OK"):

1. **Catalog filter specs drifting.** `src/ApplicationCore/Specifications/CatalogFilterSpecification.cs`
   and `CatalogFilterPaginatedSpecification.cs` must stay in sync. If filter logic
   (e.g. a price range) is added/changed in one but not the other, the filtered list
   and the paged/count query diverge. Flag if only one was touched.
2. **Cache key missing a new filter.** `src/Web/Extensions/CacheHelpers.cs`
   (`GenerateCatalogItemCacheKey`) and `src/Web/Services/CachedCatalogViewModelService.cs`.
   If a new filter parameter isn't part of the cache key, stale/incorrect cached
   results get served for different filter inputs. Flag if a filter was added without
   a matching cache-key change.
3. **Layer-boundary breaks.** Flag any dependency that crosses layers the wrong way
   (e.g. ApplicationCore referencing Web/Infrastructure, Web reaching into
   Infrastructure internals).
4. **Migrations.** Flag any added/changed EF Core migration under
   `src/Infrastructure/**` (schema change — needs review and a deploy/migration plan).

Also note standard risks when present: auth/authorization, secrets/config, deletions
or renames, public API changes, pricing/order logic, dependency bumps.

## Step 6 — Show and wait for approval (hard gate)

Print the proposed **PR title** and the full **description**. Then STOP and ask for
explicit approval to push and open the PR. Do not push, commit, or run `gh pr create`
until the human clearly approves. If they request edits, revise and show again.

## Step 7 — Push and open the PR (my fork only)

After approval:

1. Push the branch: `git push -u origin HEAD`.
2. Open the PR against my fork only:
   `gh pr create --repo glesniakiewicz-willett/eShopOnWeb --base main --head <branch> --title "<title>" --body "<description>"`
   Never target the upstream repo.
3. **If `gh` is unavailable** (not installed / not authenticated), do NOT fail
   silently. Provide:
   - a compare link to paste into the browser:
     `https://github.com/glesniakiewicz-willett/eShopOnWeb/compare/main...<branch>?expand=1`
   - the full PR title and description (from step 6) for the human to paste.

## Guardrails

- Target **my fork only** (`glesniakiewicz-willett/eShopOnWeb`). Never open a PR
  against any upstream/original repository.
- No repo-mutating action (push/commit/add/`gh pr create`) before step-6 approval.
- Report real test results only — never claim a test passed that you didn't run.
