# Rules: Unity project

These rules apply to C# code, scenes, prefabs, packages, imported assets, and Unity Editor operations.

## Project and architecture

- **MUST** inspect the current scene, scripts, package manifest, and relevant settings before changing them. The project is still a scaffold; do not assume unimplemented systems exist.
- **MUST** keep gameplay code feature-focused. Put each rule with its owning feature; keep Unity lifecycle/adaptation separate from reusable gameplay decisions where that separation is useful.
- **MUST NOT** introduce a catch-all `GameManager`, framework, service layer, event bus, assembly definitions, or dependency solely for hypothetical future flexibility. Add boundaries when a real use case justifies them.
- **MUST** keep scene/prefab assets responsible for composition and reusable object setup; avoid embedding unrelated gameplay systems in scene-only wiring.
- **MUST** keep authored configuration separate from mutable runtime state. Never store per-instance runtime state in a shared ScriptableObject asset.

## Unity asset identity and source art

- **MUST** preserve each existing `.meta` file and GUID. When moving/renaming a Unity asset, move its `.meta` with it and verify references afterward. Never regenerate a meta file for an existing asset.
- **MUST NOT** overwrite, delete, or regenerate imported source art/audio or extracted tiles unless that exact change is requested. Put project-authored derivatives in a clearly named project-owned location and keep their source identifiable.
- **MUST** read a category's own `Tilesheet.txt` before slicing it. Verify source dimensions, grid spacing, orientation, alpha, and boundary tiles; do not guess semantic names from numeric tile filenames.
- **MUST** inspect import settings and intended world scale before changing filtering, compression, or Pixels Per Unit broadly. Pixel art must retain crisp edges and transparency.

## Scenes, packages, and Editor

- **MUST** inspect and preserve unrelated scene hierarchy, object references, and prefab overrides. Update build settings when scenes are intentionally added, renamed, or removed.
- **MUST NOT** hand-edit serialized scene/prefab YAML when a connected Unity Editor or Unity MCP can perform and verify the requested operation. If Editor/MCP is unavailable, report that limitation before any risky serialization edit.
- **MUST NOT** add/update packages or change the Unity editor version unless required by the user request. Keep manifest and lockfile in sync and verify Unity resolves the package.
- **MUST** distinguish file-level checks from Editor validation. Do not claim a scene opens, compiles, or renders correctly unless that was actually checked in Unity.
