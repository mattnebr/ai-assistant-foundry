---
name: git-no-pull-requests
description: 'Solo developer Git workflow using local-only branching and merging without pull requests. Use when working in repositories where this skill is present in the .claude folder. Governs all Git operations: branch creation, committing, merging to main. Overrides team-oriented Git defaults (no PRs, no remote writes). Pair with git-creating-branches skill for branch naming.'
---
# Solo Developer Git Workflow

Local-only branch-and-merge workflow for repositories with a single developer. No pull requests. No writing to origin.

## Core Rule: Never Write to Origin

The assistant MUST NOT run any command that writes to the remote:

**Blocked commands** (never run these):
```
git push
git push origin
git push --tags
git remote add/remove/set-url
```

**Allowed commands** (reading from origin is safe):
```
git pull
git fetch
git remote -v (read-only inspection)
```

The user pushes to origin manually after acceptance testing. This preserves the "undo button" — delete local and re-clone if anything goes wrong.

## Why No Pull Requests

Solo developer repository. Pull requests exist for code review and team collaboration, neither of which apply. Do not suggest, create, or reference pull requests, merge requests, or remote branch workflows.

## Branch Creation

Follow the `git-creating-branches` skill for naming conventions and pre-flight checks. That skill's `git pull origin main` step is permitted — it reads from origin, which is safe.

## Workflow

### 1. Make Changes on the Feature Branch

Work on the feature branch as directed. Do not commit automatically — the user reviews changes in Visual Studio before approving.

### 2. Commit (Only After User Approval)

Wait for explicit user sign-off before committing. One commit per feature branch is the default.

```bash
git add <explicit-file-paths>
git commit -m "<descriptive-message>"
```

Use explicit file paths with `git add` — avoid `git add .` or `git add -A` unless the user requests it.

### 3. Merge to Main

```bash
git checkout main
git merge <branch-name>
```

Fast-forward merge is the default. If the user requests a merge commit, use `--no-ff`.

### 4. Handle Conflicts

If the merge produces conflicts, attempt to resolve them. If resolution is ambiguous or involves logic decisions, present the conflicts to the user and wait for guidance.

### 5. Verify

Confirm the merge completed:

```bash
git log --oneline -5
```

The workflow ends here. Do not delete the feature branch. Do not push to origin.