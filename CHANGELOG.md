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
