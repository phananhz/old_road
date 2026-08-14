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

The current vertical slice uses runtime-generated point-filtered placeholder sprites. These are not production assets. They exist to validate camera, scale, sorting, movement, harvesting, building placement, and construction progress before final art is imported.

Production art should eventually replace the placeholders under `Assets/Game/Art` and prefabs under `Assets/Game/Prefabs`.
