# Construction

- Version: 0.2
- Status: Draft
- Last updated: 2026-08-14
- Purpose: Define persistent construction jobs and offline progress.

Construction states:

- Planned
- Constructing
- Completed

`ConstructionJob` stores stable construction ID, building definition ID, Unix start timestamp, duration, grid placement, and state.

Progress is computed from current time minus start timestamp. It does not depend on a coroutine, so progress can be recalculated after the app closes.

`IClock` abstracts time. `SystemClock` is used at runtime and `ManualClock` supports tests.

Visual stages are derived independently from progress. The default conceptual stages are Foundation, Frame, Walls, Roof, and Complete.
