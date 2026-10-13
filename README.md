# ChessClassic

Unity **6000.0.79f1** project in `Chess/`.

## Open the board

1. Open `Chess/` in Unity Hub.
2. Open `Assets/Scenes/ChessGame.unity`.
3. Press Play. Click a square to inspect coordinates; use the bottom-right arrows to
   flip/reset the board, and the playback buttons to view the sample `c4` move.

The scene follows the supplied in-game screenshot: an 8×8 board on the left, Black at
the bottom, player bars and two clocks, with **Chơi / Các nước đi** on the right.
The New Game, Games and Players tabs are omitted. It opens on the sample `c4` position.
This is an interactive UI preview; clocks are sample values, draw/resign are disabled,
and chess movement rules, AI and networking are not implemented yet.

## Assets and editing

- `Assets/Art/ChessCom/Pieces`: twelve transparent piece sprites.
- `Assets/Art/ChessCom/Board`: downloaded green board texture.
- `Assets/Art/ChessCom/Icons`: gameplay icons, with SVGs in `Source/`.
- `Assets/Art/ChessCom/Avatars`: the site's two generic player avatars.
- `Assets/Art/ChessCom/sources.json`: source URLs, retrieval date and original SHA-256 hashes.

The source assets were downloaded from the resources loaded by
[Chess.com Play](https://www.chess.com/play) on 2026-10-13. They retain their original
ownership; this repository does not grant a license to those third-party assets.
The PNG icons are rasterized copies of the downloaded SVGs.

Edit the saved scene directly in the Unity hierarchy. The board interaction component
`ChessBoardPreview` lives in `Assets/Chess/Presentation`; preserve its serialized
references when changing the hierarchy. The one-time scene builder has been removed.

## Unity MCP

MCP for Unity is pinned in the package manifest. Open **Window → MCP for Unity** and
start/connect the local HTTP server at `http://127.0.0.1:8080`. The Codex MCP endpoint
is `http://127.0.0.1:8080/mcp`. Machine-specific configuration is kept outside Git.
