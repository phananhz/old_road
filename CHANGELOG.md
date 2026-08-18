# Changelog

## Unreleased

- Rebuilt project documentation to match the master prompt pack.
- Added required `Assets/Game` folder structure for art, audio, data, prefabs, scenes, scripts, and tests.
- Added `.editorconfig`.
- Reworked runtime architecture around `GameBootstrap` and `VerticalSliceController`.
- Added Unity Editor setup command to rebuild prototype data and Bootstrap scene through Unity APIs.
- Added stable IDs for item, resource node, building, and construction instance data.
- Added timestamp-based construction progress with `IClock`.
- Added versioned save/load repository with temp-file write and backup.
- Integrated Valen Outskirts vertical slice: move, gather, inventory, place cabin, construction progress, save/load.
- Added GAME-016: Handcrafted 16x16 pixel sprites for all 17 prototype items, character walk cycles, environment objects, and buildings with an automated PNG exporter pipeline, ScriptableObject sprite bindings, and HUD texture rendering.
- Added GAME-017: Combat Micro-Slice with real-time 2D directional attacks, weapon scaling, pixel-art slash VFX, Forest Wolf and Bandit Scout enemy AI, floating damage text, i-frames, loot drops, and food consumable healing.
