# World

- Version: 0.2
- Status: Draft
- Last updated: 2026-08-14
- Purpose: Define world exploration direction.

The world is semi-open and should be discovered through roads, landmarks, environmental clues, NPC dialogue, rumours, and old maps. Avoid excessive question-mark markers.

Possible regions include Valen, Old Forest, Blackwood, Riverlands, Marshlands, Northern Pass, Frosthold, Sun Coast, Ancient Ruins, and Old Capital.

The current prototype represents Valen Outskirts only.

## Current Prototype Map

Valen Outskirts is now generated at runtime as a wider deterministic procedural placeholder map, approximately 120 x 72 Unity units.
It contains:

- a winding old road through the center;
- a longer river section on the western/southern side;
- darker forest edges to frame the playable area;
- seeded harvestable tree and stone nodes spread across the map;
- one-time loot chests placed near road, camp, shrine, ruin, and bell marker locations;
- prototype landmarks such as a waystone, signpost, ruined arch, footbridge, and abandoned camp.

This is a finite procedural prototype, not an infinite Minecraft-style chunk world yet. These are still placeholder pixel-art assets. They exist to validate exploration scale, camera follow, minimap readability, gathering, and construction placement before production maps are authored.

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

Opened chest state is saved with the rest of the vertical-slice state.
