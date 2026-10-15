# PixelShooter rules

This is the index of mandatory rules for agents working in this repository. `AGENTS.md` routes work here; it does not replace these rules. Read the rule file that matches the task before making changes.

| Rule file | Applies to |
|---|---|
| [`rules/agent-workflow.md`](rules/agent-workflow.md) | Every task: scope, inspection, actions, uncertainty, and reporting |
| [`rules/unity-project.md`](rules/unity-project.md) | Unity code, scenes, prefabs, packages, assets, and Editor/MCP operations |
| [`rules/git.md`](rules/git.md) | Git staging, commits, pushes, and preserving user work |

## Authority and terms

1. Follow platform and safety requirements first, then the user's current request, then these project rules, then project guides and existing implementation.
2. A current, explicit user request can override a project preference. It does not authorize unrelated work or override platform/safety requirements.
3. **MUST** and **MUST NOT** are binding. A rule can be relaxed only by a clear user instruction for the specific task, unless the higher-priority requirements above prohibit it.
4. If two applicable rules conflict, do not silently choose. Explain the conflict and ask one concise question before the blocked action; continue independent, safe inspection.
5. Project guides in `docs/agents/` give context and recommended patterns. They cannot weaken a rule in this directory.

## Enforcement

These files are mandatory instructions for agents, but Markdown alone cannot technically block an editor or Git operation. When a rule must be enforced regardless of agent compliance, add a project-owned automated check and run it in CI; do not claim that documentation provides a hard technical lock.

## Project state

The project currently has a Unity 2D URP baseline and a sample scene; do not treat proposed architecture in the guides as code that already exists. Inspect the repository and Editor state for each task. Keep rules specific to observed project facts and update this index when adding or moving a rule file.
