# Chess board scene

Version: 1.1.0. Presentation-only scene with move hints; no game session, AI or networking.

Inputs: select a square, flip the board, reset the preview, previous/next sample move.
Outputs: selected-square text, move hints, board orientation and `c4` replay state.
No domain events; selecting a target does not execute a move.
Dependencies: Unity uGUI; the scene EventSystem uses the Input System UI module.
Internal: 64 square buttons, 32 piece Images, coordinates, player labels, sample clocks
and move history. Default view is Black at the bottom after White's `c4`. Draw/resign
remain disabled until a game session is implemented. Replay covers that one sample move.

Selecting either color shows legal destinations for the displayed sample position:
dots on empty targets and rings on capture targets. Clicking an empty square or the
same piece clears selection; replay/reset clears hints and flipping keeps them aligned.
`PreviewMoveHints` models only the initial position and the sample `c4` position,
including blockers and king safety. Neither position allows castling or en passant;
these special moves and move execution belong to a future game-session implementation.
`MoveHintGraphic` draws non-raycast vector markers at runtime. The three yellow
highlight objects render before the Squares layer, so they never tint the pieces.

## Reskin contract

Artists can replace the 12 sprites in `Assets/Art/ChessCom/Pieces`, the board in
`Assets/Art/ChessCom/Board`, and icons in `Assets/Art/ChessCom/Icons` while preserving
their `.meta` files. Piece Images use preserve-aspect. Scene colors and text are
editable in the hierarchy. No Addressables or sprite atlas is configured.
The downloaded source files and their SHA-256 hashes are recorded in `sources.json`.
Edit `Assets/Scenes/ChessGame.unity` directly. Preserve the serialized references on
`ChessBoardPreview` and the button event bindings when changing the hierarchy.
