# Rules: Git and repository changes

These rules apply to all Git operations in this repository.

- **MUST** inspect `git status` before staging and again before finishing. Preserve pre-existing user changes, including newly imported `Assets/AssetGame` content when it is outside the task scope.
- **MUST** stage explicit paths that belong to the task. **MUST NOT** use `git add .`, `git add -A`, or equivalent bulk staging.
- **MUST** review the staged file list and diff before committing; scan staged text for accidental credentials or secrets.
- **MUST NOT** commit generated Unity data (`Library/`, `Temp/`, `Logs/`, `UserSettings/`) or machine-specific `.mcp.json` settings.
- **MUST** use a concise conventional commit message when the user has asked for a commit.
- **MUST NOT** push, force-push, rewrite shared history, or replace remote content unless the user has requested that operation or a still-applicable instruction in the current task authorizes it. Force-push and shared-history rewrites require explicit authorization for that operation.
- **MUST** verify the resulting commit and remote/branch state after a requested push. Report failures plainly and leave the user's local work intact.
