# ADR-003 - Input Abstraction

- Version: 0.2
- Status: Approved
- Last updated: 2026-08-14
- Purpose: Keep player movement independent from a specific input device.

## Decision

Player movement consumes `IPlayerInputSource`. Keyboard input is only the current development adapter.

## Consequences

- Mobile joystick and gamepad adapters can be added without rewriting movement.
- Gameplay code does not depend on a specific joystick package.
