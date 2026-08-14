# Software Requirements Specification

- Version: 0.2
- Status: Draft
- Last updated: 2026-08-14
- Purpose: Convert the current prototype scope into traceable and testable requirements.

## Functional Requirements

- FR-PLAYER-001: The player shall move in 8 directions in real time.
- FR-PLAYER-002: Player movement shall consume an input abstraction rather than directly depending on a joystick implementation.
- FR-PLAYER-003: The prototype shall support a virtual on-screen joystick for mobile-style movement testing.
- FR-CAMERA-001: The camera shall use a fixed orientation and shall not rotate during gameplay.
- FR-CAMERA-002: The prototype camera shall use orthographic 2D framing suitable for top-down or top-down-oblique pixel-art gameplay.
- FR-VISUAL-001: Prototype gameplay objects shall use readable pixel-art placeholders until production sprites are authored.
- FR-VISUAL-002: Prototype sprites shall use point filtering and deterministic Y sorting for top-down readability.
- FR-WORLD-001: Valen Outskirts prototype world shall support a wider finite exploration map.
- FR-WORLD-002: Prototype terrain and resource placement may be generated from a deterministic seed for repeatable testing.
- FR-ITEM-001: Static item data shall use stable item IDs.
- FR-LOOT-001: Prototype loot containers shall use stable chest IDs.
- FR-LOOT-002: Opening a loot container shall reward inventory through gameplay logic and shall not update UI directly.
- FR-LOOT-003: Opened loot container state shall persist through save/load.
- FR-LOOT-004: Opening a loot container shall show a short world-space progress countdown above the target container before rewards are granted.
- FR-INV-001: Runtime inventory shall store item IDs and quantities separately from item definitions.
- FR-INV-002: Inventory removal shall fail without mutating state when resources are insufficient.
- FR-GATHER-001: Resource nodes shall be configurable by node ID, item ID, and yield amount.
- FR-GATHER-002: Harvesting shall reward inventory through gameplay logic and shall not update UI directly.
- FR-GATHER-003: Harvested node state shall be explicit.
- FR-GATHER-004: Gathering shall show a short world-space progress countdown above the target resource before rewards are granted.
- FR-EXPLORE-001: Prototype landmarks shall use stable landmark IDs.
- FR-EXPLORE-002: Player shall be able to inspect undiscovered landmarks through gameplay input.
- FR-EXPLORE-003: Discovered landmarks shall appear in a journal-style UI.
- FR-EXPLORE-004: Landmark discovery state shall persist through save/load.
- FR-EXPLORE-005: Inspecting a landmark shall show a short world-space progress countdown above the target before the journal entry is discovered.
- FR-QUEST-001: The vertical slice shall show a current objective tracker for the core prototype flow.
- FR-QUEST-002: Objective completion shall be derived from gameplay state rather than directly edited by UI.
- FR-CRAFT-001: Recipes shall define stable recipe ID, ingredients, result, optional duration, and optional workstation.
- FR-CRAFT-002: The prototype shall expose at least one manually testable crafting action through gameplay input.
- FR-BUILD-001: Building definitions shall define stable building ID, footprint, construction costs, duration, and visual stages.
- FR-BUILD-002: Building placement shall be grid-aligned.
- FR-BUILD-003: Placement shall validate buildable area, footprint, and overlap.
- FR-CONST-001: Construction shall create a stable construction instance ID.
- FR-CONST-002: Construction shall store building ID, start timestamp, duration, state, and placement.
- FR-CONST-003: Construction progress shall be derived from current time minus start time.
- FR-CONST-004: Construction shall not depend on a coroutine continuing while the app is closed.
- FR-CONST-005: Active construction sites shall show a world-space progress bar above the building footprint.
- FR-SAVE-001: Save data shall contain `saveVersion`.
- FR-SAVE-002: Save data shall persist inventory.
- FR-SAVE-003: Save data shall persist construction jobs and placement.
- FR-SAVE-004: Save data shall avoid Unity InstanceIDs and arbitrary scene references.
- FR-SAVE-005: Missing or invalid saves shall be handled explicitly.
- FR-VS-001: The Valen Outskirts flow shall be playable end-to-end in the Unity Editor.
- FR-UI-001: The prototype HUD shall show player health.
- FR-UI-002: The prototype HUD shall show a hotbar with key item quantities.
- FR-UI-003: The prototype HUD shall show a minimap with player and nearby world markers.
- FR-UI-004: The prototype HUD shall show in-game day and time.
- FR-UI-005: The prototype HUD shall allow hotbar slot selection by click or tap.
- FR-UI-006: The prototype HUD shall expose mobile-style action buttons for gather, craft, build, inventory, and map.
- FR-UI-007: The prototype HUD shall expose a journal/log overlay for discovered landmarks.

## Non-Functional Requirements

- NFR-PERF-001: Prototype gameplay code shall avoid large avoidable allocations in hot paths where practical.
- NFR-MAINT-001: Systems shall remain modular and avoid God classes.
- NFR-REL-001: Save writes shall use a temp file and backup where practical.
- NFR-USABILITY-001: The development build shall display enough HUD information to manually verify the vertical slice.

## Out Of Scope For Current Prototype

- Full combat
- NPC recruitment
- Dialogue
- Full authored quest system
- Weather simulation
- Production art
- Cloud save
- Monetization
