# ADR-004 - Save Architecture

- Version: 0.2
- Status: Approved
- Last updated: 2026-08-14
- Purpose: Define save boundaries for the vertical slice.

## Decision

Use versioned plain DTOs serialized by `SaveSerializer` and stored by `SaveRepository`.

Save files contain:

- save version
- inventory entries
- construction jobs
- vertical-slice resource node state

Construction progress is recalculated from timestamps through `IClock`.

## Consequences

- Missing save starts safely.
- Invalid or unsupported saves are explicit failures.
- Future migrations can branch by `saveVersion`.
