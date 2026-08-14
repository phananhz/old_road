# Valen Outskirts Vertical Slice

- Version: 0.2
- Status: Draft
- Last updated: 2026-08-14
- Purpose: Define the first end-to-end playable target.

## Target Flow

Player launches game -> walks around Valen Outskirts -> finds Tree -> harvests Tree -> Wood added to inventory -> finds Rock -> harvests Rock -> Stone added to inventory -> enters Building Mode -> positions Cabin -> placement validates -> confirms construction -> resources are deducted -> construction begins -> player can continue exploring -> game saves -> player exits -> player returns later -> save loads -> offline elapsed construction time is calculated -> cabin remains in correct state -> cabin eventually completes.

## Acceptance Criteria

- VS-AC-001: Bootstrap scene opens and creates a playable runtime world.
- VS-AC-002: Player moves in 8 directions with keyboard input.
- VS-AC-003: Tree can be harvested once and rewards `item.wood`.
- VS-AC-004: Rock can be harvested once and rewards `item.stone`.
- VS-AC-005: Inventory is visible in the development HUD.
- VS-AC-006: Player can enter and cancel building placement mode.
- VS-AC-007: Cabin preview follows the grid.
- VS-AC-008: Invalid placement cannot be confirmed.
- VS-AC-009: Valid placement creates a construction job.
- VS-AC-010: Construction deducts exact required resources.
- VS-AC-011: Construction visual stage is derived from progress.
- VS-AC-012: Save file persists inventory, harvested resources, and construction jobs.
- VS-AC-013: Reload recalculates elapsed construction time from timestamps.
- VS-AC-014: Camera follows the player while keeping a fixed orthographic orientation.
- VS-AC-015: Nearby gatherable resources show a clear interaction hint and glow outline.
- VS-AC-016: Player can craft one Cabin Plank from Wood using the prototype crafting recipe.
- VS-AC-017: HUD shows health, hotbar, minimap, and in-game time in a Minecraft-like layout.
- VS-AC-018: Player can open and close inventory and expanded map overlays.
- VS-AC-019: Valen Outskirts provides a wider seeded prototype exploration area with many resource nodes and visible landmarks.
- VS-AC-027: The prototype map is generated from a deterministic seed so layout remains repeatable during testing.
- VS-AC-020: Player can move with the on-screen virtual joystick.
- VS-AC-021: Player can select hotbar items by clicking or tapping slots.
- VS-AC-022: Player can trigger gather, craft, build, inventory, and map through on-screen mobile action buttons.
- VS-AC-023: Player can inspect landmarks and add them to the Roadwarden Journal.
- VS-AC-024: Discovered landmark state persists through reload.
- VS-AC-025: HUD shows objective progress for landmark inspection, gathering, crafting, and cabin construction.
- VS-AC-026: Nearby interactable objects show a glow outline while in range.
- VS-AC-028: Player can open one-time loot chests and receive prototype item rewards.
- VS-AC-029: Opened loot chest state persists through reload.
- VS-AC-030: Gathering, chest opening, and landmark inspection show a short countdown bar above the target and complete only after the countdown finishes.
- VS-AC-031: Active construction sites show a progress bar above the building footprint until construction completes.
- VS-AC-032: The prototype includes varied resource and loot item types beyond wood and stone.
- VS-AC-033: The river shows visible animated ripple motion.
- VS-AC-034: A completed cabin can be entered and exited through a nearby interaction.
- VS-AC-035: The cabin interior fills the gameplay camera and contains a bed.
- VS-AC-036: Using the bed advances the in-game clock by 8 hours.
- VS-AC-037: Outdoor brightness changes by time of day and a torch item provides a small local glow at night.

## Manual Test VS-001

Preconditions:

- Project opens in Unity 6000.5.8f1.
- Scene `Assets/Game/Scenes/Bootstrap/Bootstrap.unity` is loaded.

Steps:

