---
name: git-creating-branches
description: Create a new local Git branch following team naming conventions. Use when asked to "create a branch", "start a new feature", "start work on", "branch for", or any task requiring a new local branch off main. Handles pre-flight checks, name generation, and user approval before execution. Does NOT handle commits, merges, or post-merge cleanup.
---

# Creating Git Branches

Create new local branches from `main` following the `{type}/{author}/{description}` naming convention. Never use worktrees.

## Scope Boundaries

This skill ONLY creates local branches. Do NOT:
- Make commits, merge branches, or delete branches unless explicitly instructed.
- Use `git worktree`.
- Perform any post-merge cleanup.

## Workflow

### 1. Gather Context

Determine the branch type, ticket reference, and purpose from the user's request. Consult `references/branch-naming-conventions.md` for valid types and formatting rules.

Ask the user for their **author identifier** (initials or hyphenated full name, lowercase). Ask every time — do not assume or cache from prior conversations.

If no ticket reference is mentioned, ask whether one exists. Use `no-ref` only when the user confirms no ticket applies.

If there is not enough context to generate meaningful branch name suggestions, ask clarifying questions about the purpose of the work before proceeding.

### 2. Pre-Flight Checks

Run these checks in order. Stop and report on the first failure:

```bash
# Verify inside a git repository
git rev-parse --is-inside-work-tree  # Must output: true

# Confirm on main
git branch --show-current  # Must output: main

# Pull latest
git pull origin main

# Verify clean working tree
git status --porcelain  # Must be empty
```

If not a git repository, inform the user and stop.
If not on `main`, inform the user and stop. Do NOT switch branches automatically.
If the working tree is dirty, inform the user and stop. Do NOT stash or commit.

### 3. Propose Branch Names

Generate 2–3 branch name options following the `{type}/{author}/{description}` convention. Each suggestion should vary the description phrasing while keeping names concise and specific. Validate each:
- All lowercase, alphanumeric + hyphens + forward slashes only (dots allowed in release versions).
- Description starts with ticket reference (`issue-###`) or `no-ref`.
- Description is concise and specific — no generic terms.

Check that none of the proposed names already exist:

```bash
git branch --list "{proposed-name}"
```

Present all options to the user and wait for them to pick one, request a modification, or provide their own. Do NOT proceed without explicit confirmation.

### 4. Dry-Run

Display the exact commands that will execute:

```
The following commands will be run:
  git checkout -b {approved-branch-name}
```

Wait for user approval before executing. Always show commands first — never execute without confirmation.

### 5. Execute

After user approval, create and switch to the new branch:

```bash
git checkout -b {approved-branch-name}
```

Confirm success by displaying:

```bash
git branch --show-current
```