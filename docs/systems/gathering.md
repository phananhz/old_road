# Gathering

- Version: 0.2
- Status: Draft
- Last updated: 2026-08-14
- Purpose: Define resource-node harvesting.

`ResourceNode` is configurable by stable node ID, reward item ID, and yield amount. `TryHarvest` sends rewards through `InventoryRuntime` and marks the node harvested.

The resource node does not reference or update inventory UI.

Future expansion may add node health, required tools, drop tables, respawn, and regrowth.
