# Chess board scene

Version: 1.0.0. Presentation-only scene setup; no chess rules, opponent AI or networking.

Inputs: select a square, flip the board, reset the preview, previous/next sample move.
Outputs: selected-square text, board orientation and `c4` replay state. No domain events.
Dependencies: Unity uGUI; the scene EventSystem uses the Input System UI module.
Internal: 64 square buttons, 32 piece Images, coordinates, player labels, sample clocks
and move history. Default view is Black at the bottom after White's `c4`. Draw/resign
remain disabled until a game session is implemented. Replay covers that one sample move.

## Reskin contract

Artists can replace the 12 sprites in `Assets/Art/ChessCom/Pieces`, the board in
`Assets/Art/ChessCom/Board`, and icons in `Assets/Art/ChessCom/Icons` while preserving
their `.meta` files. Piece Images use preserve-aspect. Scene colors and text are
editable in the hierarchy. No Addressables or sprite atlas is configured.
The downloaded source files and their SHA-256 hashes are recorded in `sources.json`.
Layout or hierarchy changes require updating the editor scene builder.

Use `Chess Classic/Create Board Scene` to create the scene, or
`Chess Classic/Validate Board Scene` to check an existing scene. The builder refuses
to overwrite an existing scene; edit the saved scene normally in Unity.
