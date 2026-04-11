# AI Assistant Foundry

A curated vault of AI assistant skills and instructions, organized by category for use with Claude, GitHub Copilot, and other AI coding assistants.

## What is this?

This repository is a personal library of reusable AI skills and Copilot instruction files collected from open-source GitHub repositories. Each skill is a markdown file (or set of files) that gives an AI assistant deep expertise in a specific domain -- from code review and architecture to marketing copywriting and project management.

Skills are downloaded, categorized, and archived using [ai-assistant-foundry-loader](https://github.com/mattnebr/ai-assistant-foundry-loader).

## Folder structure

```
{category}/
  {source-owner}/
    {skill-name}/
      SKILL.md                          # or {name}.instructions.md
      references/                       # optional supporting files
```

- **Category** -- a human-chosen group name (e.g. `engineer-software`, `reviewer-code`, `writer-marketing`)
- **Source owner** -- the GitHub username or organization the skill was sourced from
- **Skill name** -- the original skill folder or instruction file name from the source repo

## How to use

Point your AI assistant at any `SKILL.md` or `.instructions.md` file to give it specialized knowledge for a task. These files work with Claude Code custom instructions, GitHub Copilot instruction files, and similar AI assistant configuration systems.

## How skills are added

Skills are sourced from public GitHub repositories and archived using the [ai-assistant-foundry-loader](https://github.com/mattnebr/ai-assistant-foundry-loader) CLI tool, which:

1. Scans configured repos for `SKILL.md` files and `.instructions.md` files
2. Presents an interactive prompt to categorize each skill into a group
3. Downloads and lints the markdown (BOM removal, trailing whitespace, heading spacing)
4. Writes files to the vault in the folder structure shown above
5. Tracks what has been archived in `_sources.json` to avoid duplicates

## Browsing with Obsidian

This vault is designed to work well as an [Obsidian](https://obsidian.md/) vault. Open this folder in Obsidian to browse, search, and link between skills. The `.obsidian/` folder is git-ignored.

## Index

See [index.md](index.md) for a full listing of all skills organized by category.
