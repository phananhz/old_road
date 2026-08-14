# The Old Road - Agent Instructions

## Project

The Old Road is a Unity 6 / C# mobile-first 2D pixel-art adventure RPG.

## Required Context

Before implementing a non-trivial feature, read:

- docs/GAME_DESIGN.md
- docs/SRS.md
- docs/ARCHITECTURE.md
- relevant docs/systems/*
- relevant docs/adr/*
- the active feature or ticket specification

Inspect existing code and tests before modifying anything.

## Source of Truth

Use this priority when requirements conflict:

1. Current explicit user instruction
2. Approved feature or system specification
3. SRS
4. Architecture and ADRs
5. Game Design Document
6. Existing implementation

If a conflict affects player-visible behavior and cannot be resolved safely, stop and ask.

## Product Decisions

Do not invent unspecified product values such as prices, damage values, crafting durations, construction durations, item stack limits, inventory capacity, resource yields, or progression costs.

Prototype values may only be introduced when clearly labelled and necessary to test a system.

## Architecture Rules

- Use Unity 6 and C#.
- Prefer modular focused components.
- Avoid God classes.
- Avoid unnecessary global singletons.
- Separate static definition data from mutable runtime state.
- Do not store runtime player state inside shared ScriptableObjects.
- Use stable IDs for persistent entities.
- UI must not directly manipulate save data.
- Keep gameplay and domain logic independent from presentation where practical.
- Player movement must use an input abstraction.
- Do not introduce third-party dependencies without approval.
- Do not overengineer early systems.

## Save Rules

- Persistent data requires stable identity.
- Do not persist Unity runtime InstanceID values as long-term identity.
- Do not serialize arbitrary scene references as persistent world identity.
- Save files must support a version field.
- Consider backward compatibility when modifying persistent structures.

## Scope Control

Implement only the requested ticket. Do not silently add multiplayer, monetization, account systems, cloud services, procedural infinite worlds, or unrelated gameplay systems.

## Verification

A task is not complete until:

- the project compiles;
- applicable tests pass;
- acceptance criteria are checked;
- no unexplained errors remain;
- documentation is updated when required.

## Unity

Prefer Unity-native workflows.

Do not blindly hand-edit complex serialized Scene or Prefab YAML. Use Editor scripts or the Unity Editor for scene/prefab generation.
