# AssetGame pack guide

`Assets/AssetGame/` is the imported game art/audio pack. Treat its files as source material; do not overwrite the originals when creating game-ready assets.

## Contents

- `PNG/Enemies/`, `PNG/Players/`, `PNG/Weapons/`, `PNG/Interface/`, and `PNG/Tiles/` each contain a `Tilesheet.txt`, a source sheet in `Tilemap/`, and extracted images in `Tiles/`.
- Each `Tilemap/` currently has `tilemap.png` and `tilemap_packed.png`. Check which one is the original grid before slicing; the packed variant has no one-pixel grid gaps.
- `Sounds/` contains OGG sound variations for actions such as movement, jumping, shooting, damage, explosions, coins, and UI errors. Preserve variations and source filenames unless a reviewed mapping is added.
- `Preview.png`, `Sample A.png`, and `Sample B.png` are visual references for identifying intended art and layout.

## Verified sheet metadata

The `Tilesheet.txt` files report tile size and a one-pixel gap. The source `tilemap.png` dimensions match those values with no outer margin:

| Category | Tile size | Grid | Gap | Source sheet size | Tiles |
|---|---:|---:|---:|---:|---:|
| Enemies | 24×24 px | 4×4 | 1×1 px | 99×99 px | 16 |
| Players | 24×24 px | 4×4 | 1×1 px | 99×99 px | 16 |
| Weapons | 24×24 px | 10×4 | 1×1 px | 249×99 px | 40 |
| Interface | 16×16 px | 18×11 | 1×1 px | 305×186 px | 198 |
| Tiles | 16×16 px | 18×13 | 1×1 px | 305×220 px | 234 |

The packed dimensions correspond to the tile grid without the gaps: respectively 96×96, 96×96, 240×96, 288×176, and 288×208 px.

## Slicing and naming

- Read that category's `Tilesheet.txt` before slicing or reconstructing its grid. The metadata is specific to each sheet; do not reuse another category's settings.
- For a zero-based column `c` and row `r`, the top-left pixel of a tile is `x = c × (tileWidth + gapX)`, `y = r × (tileHeight + gapY)` for these verified source sheets. Outer margin is currently zero. Unity Sprite Editor coordinates and exported-image coordinates use different vertical origins; verify orientation before writing extraction code.
- Preserve both the source sheet and existing extracted tiles. Do not regenerate `Tiles/` or change the importer type without checking `.meta`, references, and current usage.
- `tile_####.png` is an extraction index, not a semantic name. Only add names such as `enemy_walk_01` after visually confirming the tile and recording its original index.
- If import settings or extraction results change, document the category, source sheet, grid, and reason so the change can be repeated.
