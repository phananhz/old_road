# Building Placement

- Version: 0.2
- Status: Draft
- Last updated: 2026-08-14
- Purpose: Define grid-aligned building placement.

`BuildingDefinition` contains stable building ID, footprint, construction costs, prototype duration, and visual stage names.

`GridPlacementValidator` is independent from UI. It validates buildable area, positive footprint, and overlap state.

`BuildingPlacementController` is the development presentation adapter. Press `B` to enter placement mode, move the mouse to position the preview, left-click to confirm, and right-click to cancel.

The controller asks `VerticalSliceController` to begin construction. It does not directly save data.
