# Save System

- Version: 0.2
- Status: Draft
- Last updated: 2026-08-14
- Purpose: Define versioned runtime persistence.

Save data contains:

- `saveVersion`
- inventory entries
- construction jobs
- resource node harvested state for the vertical slice
- landmark discovery state
- loot chest state
- player position and whether the player was inside a cabin
- in-game clock minute

Save data stores stable IDs and plain values. It does not store Unity InstanceIDs, GameObjects, scene references, or ScriptableObject references.

`SaveRepository` writes to `Application.persistentDataPath` using a temporary file and backup where practical. Missing saves start safely. Invalid or unsupported saves are reported and ignored rather than silently corrupting runtime state.

Autosave runs every 10 seconds during play and also runs on important state changes such as gathering, crafting, construction, discovery, cooking, passive production, application pause, and application quit.

Save version 2 adds player position and clock time. Version 1 saves remain readable for the prototype; missing new fields are treated as optional.
