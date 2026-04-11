# Branch Naming Conventions Reference

## Format

```
{type}/{author}/{description}
```

## Section 1: Type (Required)

|Prefix|Alias|Purpose|
|---|---|---|
|`feature/`|`feat/`|New feature development|
|`bugfix/`|`fix/`|Bug fixes|
|`hotfix/`|—|Critical production fixes|
|`release/`|—|Release preparation|
|`docs/`|—|Documentation changes|
|`test/`|—|Proof-of-concept / experiments|

- Single word, lowercase, no spaces.

## Section 2: Author (Required)

- Initials (e.g., `jd`) or hyphenated full name (e.g., `john-doe`).
- Lowercase only.
- No spaces; use hyphens between first and last name.
- Ask the user for their author identifier every session.

## Section 3: Description (Required)

- Start with ticket reference: `issue-###` format (e.g., `issue-432`).
- Use `no-ref` when no ticket exists (e.g., `no-ref-critical-payment-bug`).
- Kebab-case (hyphens between words), no spaces or underscores.
- Keep it clear and concise — avoid generic terms like `update`, `changes`, `stuff`.

## Character Rules

- All lowercase.
- Alphanumeric characters, forward slashes (`/`), and hyphens (`-`) only.
- **Exception:** Release branches may include dots (`.`) for version numbers.

## Valid Examples

- `feature/jd/issue-456-user-authentication`
- `bugfix/sarah-jones/issue-789-fix-login-redirect`
- `hotfix/mk/no-ref-critical-payment-bug`
- `release/js/no-ref-v2.1.0`
- `docs/ab/issue-234-update-api-documentation`
- `test/jd/no-ref-redis-caching-experiment`

## Invalid Examples

- `feature/new-stuff` — missing author, vague description
- `Feature/JD/add_login` — uppercase, underscores
- `jd-feature-login` — wrong format
- `feature/jd/login page with spaces` — contains spaces
- `fix-bug` — missing author, missing structure
