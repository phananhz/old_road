# Gathering

- Version: 0.2
- Status: Draft
- Last updated: 2026-08-14
- Purpose: Define resource-node harvesting.

`ResourceNode` is configurable by stable node ID, reward item ID, and yield amount. `TryHarvest` sends rewards through `InventoryRuntime` and marks the node harvested.

The resource node does not reference or update inventory UI.

The prototype player gather action is timed. Pressing the gather input near a highlighted node starts a short world-space countdown above that node. The reward is granted only when the countdown completes. Moving out of interaction range cancels the action. The current 1.2 second duration is a prototype value for manual testing only.

Current prototype harvestable resources:

- pine/tree nodes: `item.wood`;
- field stones: `item.stone`;
- berry bushes: `item.wild-berries`;
- herb patches: `item.medicinal-herb`;
- mushroom clusters: `item.mushroom`;
- iron veins: `item.iron-ore`, requires `item.tool-pickaxe`.

Tool requirements are prototype progression gates. If the player is near a gated resource without the required tool, the prompt explains which tool is missing and the harvest action does not start.

Future expansion may add node health, richer tool tiers, drop tables, respawn, and regrowth.
