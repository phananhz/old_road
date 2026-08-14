# Roadmap

- Version: 0.2
- Status: Draft
- Last updated: 2026-08-14
- Purpose: Sequence milestones without expanding scope prematurely.

## M0 - Project Foundation

Goal: Unity project opens, has required folder structure, documentation, ADRs, and a Bootstrap scene.

Exit criteria: project compiles, docs exist, `ProjectSetup` can rebuild prototype scene.

## M1 - Valen Outskirts Vertical Slice

Goal: Validate the core loop before adding broad content.

Deliverables: movement, inventory, gathering, crafting foundation, building placement, construction timer, save/load, integrated manual test.

Exit criteria: TEST VS-001 passes in Editor and no unexplained compiler errors remain.

## M2 - Technical Audit

Goal: Review architecture consistency before adding combat, NPCs, dialogue, quests, and weather.

Exit criteria: findings are classified and next milestone blockers are identified.

## M3 - Exploration Danger Prototype

Goal: Add the first minimal danger/combat loop after the vertical slice is stable.

Exit criteria: small combat encounter works without breaking persistence or movement.
