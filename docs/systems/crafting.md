# Crafting

- Version: 0.2
- Status: Draft
- Last updated: 2026-08-14
- Purpose: Define data-driven recipe execution.

`RecipeDefinition` contains stable recipe ID, ingredient requirements, result item, result quantity, optional crafting duration, and optional workstation ID.

`CraftingRuntime` validates all ingredients before deducting anything. Successful crafting deducts exact ingredients and adds the result through `InventoryRuntime`.

The prototype includes a minimal recipe asset for cabin planks so the data model is exercised, even though the vertical-slice cabin directly consumes wood and stone for placement.
