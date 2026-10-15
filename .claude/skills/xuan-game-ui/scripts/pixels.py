"""Pixel inspection for pixel-art UI work. Prints character maps so layers can be compared by eye.

Usage (run with `python -I`):
  pixels.py tiles <tiles_dir> <n> [<n> ...]          dump tile_####.png maps + opaque row/col bounds
  pixels.py scale <image> [<row>]                    run lengths along a row, to find an upscaled image's pixel size
  pixels.py region <image> <x> <y> <w> <h> [<step>]  dump a region, sampling every <step> pixels (step = image scale)
  pixels.py bands <tiles_dir> <first_row> <last_row> list tiles whose coloured (non-outline) rows span exactly that range
  pixels.py match <image> <x> <y> <w> <h> <scale> <tiles_dir> [threshold] [--json]
                                                     find which tiles (and where) a preview region is built from

Colours are lettered per call; the legend is printed after each map. '.' is transparent.
Outline colours are taken from the Interface pack: white (255,255,255) and dark (71,50,75).
"""
import glob
import os
import sys

from PIL import Image

OUTLINE = {(255, 255, 255), (71, 50, 75)}


def letter(palette, rgb):
    if rgb not in palette:
        index = len(palette)
        if index >= 52:
            return "#"  # out of letters: background noise, photos, blended edges
        palette[rgb] = chr(ord("A") + index) if index < 26 else chr(ord("a") + index - 26)
    return palette[rgb]


def dump(image, xs, ys, palette):
    for y in ys:
        row = ""
        for x in xs:
            pixel = image.getpixel((x, y))
            row += "." if len(pixel) == 4 and pixel[3] == 0 else letter(palette, pixel[:3])
        print(row)


def tile_path(tiles_dir, n):
    return os.path.join(tiles_dir, f"tile_{int(n):04d}.png")


def cmd_tiles(tiles_dir, *numbers):
    for n in numbers:
        image = Image.open(tile_path(tiles_dir, n)).convert("RGBA")
        palette = {}
        rows = [y for y in range(image.height) if any(image.getpixel((x, y))[3] for x in range(image.width))]
        cols = [x for x in range(image.width) if any(image.getpixel((x, y))[3] for y in range(image.height))]
        print(f"tile {n} size={image.size} opaque rows {rows[0]}..{rows[-1]} cols {cols[0]}..{cols[-1]}" if rows else f"tile {n} empty")
        dump(image, range(image.width), range(image.height), palette)
        print(palette, "\n")


def cmd_scale(path, row=None):
    image = Image.open(path).convert("RGB")
    y = int(row) if row is not None else image.height // 2
    runs, current, count = [], image.getpixel((0, y)), 0
    for x in range(image.width):
        pixel = image.getpixel((x, y))
        if pixel == current:
            count += 1
        else:
            runs.append(count)
            current, count = pixel, 1
    print(f"size={image.size} row={y} runs={runs[:60]}")
    print("Smallest common run length is usually the scale.")


def cmd_region(path, x, y, w, h, step=1):
    image = Image.open(path).convert("RGBA")
    x, y, w, h, step = int(x), int(y), int(w), int(h), int(step)
    palette = {}
    # Sample the middle of each scaled pixel to avoid edge blending.
    offset = step // 2
    dump(image, range(x + offset, x + w, step), range(y + offset, y + h, step), palette)
    print(palette)


def cmd_bands(tiles_dir, first_row, last_row):
    first_row, last_row = int(first_row), int(last_row)
    for path in sorted(glob.glob(os.path.join(tiles_dir, "tile_*.png"))):
        image = Image.open(path).convert("RGBA")
        rows = [
            y for y in range(image.height)
            if any(image.getpixel((x, y))[3] and image.getpixel((x, y))[:3] not in OUTLINE for x in range(image.width))
        ]
        if rows and rows[0] == first_row and rows[-1] == last_row:
            centre = image.getpixel((image.width // 2, (first_row + last_row) // 2))
            print(os.path.basename(path), f"rows {first_row}..{last_row}", "centre", centre[:3])


def cmd_match(path, x, y, w, h, scale, tiles_dir, *options):
    """Find which tiles a preview region is built from. Prints matches as preview-native pixel offsets."""
    import json
    import numpy as np
    from numpy.lib.stride_tricks import sliding_window_view

    as_json = "--json" in options
    numbers = [o for o in options if o != "--json"]
    threshold = float(numbers[0]) if numbers else 0.97
    x, y, w, h, scale = int(x), int(y), int(w), int(h), int(scale)
    image = Image.open(path).convert("RGB")
    offset = scale // 2
    region = np.array(image)[y + offset:y + h:scale, x + offset:x + w:scale].astype(np.int16)
    covered = np.zeros(region.shape[:2], dtype=bool)

    candidates = []
    for tile_file in sorted(glob.glob(os.path.join(tiles_dir, "tile_*.png"))):
        tile = np.array(Image.open(tile_file).convert("RGBA")).astype(np.int16)
        mask = tile[..., 3] > 0
        opaque = int(mask.sum())
        th, tw = mask.shape
        if opaque < 8 or region.shape[0] < th or region.shape[1] < tw:
            continue
        windows = sliding_window_view(region, (th, tw, 3))[..., 0, :, :, :]
        equal = np.all(windows == tile[..., :3], axis=-1) & mask
        score = equal.sum(axis=(-1, -2)) / opaque
        for ry, rx in zip(*np.nonzero(score >= threshold)):
            candidates.append((opaque, float(score[ry, rx]), os.path.basename(tile_file), int(rx), int(ry), mask))

    # Greedy cover: biggest tiles first, keep a match only if it explains pixels nothing else has.
    candidates.sort(key=lambda c: (-c[0], -c[1]))
    picked = []
    for opaque, score, name, rx, ry, mask in candidates:
        th, tw = mask.shape
        area = covered[ry:ry + th, rx:rx + tw]
        new = int((mask & ~area).sum())
        if new >= max(4, opaque // 4):
            area |= mask
            picked.append((ry, rx, name, score, new))
    picked.sort()
    if as_json:
        tiles = [{"tile": int(name[5:9]), "x": rx, "y": ry, "score": round(score, 3)} for ry, rx, name, score, _ in picked]
        print(json.dumps({"width": int(region.shape[1]), "height": int(region.shape[0]), "tiles": tiles}))
        return
    print(f"region native size {region.shape[1]}x{region.shape[0]}, {len(candidates)} raw matches, {len(picked)} kept")
    print("x,y = top-left in preview-native pixels. score = share of the tile's opaque pixels that match exactly.")
    for ry, rx, name, score, new in picked:
        print(f"  {name}  x={rx:<4} y={ry:<4} score={score:.2f}  new_pixels={new}")
    print(f"uncovered non-background pixels: see 'region' dump; covered {int(covered.sum())} px")


COMMANDS = {"tiles": cmd_tiles, "scale": cmd_scale, "region": cmd_region, "bands": cmd_bands, "match": cmd_match}

if __name__ == "__main__":
    if len(sys.argv) < 2 or sys.argv[1] not in COMMANDS:
        print(__doc__)
        sys.exit(2)
    COMMANDS[sys.argv[1]](*sys.argv[2:])
