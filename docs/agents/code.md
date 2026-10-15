# C# gameplay code guide

- Put new runtime scripts under `Assets/_Game/Scripts/Gameplay/<Feature>/` and group by feature as described in `architecture.md`.
- Use clear names that match one responsibility. Keep `MonoBehaviour` classes focused on Unity lifecycle, component references, and translating engine events into feature behavior.
- Separate authored configuration from mutable runtime state. Do not use a ScriptableObject asset as a shared container for per-enemy or per-player runtime values.
- Make ownership and cleanup explicit for subscriptions, coroutines, spawned objects, and input callbacks. Unsubscribe or stop work when the owning object is disabled or destroyed.
- Keep serialized fields private with `[SerializeField]` when Inspector authoring is needed. Avoid public mutable fields and unexplained string-based object lookups.
- Keep pixels and world units distinct. Convert deliberately at the rendering/input boundary, and do not hard-code a scale until the project has a chosen Pixels Per Unit convention.
- Add tests for non-trivial rules and edge cases; do not add test scaffolding for trivial field forwarding. Follow `quality.md` for validation.
