# Architecture

- Version: 0.2
- Status: Draft
- Last updated: 2026-08-14
- Purpose: Define boundaries for an incremental Unity implementation.

## Layers

- Data definitions: ScriptableObjects such as `ItemDefinition`, `RecipeDefinition`, and `BuildingDefinition`.
- Domain runtime: plain C# runtime state such as `InventoryRuntime`, `ConstructionJob`, and save DTOs.
- Application/composition: scene-level orchestration such as `VerticalSliceController`.
- Presentation: MonoBehaviours that display state or adapt input, such as HUD, resource node visuals, and placement preview.
- Persistence: `SaveSerializer` and `SaveRepository`.

Dependencies should point toward stable contracts and plain state. UI may read state and call application services, but UI must not directly mutate save files.

## Composition Root

`GameBootstrap` is intentionally small. It creates `VerticalSliceController` for the current prototype. It is not a general-purpose GameManager.

`VerticalSliceController` is allowed to wire prototype scene objects together. Product logic should still live in domain classes where practical.

## Static Data vs Runtime State

ScriptableObjects hold authored static data only. Mutable player state lives in runtime classes and save DTOs.

Examples:

- `ItemDefinition`: item ID and display metadata
- `InventoryRuntime`: item quantities
- `BuildingDefinition`: cabin footprint, cost, duration, stages
- `ConstructionJob`: one placed construction instance

## Time

Construction uses `IClock`. Domain logic accepts time as an abstraction so tests can simulate elapsed time without waiting.

## Save

Save data stores stable IDs and plain values. It must not store Unity `InstanceID`, GameObject references, scene references, or ScriptableObject object references.

## Unity Scene Policy

Do not hand-edit complex scene YAML. Use the Unity Editor or `Assets/Game/Scripts/Editor/ProjectSetup.cs` to rebuild project data and the Bootstrap scene.