1. Press Play.
2. Confirm the HUD shows health, minimap, time, and bottom hotbar.
3. Confirm the objective tracker is visible under the status panel.
4. Press `I`, confirm inventory overlay opens, then press `Esc` to close.
5. Press `M`, confirm expanded map opens, then press `Esc` to close.
6. Move the player with `WASD` or arrow keys.
7. Drag the bottom-left virtual joystick and confirm the player moves in that direction.
8. Confirm the camera follows the player and the minimap marker moves.
9. Click or tap a hotbar slot and confirm the selected slot highlight changes.
10. Tap the `Map` action button and confirm the expanded map opens, then tap `Close`.
11. Tap the `Bag` action button and confirm inventory opens, then tap `Close`.
12. Press `J`, confirm the Roadwarden Journal opens, then press `Esc` to close.
13. Walk along the road and confirm the camera continues following across the wider map.
14. Move near a landmark such as Old Road Sign and confirm an inspect prompt appears.
15. Press `E` or tap `Gather` to inspect it, confirm a countdown bar appears above the landmark, then confirm the objective tracker updates after the countdown completes.
16. Open `J` and confirm the journal entry appears.
17. Press `M` and confirm the expanded map shows the road, river, resources, landmarks, and player marker.
18. Move near a loot chest and confirm it shows a glow outline with an `E` prompt.
19. Press `E` or tap `Gather` to open it, confirm a countdown bar appears above the chest, then confirm item quantities update after the countdown completes.
20. Move near a Tree and confirm it shows a glow outline with an `E` prompt.
21. Press `E` or tap `Gather` to gather Tree, and confirm the reward is granted only after the countdown bar completes.
22. Move near a Rock and press `E` or tap `Gather`, and confirm walking away before the countdown completes cancels the action.
23. Move near berries, herbs, mushrooms, or iron ore and confirm the gathered item appears in the hotbar and inventory.
24. Confirm the hotbar shows enough wood and stone and objective progress updates.
25. Press `C` or tap `Craft` and confirm Cabin Plank increases while Wood decreases.
26. Press `B` or tap `Build`.
27. Move the mouse to a valid grid cell.
28. Left-click to start cabin construction and confirm a construction progress bar appears above the cabin site.
29. Wait for the cabin to complete, stand near it, and press `F` or tap the contextual `Enter` button.
30. Confirm the camera shows the full cabin interior with bed, hearth, table, and exit marker.
31. Stand near the bed and press `F` or tap `Sleep`; confirm the clock advances 8 hours.
32. Press `F` or tap `Exit` away from the bed and confirm the player returns outside.
33. Open a torch chest, wait for evening/night, and confirm a warm glow follows the player.
34. Stop Play, then press Play again.

Expected result:

- Inventory persists through reload.
- Crafted Cabin Plank persists through reload.
- Resource nodes remain harvested.
- Opened loot chests remain open.
- Discovered landmarks remain recorded in the journal.
- Objective tracker reflects restored inventory, landmark, and construction state.
- Cabin construction is restored.
- Construction progress reflects elapsed real time.
- Cabin completes when elapsed time is greater than or equal to duration.

## Domain Test Cases To Automate

- Inventory removal fails without partial mutation.
- Placement rejects out-of-bounds footprint.
- Construction completes when elapsed timestamp reaches duration.
- Save serializer rejects unsupported versions.

These are documented here because the current Unity Test Framework assembly reference did not resolve reliably in the installed Unity 6000.5.8f1 environment. Do not re-add test code until the test assembly setup is verified.

## Prototype Limitations

- Art is runtime-generated pixel-art placeholder sprites for camera, scale, and readability testing. It is not final production art.
- Collision is represented by placement occupancy, not final character collision.
- The map is wide and seeded, but it is not an infinite chunk-streaming world yet.
- Combat, NPC, dialogue, full authored quests, and weather are intentionally excluded.
