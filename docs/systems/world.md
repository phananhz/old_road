# World

- Version: 0.2
- Status: Draft
- Last updated: 2026-08-14
- Purpose: Define world exploration direction.

The world is semi-open and should be discovered through roads, landmarks, environmental clues, NPC dialogue, rumours, and old maps. Avoid excessive question-mark markers.

Possible regions include Valen, Old Forest, Blackwood, Riverlands, Marshlands, Northern Pass, Frosthold, Sun Coast, Ancient Ruins, and Old Capital.

The current prototype starts in Valen Outskirts and now supports chunk-streamed exploration beyond the original map.

## Current Prototype Map

Valen Outskirts is now generated at runtime as a wider deterministic procedural placeholder map, approximately 120 x 72 Unity units.
It contains:

- a winding old road through the center;
- a longer river section on the western/southern side with lightweight animated ripple overlays;
- darker forest edges to frame the playable area;
- seeded harvestable tree, stone, berry, herb, mushroom, and iron-ore nodes spread across the map;
- one-time loot chests placed near road, camp, shrine, ruin, and bell marker locations;
- prototype landmarks such as a waystone, signpost, ruined arch, footbridge, and abandoned camp.

The original Valen Outskirts map remains as the authored starting area. Beyond it, `InfiniteWorldStreamer` creates deterministic 32 x 32 Unity-unit chunks around the player and unloads far chunks. This is a prototype infinite-world approach: terrain, decorative resources, villages, and NPCs are regenerated from stable coordinates and seed values rather than authored as final content.

The current chunk-streaming prototype intentionally does not persist every streamed decoration or NPC. Persistent gameplay state still lives in the save system for the vertical-slice authored objects and player-built construction.

## Prototype Villages

Streamed chunks can contain small deterministic villages. Villages currently include placeholder houses, a storehouse, a campfire, a sign, and harmless NPC villagers. NPCs have visible job labels such as Miller, Woodcutter, and Herbalist and walk between local work points to make villages feel active.

## Landmark Discovery

Prototype landmarks use stable IDs and can be inspected with the same interaction input used by the gather prototype. Discovered landmarks are recorded in the Roadwarden Journal and saved with the rest of the vertical-slice state.

Current discoverable landmarks:

- Northern Waystone
- Old Road Sign
- Broken Watch Arch
- River Footbridge
- Abandoned Camp
- Eastern Bell Marker
- Hunter Shrine
- South Ruin Gate

## Prototype Loot

One-time loot chests use stable IDs and can be opened with the same interaction input used by the gather prototype. Current rewards are prototype values for testing exploration flow only:

- Roadside Cache: wood
- Abandoned Camp Chest: cabin plank
- South Ruin Chest: stone
- Hunter Shrine Cache: wood
- Bell Marker Cache: stone
- Forager's Hidden Pouch: medicinal herb
- Bridge Toll Box: old coin
- Collapsed Mine Crate: iron ore
- Camp Torch Bundle: torch

Opened chest state is saved with the rest of the vertical-slice state.

## Day and Night Prototype

The runtime world now applies a camera-following tint overlay based on the in-game clock. Daylight is strongest around midday, fades through evening, and becomes darkest overnight. This is a prototype rendering layer for readability testing; final production lighting can replace it later.

The `item.torch` prototype item enables a small warm glow around the player during darker hours.
