# Player System

- Version: 0.2
- Status: Draft
- Last updated: 2026-08-14
- Purpose: Define player movement boundaries.

The player uses 8-direction real-time movement for a fixed top-down or top-down-oblique 2D perspective. Diagonal input is clamped to prevent faster diagonal movement.

Input is supplied through `IPlayerInputSource`. `KeyboardPlayerInputSource` is the development adapter. Mobile joystick input is future scope and must plug into the same abstraction.

`PlayerMovement` owns locomotion only. Combat, gathering, UI, and save logic remain separate.

The current prototype uses transform-based movement to avoid editor package instability while preserving collision-ready architecture boundaries for later.
