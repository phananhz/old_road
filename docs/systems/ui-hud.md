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
- center-bottom: contextual prompts for gathering, crafting, and building.
- `I`: inventory overlay with all current prototype item stacks.
- `M`: expanded map overlay with player, resources, construction sites, landmarks, road, and river markers.
- `Esc`: close overlays.

The visual skin uses dark panels, gold trim, parchment text, subtle shadows, corner accents, compact control pills, item glyphs, virtual joystick controls, and map legends to avoid covering the play field while looking closer to a real game HUD than a debug overlay.

Hotbar slots can be selected with number keys or by clicking/tapping directly on the slot.

The UI is implemented as runtime `OnGUI` prototype code so it can be iterated without hand-editing scene YAML.

## Current Hotbar Slots

1. Wood
2. Stone
3. Cabin Plank
4. Cabin/build plan
5-9. Reserved

This is not the final inventory UX. It is the first testable gameplay HUD.
