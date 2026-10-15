# Game architecture guide

Use a small, feature-oriented structure for this 2D pixel shooter. Keep gameplay rules understandable and testable without turning a small game into a framework.

## Suggested code layout

When gameplay code is added, start with:

```text
Assets/Scripts/
  Core/                 # shared primitives and project-wide contracts only
  Features/
    Player/
    Enemies/
    Weapons/
    Combat/
    UI/
  Infrastructure/      # Unity/platform adapters, save data, scene loading
```

Create a feature folder when its first real behavior is implemented; do not create empty folders for every possible system. Keep a feature's components, configuration, and focused tests together where practical.

## Responsibilities

- Feature code owns rules for that feature: movement, enemy behavior, firing, damage, health, and HUD behavior stay in their respective areas.
- Unity components adapt engine events and data to gameplay behavior. Avoid a single `GameManager` owning input, combat, spawning, audio, and UI.
- Use ScriptableObjects for authored, mostly immutable tuning data such as weapon or enemy definitions. Keep runtime health, cooldowns, and match state in runtime objects.
- Let scenes compose systems and references. Put reusable actors and UI elements in prefabs; avoid hiding important gameplay logic in scene-only wiring.
- Add interfaces, services, assembly definitions, or event buses only when there is a concrete need such as multiple implementations, a test seam, or a real dependency boundary.

## Change boundaries

Before adding a system, identify its owner feature, inputs, outputs, lifetime, and who creates it. Prefer direct references for simple local relationships; use events for meaningful decoupling, not for every method call.
