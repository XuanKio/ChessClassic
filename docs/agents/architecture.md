# Game architecture guide

Use a small, feature-oriented structure for this 2D pixel shooter. Keep gameplay rules understandable and testable without turning a small game into a framework.

## Current code layout

The player prototype currently uses the project-owned `_Game` layout:

```text
Assets/_Game/
  Animations/Characters/
  ScriptableObjects/Characters/
  Scripts/Gameplay/
    Characters/
    Combat/
    Weapons/
```

Keep new runtime code in the matching feature folder under `Assets/_Game/Scripts/Gameplay/`. Create a feature folder when its first real behavior is implemented; do not create empty folders for every possible system. Keep authored character data in `ScriptableObjects/Characters` and character clips/controllers in `Animations/Characters`.

## Responsibilities

- Feature code owns rules for that feature: movement, enemy behavior, firing, damage, health, and HUD behavior stay in their respective areas.
- Unity components adapt engine events and data to gameplay behavior. Avoid a single `GameManager` owning input, combat, spawning, audio, and UI.
- Use ScriptableObjects for authored, mostly immutable tuning data such as weapon or enemy definitions. Keep runtime health, cooldowns, and match state in runtime objects.
- Let scenes compose systems and references. Put reusable actors and UI elements in prefabs; avoid hiding important gameplay logic in scene-only wiring.
- Add interfaces, services, assembly definitions, or event buses only when there is a concrete need such as multiple implementations, a test seam, or a real dependency boundary.

## Implemented player prototype

`SampleScene` composes the current player prototype. The root `Player` owns input, movement, aiming, and the Rigidbody2D. `PlayerVisual` owns the Animator, character SpriteRenderer, presentation behavior, and a child `HitFlashOverlay`. `WeaponPivot` owns firing; its `WeaponVisual` child owns the weapon SpriteRenderer, flip behavior, and `FirePoint`.

| Component | Responsibility |
|---|---|
| `PlayerInputHandler` | Reads WASD, mouse screen position, and mouse-button firing input. |
| `PlayerController` | Passes input to movement, aim, and weapon components. |
| `CharacterMovement` | Applies acceleration/deceleration and speed through Rigidbody2D. |
| `CharacterData` | Authored move speed, acceleration, and deceleration values. |
| `CharacterAim` | Aims at the mouse and flips character/weapon orientation together. |
| `CharacterPresentation` | Drives Idle/Run from Rigidbody2D velocity, exposes hit flash and death calls. |
| `WeaponController` | Spawns the configured projectile at `FirePoint`. |
| `WeaponVisual` | Flips the weapon sprite and mirrors the fire point when aiming left. |
| `ProjectileMovement` | Moves the projectile and removes it after its lifetime. |

The player Animator uses `IsMoving` and `IsDead` bool parameters. `Idle` holds `tile_0000`; looping `Run` cycles `tile_0000` through `tile_0002`; `Die` holds `tile_0003`. `CharacterPresentation.PlayHitFlash()` briefly enables a white sprite overlay. `CharacterPresentation.SetDead()` triggers `Die` and disables the Rigidbody2D. Call these methods from a future health/damage owner; the prototype does not yet include a damage event or health system.

## Change boundaries

Before adding a system, identify its owner feature, inputs, outputs, lifetime, and who creates it. Prefer direct references for simple local relationships; use events for meaningful decoupling, not for every method call.
