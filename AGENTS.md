# PixelShooter agent guide

These instructions apply to the whole Unity project. Before changing a subsystem, read its matching guide in `docs/agents/`:

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
- Current gameplay baseline: `Assets/Scenes/SampleScene.unity`; inspect the scene before assuming gameplay systems exist.
- Repository: `XuanKio/PixelShooter`, branch `main`.

## Working rules

- Inspect the existing project and relevant guide before making changes. Keep each task focused and avoid introducing architecture for hypothetical features.
- Preserve Unity `.meta` files and GUIDs. Move or rename an asset together with its `.meta`; never regenerate a meta file for an existing asset.
- Keep original art/audio source files intact. Create derived assets in clearly named project-owned folders and keep a traceable link to their source.
- Do not commit generated Unity folders such as `Library`, `Temp`, or `Logs`, or machine-specific `.mcp.json` settings.
- Do not stage unrelated user changes. Review `git status` and stage only files needed for the requested task.
- Explain the change and what was checked in plain language. If Unity Editor validation was unavailable, say so rather than implying it passed.
