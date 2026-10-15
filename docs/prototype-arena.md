# Prototype arena

The arena is authored in `Assets/_Game/Scenes/SampleScene.unity` with Unity Tilemaps. It is not generated at runtime. Tile size is 0.64 world units, matching a 16 px terrain tile beside the existing 24 px character scaled by four.

## Source and corners

Source: `Assets/_Game/Art/Sprites/Tiles/Tilemap/tilemap.png`, 18 columns × 13 rows, 16 px tiles and 1 px gaps. Indices run left to right, top to bottom. The map uses a project-owned copy under `Assets/_Game/Art/Environments/`, sliced into full 16 px cells at 16 PPU with point filtering. Source PNGs and their import settings remain unchanged.

| Terrain | Top left / edge / right | Middle left / fill / right | Front left / edge / right |
|---|---|---|---|
| Purple elevation | 0 / 1 / 2 | 18 / 19 / 20 | 36 / 37 / 38 |
| Green elevation | 5 / 6 / 7 | 23 / 24 / 25 | 41 / 42 / 43 |
| Gray roof | 10 / 11 / 12 | 28 / 29 / 30 | 46 / 47 / 48 |
| Gray facade | 51 / 52 / 53 | 69 / 70 / 71 | 87 / 88 / 89 |

## Ownership

`PrototypeArena.prefab` lives in `Assets/_Game/Prefabs/Environments/`. Its hierarchy separates `01_Ground_Sand`, `02_Ground_Decoration_NoCollision`, `03_Elevations_NoClimb`, `04_Buildings_Solid`, `05_Obstacles_HardCover`, and `06_PerimeterFence_ClosedPlayableArea`. Each elevated region has separate front-face and top-surface Tilemaps. Occluding objects have SortingGroups.

| Physics category | Movement | Projectiles |
|---|---|---|
| Ground and decorative marks | Pass | Pass |
| `WorldTerrain`: elevations, building, perimeter | Block | Stop/destroy |
| `WorldObstacle`: trunks, rocks, crates, fences | Block at the footprint | Stop/destroy |

The player has a small foot collider and a Rigidbody2D with frozen Z rotation. Ground fills 64×48 cells, extending beyond the 38×24-cell fenced play area. Every future gameplay map keeps sand ground continuous under terrain, objects, and visible camera margins.

Elevated surfaces and the building are inaccessible; there is no climbing mechanic. Player and ground-level occluders share external sorting layer `WorldActors`. `WorldDepthSorting` updates the player's group from its foot Y coordinate; static groups use their base Y. Lower feet render later. The weapon stays inside the player's SortingGroup. Decoration on inaccessible upper surfaces uses `WorldCanopy` above its terrain. Global Light 2D includes the new sorting layers.

`ProjectileMovement` sweeps a small circle over each frame's movement against both blocking layers to catch thin fences. `WeaponController` checks the path from actor to muzzle, preventing shots through cover when the muzzle protrudes through it. This implements environment blocking, not character damage detection.

## Perimeter asset names

The original atlas uses auto-sliced sprite names that differ from the full-cell indices used by this map. `tilemap_178` is not `tile_0178.png`.

| Part | Original sprite name | Full-cell tile index |
|---|---|---|
| Upper left / rail / upper right | 178 / 180 / 181 | 177 / 178 / 179 |
| Left / right vertical post | 199 / 201 | 195 / 197 |
| Lower left / rail / lower right | 217 / 218 / 219 | 213 / 214 / 215 |

Use full 16×16 cells from the derivative atlas to avoid original auto-slices crossing one-pixel gutters. A Scene/Game View grid or gizmo can draw a line across terrain: compare a clean render before editing sprites to fix an apparent cut.

## Pixel camera and health initialization

Gameplay HUD is `HUDCanvas`. `AmmoDisplay/AmmoCountText` uses `AmmoDisplayView`, wired to the scene weapon and TMP label. WeaponController owns an initial capacity of 30 and current ammunition; successfully spawning a bullet consumes one round and emits OnAmmoChanged. Empty magazines automatically reload in 1.5 seconds; R requests an early reload unless already full or reloading. Shooting is blocked during reload, and holding Fire resumes after completion. Reserve ammunition is unlimited for this prototype. The view shows `...` during reload and subscribes to ammo/reload events while enabled; Start refreshes after weapon Awake.

Input is authored in `Assets/_Game/Settings/Input/PlayerControls.inputactions`, Gameplay map: Move (normalized WASD composite), Aim (pointer position), Fire (left/right mouse), Reload (R). PlayerInputHandler clones the asset per player, enables/disables only its own map, and publishes input before PlayerController using execution order -50. PlayerController forwards reload requests; WeaponController owns reload timing and ammo state. Assign the Input Actions field when creating another player.

The basic SampleScene now uses `BackgroundCanvas/Background_Tiled`: a camera-space UI Image using source `Assets/_Game/Art/Sprites/Tiles/Tiles/tile_0034.png`, Type Tiled, stretched anchors (0,0)–(1,1), raycast disabled. BackgroundCanvas sorts on Ground at -1000 behind world objects. `01_Ground_Sand` is disabled in this scene; the perimeter remains world geometry with its colliders. The background fills the viewport independently of arena size.

Set the URP Pixel Perfect Camera `cropFrame` directly to `None` and keep Camera viewport rect at (0,0,1,1) to fill Game View. Avoid the obsolete cropFrameX/cropFrameY/stretchFill setters: they can select Windowbox unexpectedly in this URP version.

SampleScene Main Camera uses the URP Pixel Perfect Camera at 25 assets PPU, reference resolution 750×420, with Upscale Render Texture enabled. Terrain (16 PPU at 0.64 scale) and player (100 PPU at 4 scale) both resolve to 25 pixels per world unit. Keep this ratio when adding art; arbitrary camera zoom may be rounded to the pixel grid.

CharacterHealth initializes once on the first read or Awake, whichever comes first, so Controller/Presentation can safely read it in OnEnable without forcing health execution order. See [the code guide](code-guide.md) for current component ownership and weapon prefab/data locations.
