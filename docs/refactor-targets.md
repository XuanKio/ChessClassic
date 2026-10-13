# Chess scene architecture refactor

Baseline: `1fc8880`. Scope: preserve the interactive preview while separating the
board, HUD, data/rules and input orchestration. Full gameplay and networking are
future work.

`ChessBoardPreview` mixes board layout, highlights, sample-position changes,
selection state, clocks, player labels and HUD controls. `PreviewMoveHints` also
combines sample data creation with move geometry and king-safety rules.

Target boundaries:

- Domain: immutable board position and basic move rules; no Unity references.
- Application: session/view ports, render snapshots and presenter; no Unity references.
- Presentation: reusable BoardView and GameHUD prefabs; no references to each other.
- Infrastructure: sample session adapter, replaced later by a real session adapter.
- Bootstrap: one scene GameController constructs dependencies and owns their lifetime.

Migration checkpoints: identify; extract pure logic; add ports; add view adapters;
test pure behavior; wire scene/prefabs; document the contract. Use a manual
composition root for this small project instead of adding a dependency-injection
framework. Preserve the public behavior at each checkpoint.

Acceptance: matching visuals, 64 selectable squares, correct piece positions in
both orientations, dots below/around pieces, working HUD controls, zero missing
references or scripts, two self-contained prefab assets, tests of presenter
subscription lifetime and snapshot isolation.
