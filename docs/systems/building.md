# Building Placement

- Version: 0.2
- Status: Draft
- Last updated: 2026-08-14
- Purpose: Define grid-aligned building placement.

`BuildingDefinition` contains stable building ID, footprint, construction costs, prototype duration, and visual stage names.

`GridPlacementValidator` is independent from UI. It validates buildable area, positive footprint, and overlap state.

The HUD build action opens a prototype construction catalog before placement. The catalog shows a large panel with category tabs such as Housing, Fire & Light, and Animal Pens. Each card shows a prototype building preview and required material information. Selecting a buildable card enters placement mode only when the player has the required materials; otherwise the catalog stays open and shows which materials are missing.

Current buildable prototype buildings:

- Cabin
- Stone Cottage
- Storage Shed
- Campfire
- Cooking Hearth
- Small Animal Pen
- Long Animal Pen

`BuildingPlacementController` is the development placement adapter. After a building is selected from the catalog, move the mouse to position the preview, left-click to confirm, and right-click or press `B` to cancel.

The controller asks `VerticalSliceController` to begin construction. It does not directly save data.
