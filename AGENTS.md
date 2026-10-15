# PixelShooter agent guide

These instructions apply to the whole Unity project. The agent's role is to coordinate the requested work: inspect the project, select the applicable rules, make only the authorized changes, and verify them.

## Required rules

Read `docs/RULES.md` before every task. Its linked files define mandatory project rules; follow every rule that applies to the task. If rules conflict or a required action is blocked, stop the dependent change and explain the conflict. The current user request takes precedence over project preferences, within platform and safety requirements.

## Project guides

Use the relevant guide in `docs/agents/` for project context and recommended patterns; guides explain the codebase and do not grant permission to widen task scope:

- Game structure and boundaries: `docs/agents/architecture.md`
- C# gameplay code: `docs/agents/code.md`
- Scenes and prefabs: `docs/agents/scenes-and-prefabs.md`
- Unity assets in general: `docs/agents/assets.md`
- The imported art/audio pack: `docs/agents/assetgame.md`
- Validation and handoff: `docs/agents/quality.md`

## Project facts

- Unity project root: this directory; the current Unity project folder is named `LTM`.
- Unity version: read `ProjectSettings/ProjectVersion.txt` before relying on editor-specific behavior.
- Rendering: 2D URP packages are installed.
- Current gameplay baseline: `Assets/_Game/Scenes/SampleScene.unity`; inspect the scene before assuming gameplay systems exist.
- Repository: `XuanKio/PixelShooter`, branch `main`.
