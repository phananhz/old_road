# Combat System

- Version: 0.3
- Status: Implemented (GAME-017 Micro-slice)
- Last updated: 2026-08-15
- Purpose: Define real-time 2D action combat and danger loop.

## Overview

The Old Road features top-down 2D real-time action combat with directional attack swings, weapon damage scaling, hitboxes/hurtboxes, knockback, floating damage numbers, invincibility frames (i-frames), and enemy loot drops.

## Player Combat

- **Controls**: `Space`, left-click on PC, or on-screen `⚔` button on mobile.
- **Attack Arc**: Directional pixel-art slash arc (`SlashVfx`) sweeping in the player's movement/facing direction.
- **Weapon Damage Scaling**:
  - **Unarmed**: 1 Damage, range 0.95, knockback 2.0.
  - **Worn Axe (`item.tool-axe`)**: 4 Damage, range 1.35, knockback 3.5 (Slashing).
  - **Stone Pick (`item.tool-pickaxe`)**: 3 Damage, range 1.15, knockback 4.5 (Blunt).
  - **Torch (`item.torch`)**: 2 Damage, range 1.05, knockback 2.2 (Fire).
- **Survival & I-Frames**:
  - Taking damage triggers 0.75s invincibility frames (sprite flashing) and red screen vignette.
  - Player health hearts on HUD deplete upon taking damage.
  - On death (0 HP), the player collapses and awakens by the campfire embers with restored health.

## Consumables & Quick-Eat

Food items in the hotbar can be quickly consumed with `Q` or the on-screen `Ăn / Eat` button to restore health:
- `item.wild-berries`: +2 HP
- `item.medicinal-herb`: +5 HP
- `item.cooked-meal`: +12 HP
- `item.egg`: +3 HP
- `item.milk`: +4 HP

## Enemy AI & State Machine

- **States**: `Idle` -> `Patrol` -> `Alert` -> `Chase` -> `AttackWindup` -> `Stagger` -> `Dead`.
- **Prototype Enemies**:
  - **Forest Wolf (`enemy.forest-wolf`)**: Fast agile beast lurking in dark woods (10 HP, 2.4 speed, 3 damage, drops `Wool` and `Wild Berries`).
  - **Bandit Scout (`enemy.bandit-scout`)**: Wandering human rogue patrolling ruins and bridges (14 HP, 1.9 speed, 4 damage, drops `Old Coin` and `Torch`).
- **Feedback**: Floating damage numbers (`FloatingTextController`), red hit flash, dynamic health bar displayed above damaged enemies, and death dissolve animation.

