# Crafting

- Version: 0.2
- Status: Draft
- Last updated: 2026-08-14
- Purpose: Define data-driven recipe execution.

`RecipeDefinition` contains stable recipe ID, ingredient requirements, result item, result quantity, optional crafting duration, and optional workstation ID.

`CraftingRuntime` validates all ingredients before deducting anything. Successful crafting deducts exact ingredients and adds the result through `InventoryRuntime`.

The prototype includes a minimal recipe asset for cabin planks so the data model is exercised, even though the vertical-slice cabin directly consumes wood and stone for placement.

In the current editor prototype, press `C` to run the next craftable progression recipe. The current priority is:

- Worn Axe: `2 item.wood`, `1 item.stone` -> `1 item.tool-axe`
- Stone Pick: `2 item.wood`, `3 item.stone` -> `1 item.tool-pickaxe`
- Cabin Plank: `2 item.wood` -> `1 item.cabin-plank`

Tool recipes are single-unlock prototype items. Once a tool exists in inventory, crafting skips that tool and moves to the next progression recipe. These values are prototype tuning only, not final progression balancing.
