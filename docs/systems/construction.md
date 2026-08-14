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

In the prototype, every active construction site draws a world-space progress bar above the building footprint. The bar uses the persisted job start time and duration, so it continues to reflect offline progress after save/load.

Completed cabin sites expose an enter interaction when the player stands nearby. The current prototype interior is generated at runtime and contains three visible areas: sleeping nook, living room, and kitchen. Partition walls visually divide those areas. Standing near the bed and using the cabin action opens a confirmation prompt; confirming advances the in-game clock by 8 hours.

Completed campfire and cooking hearth buildings emit a tight local warm light, flickering flame, ember pulse, rising smoke, and support prototype cooking. Completed cabin-style houses emit a small local warm window light and chimney smoke. Completed animal pen buildings produce prototype animal products over time. These values are prototype-only and exist to test the broader build loop.
