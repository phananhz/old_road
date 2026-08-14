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

Save data stores stable IDs and plain values. It does not store Unity InstanceIDs, GameObjects, scene references, or ScriptableObject references.

`SaveRepository` writes to `Application.persistentDataPath` using a temporary file and backup where practical. Missing saves start safely. Invalid or unsupported saves are reported and ignored rather than silently corrupting runtime state.
