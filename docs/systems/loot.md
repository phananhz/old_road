# Loot

- Version: 0.1
- Status: Draft
- Last updated: 2026-08-15
- Purpose: Define the first prototype loot-container loop.

The current prototype supports one-time loot chests. Each chest has:

- a stable chest ID;
- a display name;
- one prototype item reward;
- a saved opened state.

Opening a chest rewards `InventoryRuntime` through gameplay logic and then marks the chest opened. UI reads the inventory and save-derived state; it does not directly mutate loot state.

The prototype open action is timed. Pressing the gather/interact input near a highlighted chest starts a short world-space countdown above that chest. The reward is granted only when the countdown completes. Moving out of interaction range cancels the action. The current 0.8 second duration is a prototype value for testing only.

Current rewards are prototype values for testing only and should be replaced by authored loot tables later. The current slice uses chests to introduce extra items such as `item.old-coin`, `item.iron-ore`, and `item.medicinal-herb` before full loot tables exist.
