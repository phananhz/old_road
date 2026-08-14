# UI HUD

- Version: 0.1
- Status: Draft
- Last updated: 2026-08-15
- Purpose: Define the first playable HUD layout.

## Prototype Layout

The current prototype HUD uses a Minecraft-like readable layout with a medieval dark-fantasy skin:

- top-left: game title, player health, day/time, and save status;
- top-right: in-game day/time and minimap scaled to the expanded Valen Outskirts prototype map;
- bottom-center: 9-slot hotbar for key materials and build actions;
- bottom-left: virtual joystick for mobile-style movement testing;
- bottom-right: mobile-style action buttons for `Gather`, `Craft`, `Build`, `Bag`, `Map`, and `Log`;
- contextual mobile `F` action button for cabin `Enter`, `Exit`, and `Sleep` interactions;
- top-center: temporary contextual prompts for gathering, inspecting, crafting, and building.
- `I`: inventory overlay with all current prototype item stacks.
- `M`: expanded map overlay with player, resources, construction sites, landmarks, road, and river markers.
- `J`: Roadwarden Journal overlay with discovered landmark entries.
- `Esc`: close overlays.

The visual skin uses dark panels, gold trim, parchment text, subtle shadows, corner accents, compact control pills, item glyphs, virtual joystick movement controls, and map legends to avoid covering the play field while looking closer to a real game HUD than a debug overlay.

Hotbar slots can be selected with number keys or by clicking/tapping directly on the slot.

The mobile action buttons emit the same prototype input events as keyboard controls so gathering, crafting, building, inventory, map, and journal logic remain outside the UI layer.

Contextual prompt messages appear near the top of the screen and auto-hide after a short delay so they do not cover the hotbar.

Nearby interactable objects show a temporary glow outline while they are in interaction range.

Timed interactions draw a compact world-space countdown/progress bar above the target object. The current prototype uses this for gathering resources, opening loot chests, inspecting landmarks, and active construction sites.

The UI is implemented as runtime `OnGUI` prototype code so it can be iterated without hand-editing scene YAML.

## Current Hotbar Slots

1. Wood
2. Stone
3. Cabin Plank
4. Wild Berries
5. Medicinal Herb
6. Mushroom
7. Iron Ore
8. Torch
9. Cabin/build plan

This is not the final inventory UX. It is the first testable gameplay HUD.
