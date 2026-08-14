# UI HUD

- Version: 0.1
- Status: Draft
- Last updated: 2026-08-15
- Purpose: Define the first playable HUD layout.

## Prototype Layout

The current prototype HUD uses a Minecraft-like readable layout with a medieval dark-fantasy skin:

- title/start screen: blocks gameplay until the player starts the journey, hides the gameplay HUD behind it, and exposes Settings/Quit;
- top-left: game title, player health, day/time, and save status;
- top-right: in-game day/time and a local minimap centered around the player for chunk-streamed exploration;
- bottom-center: 9-slot hotbar for key materials and build actions;
- bottom-left: virtual joystick for mobile-style movement testing;
- bottom-right: mobile-style action buttons for `Gather`, `Craft`, `Build`, `Bag`, `Map`, and `Log`;
- contextual `Cook` action appears near completed campfires or cooking hearths when the player is close enough.
- `B` or the `Build` action opens the construction catalog before placement.
- the construction catalog contains a large build grid and a side category list for Housing, Fire & Light, and Animal Pens.
- contextual mobile `F` action button for cabin `Enter`, `Exit`, and `Sleep` interactions;
- a centered bed confirmation prompt that can be accepted with `Y`/`Enter` or the `Yes` button, and cancelled with `N`/`Esc` or the `No` button;
- top-center: temporary contextual prompts for gathering, inspecting, crafting, and building.
- `I`: inventory overlay with all current prototype item stacks.
- `M`: expanded map overlay with player, resources, construction sites, landmarks, road, and river markers.
- `J`: Roadwarden Journal overlay with discovered landmark entries.
- `Esc`: close overlays.

The runtime settings panel can be opened from the title screen or the in-game HUD. It supports Unity graphics quality selection, target frame rate selection including Unlimited, English/Vietnamese language selection, sound on/off, and master volume. Settings are saved locally through `PlayerPrefs` and applied through concrete Unity runtime settings: `QualitySettings.SetQualityLevel`, anti-aliasing, anisotropic filtering, shadow quality, LOD bias, disabled vSync, `Application.targetFrameRate`, the active localization runtime, and the Unity audio listener where available. The HUD and settings panel show measured FPS so frame-rate changes can be verified while testing.

Language switching is available while gameplay is paused in the settings panel. The current prototype localizes the title/settings panels plus the main gameplay HUD, hotbar controls, inventory/map/journal overlays, build catalog labels, key item names, and objective text.

The visual skin uses dark panels, gold trim, parchment text, subtle shadows, corner accents, compact control pills, item glyphs, virtual joystick movement controls, and map legends to avoid covering the play field while looking closer to a real game HUD than a debug overlay.

Hotbar slots can be selected with number keys or by clicking/tapping directly on the slot. The bag overlay uses compact square item cells with quantity badges to avoid text spilling outside slots. Build catalog cards can be selected by click or tap.

The build catalog now exposes multiple buildable prototype buildings: Cabin, Stone Cottage, Storage Shed, Campfire, Cooking Hearth, Small Animal Pen, and Long Animal Pen. Fire buildings support cooking and warm light; animal pens produce prototype animal-product items over time. If the player lacks required materials, the catalog does not enter placement mode and shows the missing materials.

The mobile action buttons emit the same prototype input events as keyboard controls so gathering, crafting, building, inventory, map, and journal logic remain outside the UI layer.

Contextual prompt messages appear near the top of the screen and auto-hide after a short delay so they do not cover the hotbar.

Nearby interactable objects show a temporary glow outline while they are in interaction range.

Timed interactions draw a compact world-space countdown/progress bar above the target object. The current prototype uses this for gathering resources, opening loot chests, inspecting landmarks, and active construction sites.

The UI is implemented as runtime `OnGUI` prototype code so it can be iterated without hand-editing scene YAML.

## Current Hotbar Slots

1. Wood
2. Stone
3. Cabin Plank
4. Worn Axe
5. Stone Pick
6. Wild Berries
7. Iron Ore
8. Torch
9. Bell Fragment

This is not the final inventory UX. It is the first testable gameplay HUD.
