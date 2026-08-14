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
- VS-AC-015: Nearby gatherable resources show a clear interaction hint and visual highlight.
- VS-AC-016: Player can craft one Cabin Plank from Wood using the prototype crafting recipe.
- VS-AC-017: HUD shows health, hotbar, minimap, and in-game time in a Minecraft-like layout.
- VS-AC-018: Player can open and close inventory and expanded map overlays.
- VS-AC-019: Valen Outskirts provides a larger prototype exploration area with multiple resource nodes and visible landmarks.
- VS-AC-020: Player can move with the on-screen virtual joystick.
- VS-AC-021: Player can select hotbar items by clicking or tapping slots.

## Manual Test VS-001

Preconditions:

- Project opens in Unity 6000.5.8f1.
- Scene `Assets/Game/Scenes/Bootstrap/Bootstrap.unity` is loaded.

Steps:

1. Press Play.
2. Confirm the HUD shows health, minimap, time, and bottom hotbar.
3. Press `I`, confirm inventory overlay opens, then press `Esc` to close.
4. Press `M`, confirm expanded map opens, then press `Esc` to close.
5. Move the player with `WASD` or arrow keys.
6. Drag the bottom-left virtual joystick and confirm the player moves.
7. Confirm the camera follows the player and the minimap marker moves.
8. Click or tap a hotbar slot and confirm the selected slot highlight changes.
9. Walk along the road and confirm the camera continues following across the larger map.
10. Press `M` and confirm the expanded map shows the road, river, resources, landmarks, and player marker.
11. Move near a Tree and confirm it highlights with an `E` prompt.
12. Press `E` to gather Tree.
13. Move near a Rock and press `E`.
14. Confirm the hotbar shows enough wood and stone.
15. Press `C` and confirm Cabin Plank increases while Wood decreases.
16. Press `B`.
17. Move the mouse to a valid grid cell.
18. Left-click to start cabin construction.
19. Stop Play, then press Play again.

Expected result:

- Inventory persists through reload.
- Crafted Cabin Plank persists through reload.
- Resource nodes remain harvested.
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
- Combat, NPC, dialogue, quests, and weather are intentionally excluded.
