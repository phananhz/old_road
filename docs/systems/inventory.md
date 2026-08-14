# Inventory

- Version: 0.2
- Status: Draft
- Last updated: 2026-08-14
- Purpose: Define runtime item ownership.

`InventoryRuntime` stores item IDs and quantities. It supports add, remove, query, bulk cost checks, and conversion to/from save DTO entries.

Inventory removal must fail without partial mutation when resources are insufficient. UI observes inventory state and does not directly mutate save data.
