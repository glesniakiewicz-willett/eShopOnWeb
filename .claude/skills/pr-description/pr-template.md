# PR Template

Copy the block below into the pull request. Replace the guidance in _italics_.

---

## Summary

_One or two sentences: what this PR does and why. Written for a reviewer who has
not seen the code._

## Changes

_Bullet list of the notable changes, grouped by area. Call out which are staged vs
unstaged/untracked if that matters for what will actually be committed._

- _Change 1_
- _Change 2_

## Testing

### Tests run (this session)

_Only tests actually executed, with command and observed result. Write "None run
in this session." if nothing was run._

- `dotnet test tests/UnitTests/UnitTests.csproj` — _e.g. passed, 123 tests_

### Tests recommended (not yet run)

_Tests the author should run before merge. These have NOT been executed._

- _e.g._ `dotnet test eShopOnWeb.slnx`
- _e.g._ E2E: `dotnet test tests/EndToEndTests/EndToEndTests.csproj`

## Risks

_Risk level (Low / Medium / High) and why. Explicitly flag: migrations, auth,
security/secrets/config, deletions/renames, public API changes, pricing/order
logic, broad refactors, dependency bumps. Write "Low — no risky areas touched" if
applicable._

- _Risk / impacted area 1_

### Open questions / missing info

_Anything unclear or absent: missing tests, undocumented API change, ambiguous
intent. Omit this subsection if there is nothing to flag._

- _Open question 1_
