# ADR-001 - ScriptableObject Definitions

- Version: 0.2
- Status: Approved
- Last updated: 2026-08-14
- Purpose: Decide how authored static data is represented.

## Decision

Use ScriptableObjects for static definitions such as items, recipes, and buildings.

Runtime player state must not live inside shared ScriptableObject assets.

## Consequences

- Static data can be authored in Unity and referenced by IDs.
- Save data stores stable IDs, not asset references.
- Prototype fallback definitions may be created at runtime only to keep the development slice runnable.
