---
name: xuan-game-ui
description: Xuan's pixel-art game UI builder for this Unity project. Give it a preview image (Sample A/B.png, Preview.png or an attached crop) and it finds which AssetGame tiles the preview is made of and where each one sits, rebuilds that UI under a canvas via Unity MCP (health bars, segmented meters, frames, panels, icons), makes fills shrink with health, and checks the result pixel by pixel with an off-screen render. Invoke as /xuan-game-ui <preview-image> [region or description] [canvas].
---

# /xuan-game-ui: build game UI from a preview

Turns a preview image into Unity UI made of the pack's own tiles, placed pixel for pixel. Worked example: the player `HealthBar` under `HUDCanvas` in `SampleScene`. See `docs/code-guide.md` and `docs/agents/architecture.md` for current component ownership.

**Before anything else:** read `docs/RULES.md` and its rule files. They bind this skill: never overwrite source art or re-slice tiles unasked, ask before saving a scene that has changes you did not make, and report only checks you actually ran.

Tools: `python -I .claude/skills/xuan-game-ui/scripts/pixels.py` (run with no arguments for usage) · template `references/build_from_match.cs` for `mcp__UnityMCP__execute_code`.

## 1. Read the preview

1. Find the image's pixel scale: `pixels.py scale <img> <row>`. The smallest repeated run length is the scale (Sample B and a pasted crop were both ×2).
2. If the user named an area ("thanh máu góc trái"), locate it. Dump the image at native resolution with `pixels.py region <img> x y w h <scale>` and pick out the widget's box. Image analysis follows the session's image-routing rule; the pixel dump is the source of truth for sizes.

## 2. Find the tiles the preview is made of

```
pixels.py match <img> <x> <y> <w> <h> <scale> Assets/_Game/Art/Sprites/<Category>/Tiles [threshold] --json
```

The output lists `{tile, x, y, score}`, where x/y are preview-native pixels at the tile PNG's top-left. Notes from real runs:

- The preview places tiles **off the 16 px grid, with overlaps**. The red health bar is 139 / 141 / 141 / 142 at x = 12 / 22 / 38 / 48, which is how its segments come out even at 14 px. Rebuild with exactly these positions; do not re-grid.
- Score 1.0 means every opaque pixel of the tile matches. Tiles drawn over each other only match where they show, so lower the threshold (0.9) when the preview stacks layers, and check the leftovers with a `region` dump.
- Group the hits into widgets by row (y) and make one build per widget. Text such as "100%" matches number tiles (93/94); leave it out unless the user asks for it.

## 3. Decide what moves

Ask yourself, or the user via `AskUserQuestion` when it's ambiguous:

- Which tiles are **fill that shrinks** (layer 1) and which are **static** (layer 0)?
- Fill tiles 139–142 (red) and 157–160 (blue) are "frame + fill" tiles. The template draws the matching empty frame (121–124) over each one at the same position, so the fill can shrink under an intact frame. The `frameFor` table holds this mapping; extend it if you meet a new family, after checking with `pixels.py tiles` that the outlines are identical.
- A preview of a partly filled bar (fill ending in an end cap, such as 160) shows one state of the same widget. Build it from the full state.

## 4. Render-check off screen, then build

Fill the template's CONFIG block (`entries` as tile, x, y, layer) and run it through `execute_code`:

1. `renderCheck = true`: builds the widget in a temporary additive scene, renders `Temp/Captures/<widget>_<pct>.png` at 100 / 60 / 1 % with a private camera, then closes the scene unsaved. Dump each file with `pixels.py region <png> 16 16 <w×3> <h×3> 3` and compare it with the preview dump. These renders are pixel-exact whatever the Game view's zoom.
2. `renderCheck = false`: builds under `canvasName` in the open scene, top-left by default (`marginX`/`marginY`, `uiScale` 3), and wires `HealthBarView` (`Assets/_Game/Scripts/UI/`) to the player's `CharacterHealth` when the widget has a fill. For another data source, write a small view in the same style: subscribe in `OnEnable`, read the first value in `Start`, resize only the mask.

Why the template is built this way:

- **Sprite rects are read at run time.** Tiles use Multiple mode with automatic trimming, so a sprite is often smaller than its 16×16 PNG (16×14, 12×16 and so on). The template places each Image over the sprite's exact rect inside the PNG, so trimmed pieces still line up. **Never re-slice the tiles to "fix" sizes.** Existing UI depends on the current rects; changing them broke the health bar once and had to be rolled back.
- The fill sits in a `RectMask2D`. The view resizes it to `inset + Ceil(fraction × (width − 2·inset))`, so any remaining value shows at least 1 px. `frameInset` is 3 for this pack (1 white + 2 dark outline pixels).
- Don't touch the canvas scaler, because other UI depends on it. Pixel scale comes from the widget root's integer `localScale`.
- CodeDom compiles C# 6: no tuples or local functions, and write `UnityEngine.Object` explicitly, because a bare `Object` is ambiguous.

## 5. Verify in the game and hand over

1. Read the console after any script change.
2. To check gameplay: enter Play mode, call `refresh_unity wait_for_ready=true`, and confirm `EditorApplication.isPlaying` **before** calling `TakeDamage`. Calls made during the domain reload run in edit mode, where `Awake` hasn't run and health reads 0. Game-view screenshots (`manage_camera screenshot`, `output_folder: Temp/Captures`) are resampled unless the view is at integer scale; check with `pixels.py scale` before reading them pixel by pixel. Use the off-screen render from step 4 for pixel claims.
3. `manage_scene get_active`: if `isDirty` is true from changes that aren't yours, ask before saving, because saving writes them too. Other people edit this scene in parallel, so look up the canvas by its current name and don't assume it.
4. Record the widget's tiles, positions and size in `docs/agents/architecture.md`, then report which checks ran and which did not.
