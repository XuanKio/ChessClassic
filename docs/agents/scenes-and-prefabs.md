# Scene and prefab guide

- Keep authored game scenes under `Assets/Scenes/`; the existing baseline is `SampleScene.unity`. Inspect it before changing or replacing it.
- Use descriptive scene names when a real screen or level is introduced, for example `MainMenu`, `Gameplay`, or `Level_01`; do not rename the current scene without updating build settings and references.
- Treat a scene as composition: it wires feature components, cameras, lighting, and UI. Put reusable player, enemy, weapon, and UI objects in prefabs rather than duplicating their setup across scenes.
- Keep prefab references local and intentional. Check prefab overrides and scene references after moving or renaming objects/assets.
- Before editing a scene, record the relevant existing hierarchy and settings. Preserve unrelated objects and save only the intended scene changes.
- Check `ProjectSettings/EditorBuildSettings.asset` whenever scenes are added, renamed, or removed.
