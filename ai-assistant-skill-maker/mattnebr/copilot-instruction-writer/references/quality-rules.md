# Copilot Instruction File: Quality Rules & Template

## Output Template

```markdown
---
applyTo: "<glob-pattern>"
description: "<one-line human note — does not affect Copilot behavior>"
---

## Context
<What this layer/area is responsible for. Any architectural constraints Copilot should be aware of. 2–4 sentences max.>

## Do
- <Specific, verifiable rule>
- <Specific, verifiable rule>
- <e.g.: Use file-scoped namespaces. Prefer primary constructors for DI.>

## Avoid
- <Anti-pattern or banned approach — include the *why* if non-obvious>
- <e.g.: Never use Moq — the team standardised on NSubstitute.>

## Examples
<Short inline snippet illustrating a key Do or Avoid — only if prose alone is unclear. Omit if rules are self-explanatory.>
```

---

## Quality Rules

### Content rules

- Rules must be **specific and verifiable** — replace aspirational adjectives ("clean", "good") with concrete statements ("use file-scoped namespaces")
- Write in **bullet points**, not prose paragraphs
- Include the _why_ for non-obvious Avoid rules
- Do NOT include rules already enforced by `.editorconfig`, Roslyn analyzers, linters, or CI
- Do NOT include secrets, tokens, internal URLs, or sensitive details
- Do NOT paste large code blocks or third-party snippets
- Keep the file under ~200 lines — if it grows larger, split by concern

### Frontmatter rules

- `applyTo` must be a **single comma-separated string**, not a YAML list
- Patterns are **relative to the repo root** — no leading `./` or `/`
- Exclusion patterns (e.g. `!src/Legacy/**`) are **not supported** — use positive patterns only
- Case-sensitive — match the exact casing of repository paths

### Good vs. Bad examples

|❌ Bad|✅ Good|
|---|---|
|Write clean, maintainable code.|Use file-scoped namespaces. Prefer primary constructors for simple DI.|
|Follow best practices for error handling.|Use `Result<T>` from ErrorOr. Do not throw exceptions in application layer code.|
|Write good tests.|Test method names: `MethodName_Scenario_ExpectedResult`. Each test has Arrange/Act/Assert comments.|
|Use appropriate logging.|Use `ILogger<T>` via constructor. Log at `Information` for business events, `Warning` for recoverable errors.|
|Keep controllers thin.|Controllers must not contain business logic. Delegate to a service or MediatR handler immediately.|
