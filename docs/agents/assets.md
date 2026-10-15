# Unity asset handling guide

- Keep Unity asset files and their `.meta` sidecars together. The `.meta` GUID is part of the asset's identity and may be referenced by scenes, prefabs, and other assets.
- Do not hand-edit or regenerate existing `.meta` files. For moves/renames, move the sidecar with the asset and verify Unity references remain valid.
- Separate source/imported material from project-authored derivatives. Store generated sprites, atlases, prefabs, and configuration outside the source pack, with names that show their purpose.
- For pixel art, preserve transparency and sharp edges. Use point filtering where appropriate, avoid lossy compression, and verify Pixels Per Unit and sprite bounds against the intended world scale before changing import settings broadly.
- Prefer Unity's Sprite Editor slicing for a source sheet when individual sprites are needed. Keep the sheet as the source of truth, use the provided grid/padding metadata, and verify the first, last, and boundary sprites visually.
- Do not infer a tile's gameplay meaning from its numeric filename alone. Confirm it in the source sheet or preview, then document any mapping used by code or tilemaps.
