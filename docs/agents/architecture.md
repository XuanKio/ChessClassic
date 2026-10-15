# Game architecture guide

Use small Unity components with clear responsibilities. Focus on S (single responsibility) and O (extend through configuration and composition). Keep direct references for local relationships and events for state changes observed by UI or visuals.

See [the Vietnamese code guide](../code-guide.md) for reading order, component placement, and examples.

## Current structure

```text
Assets/_Game/
  Art/
    Sprites/          Source sprites, sheets and Tilesheet.txt metadata
    Environments/ Terrain atlas and Tile assets
    References/       Preview and Sample A/B images
  Audio/             Source OGG audio variations
  Fonts/                      Source fonts and HUD SDF font
  Materials/                  Characters, Environments and VFX materials
  Shaders/                    Shader source only
  Animations/Characters/      Character clips and Animator controller
  Scenes/                     Bootstrap.unity and SampleScene.unity
  Scripts/
    Bootstrap/                 Scene startup and build-scene dropdown
    Gameplay/
      Characters/              Input, movement, aim, health, player coordination
      Weapons/                 Weapon state, data, projectile creation, weapon visuals
      Combat/                  Projectile movement and damage contract
    Presentation/              World depth sorting
    UI/                        HUD and crosshair
  ScriptableObjects/
    Characters/                CharacterData assets
    Weapons/                   StarterWeaponData.asset
  Prefabs/Projectiles/          PlayerBullet.prefab
  Settings/Input/              PlayerControls.inputactions
  Settings/Rendering/          URP assets and scene template
```

## Component ownership

| Location | Components | Responsibility |
|---|---|---|
| Player | PlayerInputHandler, PlayerController | Read local Input Actions; coordinate movement, aim, fire and reload |
| Player | CharacterMovement, Rigidbody2D, CapsuleCollider2D | Accelerate/decelerate and interact with world collision |
| Player | CharacterHealth | Own health/death and publish damage/death events |
| Player | CharacterAim, WorldDepthSorting, SortingGroup | Apply aiming pose/flip and sort the character by foot Y |
| PlayerVisual | CharacterPresentation, Animator, SpriteRenderer | Idle/Run/Die and shader hit flash; never change physics |
| WeaponPivot | WeaponController, ProjectileSpawner | Own ammo/cooldown/reload; create configured projectile |
| WeaponVisual | WeaponVisual, SpriteRenderer | Flip weapon and mirror FirePoint |
| PlayerBullet prefab | ProjectileMovement, SpriteRenderer | Move, sweep blockers and expire; authored bullet appearance |
| HUDCanvas/CrosshairUI | CrosshairView | Follow local aim input and own cursor visibility |
| HUDCanvas/AmmoDisplay | AmmoDisplayView | Observe ammo/reload and update TMP text |
| HUDCanvas/HealthBar | HealthBarView | Observe health and clip the bar fill |
| Player/MoveDust | ParticleSystem (MoveDust.prefab) | Footstep dust: emits by distance travelled, so it is silent while standing still |

## Data and state

- CharacterData stores authored movement tuning.
- WeaponData stores shot interval, projectile speed/lifetime, magazine capacity, and reload duration. StarterWeaponData preserves the previous scene weapon's values.
- Current health, ammo, cooldown and reload timers belong to individual runtime components, never the shared ScriptableObjects.
- Projectile appearance, scale, sorting and blocker mask belong to PlayerBullet.prefab. ProjectileSpawner has a separate muzzle-clearance mask.
- The scene wires references. The current Player is a scene object, not a player prefab.

## Runtime flow

PlayerInputHandler clones PlayerControls per player, enables/disables its Gameplay map, and exposes Move/Aim/Fire/Reload. It reads before PlayerController through execution order -50. PlayerController forwards requests; WeaponController owns whether a shot/reload is allowed.

WeaponController asks ProjectileSpawner to create a projectile. A blocked muzzle consumes no ammo. At zero ammo, the last successful shot starts reload. R requests early reload. Reload blocks firing; holding fire resumes after completion. Disabling the controller clears movement input and cancels weapon actions; it does not permanently lock movement.

CharacterHealth initializes once on the first read or Awake, whichever comes first. This avoids a dependency on health execution order. On death PlayerController locks movement and cancels firing/reload; CharacterPresentation only responds visually. No respawn/reset lifecycle exists yet.

CharacterPresentation retains a local flag for the applied death visual; CharacterHealth is the gameplay source of truth. Hit flash changes _FlashAmount through MaterialPropertyBlock, with no overlay sprite. URP applies `SpriteRenderer.flipX/Y` in the vertex shader, so `SpriteHitFlash.shader` must keep `Core2D.hlsl` with `SetUpSpriteInstanceProperties()` + `UnityFlipSprite`; without it CharacterAim's flip is silently ignored.

ProjectileMovement currently blocks against the environment only. IDamageable is implemented by CharacterHealth; projectile-to-character damage is not wired yet. Gameplay authority is still offline/local, not server-authoritative.

## HUD and maps

HUDCanvas is a screen-space overlay. CrosshairView belongs only to the local HUD; remote actors must not own it. AmmoDisplayView subscribes to ammo/reload events and displays ... during reload. HealthBarView refreshes immediately when enabled.

Health bar frame/fill use the existing UI tile setup and RectMask2D. See the scene and [map notes](../prototype-arena.md) for asset details. BackgroundCanvas is camera-space behind the world; the map retains its perimeter collision and decoration layers.

## Movement dust

`Assets/_Game/Prefabs/VFX/MoveDust.prefab` (material `Art/VFX/MoveDust.mat`, Sprite-Lit-Default, untextured square quads) sits at the bottom edge of PlayerVisual (local y −0.46; the sprite ends at −0.48). It simulates in world space and uses only `rateOverDistance` (5 per unit), so particles trail behind the character and none spawn while idle; no script drives it. Particles are 2–4 art pixels (0.08–0.16 units; one art pixel is 0.04), start fully opaque white and fade to the pack's light lavender (195,198,233) over 0.35–0.55 s, spreading in a low 80° fan. They render at order −10 on the Player layer, behind PlayerVisual, inside the player's SortingGroup, so they never cover the character; the trail stays visible because world-space particles are left outside the silhouette. Emitting at the old feet offset (−0.26, inside the body) hides them completely. Reuse the prefab on other walking actors at their sprite's bottom edge.

## Bootstrap

Assets/_Game/Scenes/Bootstrap.unity is build index 0. GameBootstrap loads the enabled scene selected with the BuildScene dropdown. Pressing Play directly from SampleScene skips Bootstrap. Add global services only when they actually exist.

## Change boundaries

Prefer editing data or prefab composition before adding branches to controllers. Use existing health/ammo events for observers. Add an interface or inheritance only when a real second implementation needs it. Keep the existing ScriptableObject and prefab GUIDs when editing assets.
