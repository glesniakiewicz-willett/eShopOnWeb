---
name: pr-description
description: Summarize staged and unstaged git changes and draft a PR description using the Summary / Changes / Testing / Risks template. Separates tests actually run from tests only recommended, and flags risky changes and missing info. Manual-only — does NOT commit, push, stage, or open a PR.
disable-model-invocation: true
metadata:
  author: nimblepros
  version: "1.0"
  spec: agentskills.io
---

# PR Description Skill

## Purpose

Produce a ready-to-paste pull request description from the current working-tree
changes. This skill **only reads and reports** — it never modifies git state.

## Hard Guardrails (never violate)

- Do **NOT** run `git commit`, `git push`, `git add`, `git stage`, `git reset`,
  `git checkout`, `gh pr create`, or anything that mutates the repo or remote.
- Do **NOT** edit, create, or delete source files as part of this skill.
- Output is the PR description text (and notes) only. The human copies it into the PR.
- If asked within this skill to commit/push/open the PR, decline and restate this boundary.

## Workflow

1. **Collect the changes (read-only).**
   - `git status --short --branch` — overview of staged vs unstaged vs untracked.
   - `git diff --staged` — staged changes.
   - `git diff` — unstaged changes to tracked files.
   - Note untracked files separately (they won't appear in `git diff`).
   - If there are no changes at all, say so and stop.

2. **Summarize what changed.**
   - Group related edits by feature/area, not file-by-file noise.
   - Distinguish **staged** from **unstaged/untracked** so the author knows what
     would actually be committed right now.

3. **Draft the description** using `pr-template.md` (beside this file). Fill all four
   sections: **Summary**, **Changes**, **Testing**, **Risks**.

4. **Separate tests run from tests recommended.** In the Testing section:
   - **Tests run** — only list a test/command here if you actually executed it in
     this session and observed the result. Include the command and the outcome
     (pass/fail/counts). If you ran nothing, write "None run in this session."
   - **Tests recommended** — suggested tests/commands the author should run before
     merging. Clearly marked as *not yet executed*.
   - Never present a recommended test as if it passed.

5. **Flag risks and missing info.**
   - Risky change signals to call out explicitly: database/EF migrations, auth or
     authorization changes, security/secrets/config, deletions or renames, public
     API or contract changes, concurrency/threading, broad refactors, dependency
     bumps, anything touching money/pricing/orders.
   - Missing-info signals: no tests added for new behavior, unclear intent, docs not
     updated for an API change, large diff with a thin summary, commented-out code,
     TODOs left in.
   - If something needed for a good PR description is unknown, list it under
     **Open questions / missing info** rather than guessing.

## Output Contract

Return, in this order:

1. A short line stating branch + counts (e.g. staged N files, unstaged M, untracked K).
2. The filled-in PR description (the four-section template).
3. A brief **Open questions / missing info** list if anything is unclear.

## Quality Gate

- Every section of the template is filled (use "N/A" or a note if genuinely empty).
- Tests run vs recommended are unambiguously separated.
- At least one explicit risk assessment is present (even if "Low — no risky areas touched").
- No git-mutating or file-mutating action was taken.
