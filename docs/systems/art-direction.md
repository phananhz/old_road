# Art Direction

- Version: 0.1
- Status: Draft
- Last updated: 2026-08-14
- Purpose: Define the visual direction for prototype and future authored art.

## Camera and Readability

The playable game uses a fixed orthographic top-down or top-down-oblique 2D view. The goal is similar readability to farming/adventure RPGs while keeping The Old Road's own medieval fantasy identity.

Sprites must remain readable on mobile screens. Player, resource nodes, construction sites, interactables, and exits should have clear silhouettes before decorative detail is added.

## Pixel-Art Target

The target mood is medieval dark fantasy with cozy human warmth:

- warm fireplaces, candles, lanterns, and shop interiors;
- cool forest shadows, ruins, roads, fog, and mountain silhouettes;
- old stone, aged wood, banners, iron tools, castles, and village structures;
- restrained color palettes with strong value contrast.

Reference images can guide mood, lighting, and density. Do not copy protected assets, compositions, logos, characters, or maps.

## Prototype Implementation

The vertical slice now features handcrafted 16x16 pixel sprites for all 17 prototype items, authored character walk cycles (player, 4 villager variants, 4 animal variants), environment features (trees, rocks, bushes, herbs, mushrooms, ore, chests, waystone, signs, arch, footbridge), buildings (cabin and 5 construction stages, campfire, cooking hearth, small/long animal pens, storage shed, stone cottage), and VFX (torch glow, smoke puff, solid pixel).

All sprites are exported to `Assets/Game/Art/` as PNG assets configured with:
- 16 Pixels Per Unit (PPU)
- Point (no filter) texture filtering for crisp pixel art
- Uncompressed RGBA32 format
- Custom Y-sort bottom pivots for characters and environment entities
- ScriptableObject asset bindings (`ItemDefinition.icon`, `BuildingDefinition.completeSprite`, `BuildingDefinition.stageSprites`)
- OnGUI and HUD rendering via `PrototypePixelArtFactory` and `ItemDefinition.Icon` textures.

Production art will follow this exact asset pipeline and directory structure under `Assets/Game/Art/`.

