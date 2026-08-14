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

Current prototype visual upgrades:

- the player has a simple four-frame walk cycle and horizontal flip for left/right movement;
- the completed cabin sprite has a more detailed medieval pixel-art silhouette with roof, windows, door, stone base, chimney, and wood planks;
- the cabin interior is divided into sleeping, living, and kitchen areas with visible partition walls and room-specific furniture.
- completed fires use a tight animated warm glow, flickering flame, ember pulse, and rising smoke;
- completed houses use small warm night window glow and chimney smoke so the house area remains readable in very dark nights without lighting the whole map.

Production art should eventually replace the placeholders under `Assets/Game/Art` and prefabs under `Assets/Game/Prefabs`.
