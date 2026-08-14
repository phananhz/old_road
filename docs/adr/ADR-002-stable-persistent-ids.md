# ADR-002 - Stable Persistent IDs

- Version: 0.2
- Status: Approved
- Last updated: 2026-08-14
- Purpose: Define identity rules for save data.

## Decision

Persistent entities use stable string IDs.

Examples:

- Item: `item.wood`
- Building definition: `building.cabin`
- Resource node: `node.tree.01`
- Construction instance: `construction.<guid>`

## Consequences

- Save data can survive scene reloads and Unity InstanceID changes.
- Runtime object references are reconstructed from saved IDs and plain placement data.
