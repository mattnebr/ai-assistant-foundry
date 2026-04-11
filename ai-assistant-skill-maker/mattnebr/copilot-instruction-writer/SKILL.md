---
name: 'copilot-instruction-writer'
description: 'Generates a ready-to-commit GitHub Copilot *.instructions.md file for a specific layer or area of a codebase. Use when a human asks to create, write, or generate a Copilot instruction file, a .instructions.md file, or Copilot conventions for a specific part of their repo.'
---

# Copilot Instruction File Writer

Generate a high-quality, ready-to-commit `*.instructions.md` file for GitHub Copilot based on information the human provides.

## Preflight — Collect Required Information

Before writing anything, ask the human for the following. Collect all answers in one go:

1. **Layer / area name** — What is this instruction file for? (e.g., API controllers, domain layer, tests, infrastructure, workflows)
2. **File scope** — Which files should this apply to? Provide example paths or describe the folder structure. (Used to derive the `applyTo` glob.)
3. **Tech stack** — What languages, frameworks, and key libraries are used in this area? (e.g., C# 13 / .NET 9, MSTest, NSubstitute, MediatR, EF Core)
4. **Existing tooling** — What is already enforced by `.editorconfig`, linters, Roslyn analyzers, or CI? (Rules enforced by tooling must be excluded from instructions.)
5. **Key conventions to follow** — What patterns, libraries, or approaches should Copilot use here that aren't enforced by tooling?
6. **Things to avoid** — What anti-patterns, banned libraries, or rejected approaches exist? Include the *why* for any non-obvious items.
7. **Architecture constraints** — Any cross-layer rules? (e.g., "this layer must not reference the Infrastructure project")
8. **Code examples** — Optional: paste 1–2 short representative snippets from this area to help illustrate the conventions.

---

## Generation Process

Read `references/quality-rules.md` before writing the file.

1. **Derive the `applyTo` glob** from the scope the human described. Use Minimatch-style syntax, relative to the repo root, no leading `./` or `/`, no exclusion patterns. If the human's paths suggest multiple patterns, use a comma-separated single string.

2. **Write the file** using the four-section template in `references/quality-rules.md`:
   - **Context** — 2–4 sentences on what this layer does and any architectural constraints
   - **Do** — Specific, verifiable rules as bullet points. Each rule must be concrete enough that a developer could check compliance.
   - **Avoid** — Anti-patterns and banned approaches. Include *why* for non-obvious items.
   - **Examples** — Short inline snippets only if the Do/Avoid rules genuinely need illustration. Omit if rules are self-explanatory.

3. **Apply all quality rules** from `references/quality-rules.md`:
   - No rules already covered by the human's stated tooling
   - No prose paragraphs — bullets only
   - No secrets, internal URLs, or sensitive details
   - No large code blocks or third-party snippets
   - Positive `applyTo` patterns only, correctly cased

4. **Output the complete file** as a fenced Markdown code block, ready to save as `<name>.instructions.md` under `.github/instructions/`.

5. **After the output**, briefly note:
   - The suggested filename
   - Any `applyTo` glob assumptions made that the human should verify
   - Any rules the human mentioned that were excluded because they're tooling-enforced (so nothing is silently dropped)