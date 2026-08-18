# The Old Road

The Old Road is a Unity 6 / C# mobile-first 2D pixel-art adventure RPG prototype.

The current goal is the Valen Outskirts vertical slice: walk, gather wood and stone, place a cabin blueprint, begin timestamp-based construction, save, quit, and reload with offline construction progress restored.

## Unity Version

- Editor: Unity 6000.5.8f1
- Pipeline: Built-in render pipeline for the prototype
- Primary targets: Android and iOS

## How To Run

1. Open this folder in Unity Hub: `E:\Unity Project\Old Road`.
2. Open `Assets/Game/Scenes/Bootstrap/Bootstrap.unity`.
3. Press Play.

The scene is intentionally small. `GameBootstrap` creates the Valen Outskirts prototype runtime.

## Controls

- Move: `WASD` or arrow keys
- Gather: `E` near a tree or rock
- Build mode: `B`
- Confirm building placement: left mouse click
- Cancel building placement: right mouse click

Prototype values:

- Tree reward: `3 item.wood`
- Rock reward: `2 item.stone`
- Cabin cost: `3 item.wood`, `2 item.stone`
- Cabin duration: `30` seconds

## Important Files

- `AGENTS.md` - work rules for future Codex sessions
- `docs/GAME_DESIGN.md` - product vision and story premise
- `docs/SRS.md` - traceable requirements
- `docs/ARCHITECTURE.md` - architecture rules and dependency direction
- `docs/features/vertical-slice.md` - playable slice flow and test
- `Assets/Game/Scripts/Core/VerticalSliceController.cs` - prototype composition root

## Rebuild Prototype Scene

If Unity shows a broken scene, run:

`Tools > The Old Road > Rebuild Prototype Project`

or batchmode:

`E:\Unity Editor\6000.5.8f1\Editor\Unity.exe -batchmode -quit -projectPath "E:\Unity Project\Old Road" -executeMethod TheOldRoad.Editor.ProjectSetup.RebuildPrototypeProject`
 1   0=-090-+87\][POIYTREA.,mnbvcz ]