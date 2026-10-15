# Validation and handoff guide

For each change, choose checks that match its scope and state their result accurately.

- C# changes: validate compilation in the project's Unity version; run focused EditMode/PlayMode tests when gameplay rules changed and the Editor/test runner is available.
- Scene or prefab changes: open the affected scene/prefab in Unity, check the Console, references, missing components, and visible result; do not claim visual validation without opening it.
- Sprite slicing/import changes: check source dimensions against `Tilesheet.txt`, sample tiles at the sheet boundaries, transparency, orientation, pixels-per-unit, and filtering.
- Project/package changes: inspect the relevant manifest/lockfile and confirm Unity resolves packages before calling the project ready.
- Before handoff, inspect `git status` and the diff. Exclude generated folders and unrelated user changes; preserve `.meta` files for Unity assets.
- Report what changed, what now works, checks run, and any check that could not be run.
