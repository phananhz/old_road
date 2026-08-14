# Game Design Document

- Version: 0.2
- Status: Approved for current prototype scope
- Last updated: 2026-08-14
- Purpose: Define the product vision, player fantasy, world premise, and vertical-slice direction.

## Product Vision

The Old Road is a mobile-first 2D pixel-art adventure RPG built with Unity 6 and C#. The game uses a fixed orthographic top-down or top-down-oblique perspective with its own medieval fantasy identity.

The emotional target is: make players feel nostalgic for a world they have never actually lived in.

The game is not only cozy. Valen and nearby settlements feel warm and human, while exploration becomes more dangerous as the player travels farther from civilization.

## Platforms

- Primary: Android, iOS
- Possible future: PC

## World Tone

The world includes villages, forests, rivers, mountains, snow, rain, fog, ruins, ancient roads, abandoned settlements, castles, warm houses, fireplaces, candles, atmospheric weather, and nostalgic music.

The target visual style is handcrafted pixel art with readable top-down gameplay silhouettes and dark medieval atmosphere: warm firelight, cool night shadows, old stone, aged wood, banners, roads, castles, forest edges, and cozy interiors. Cinematic key art may be more detailed, but playable sprites must remain clear at mobile scale.

Do not copy Stardew Valley assets, characters, maps, systems, or visual design.

## Core Loop

Explore -> gather resources -> fight enemies -> discover places -> find loot, story clues, and materials -> return to settlements -> craft -> build -> upgrade infrastructure -> become stronger -> access new regions -> explore farther.

Permanent motivations:

- What is beyond the next road?
- How can I become stronger and better prepared?
- What happened to this world?

## Sandbox Principles

The game borrows Minecraft-like principles of gathering, cutting trees, mining rocks and ore, gathering plants, fishing, crafting, building structures, and transforming the world.

This is not a voxel game. The world remains handcrafted 2D pixel art.

## Construction

Construction is time-based and should support offline progress. A cabin should progress through stages such as Foundation, Frame, Walls, Roof, and Complete. The player can start construction, leave, explore, close the app, return later, and see correct progress.

Timers exist for simulation and immersion, not monetization barriers.

## Settlement

The starting settlement is Valen, a small medieval village. The player's actions can gradually develop it. Specialists such as carpenter, blacksmith, tailor, alchemist, and glassmaker may unlock systems later.

## Story Premise

About twenty years before the game begins, The Night The Bells Fell Silent caused the old kingdom to collapse. Ancient roads became inaccessible or dangerous. Settlements became isolated. Trade collapsed. Strange creatures and phenomena appeared.

The player grew up in Valen. The player's father was one of the last Roadwardens, protectors of the ancient road network. He disappeared years ago, leaving an old sword, an incomplete map, and a journal.

The journal's final message is approximately: "If the roads ever open again, do not believe what they told us about that night."

At the start of the game, a bell rings from far beyond the forest. The next morning, an ancient road has returned.

## Main Mystery

The official history of the collapse is incomplete. The Bell Towers were connected to something beneath the Old Capital: The Heart Below. The old king deliberately disrupted the network, destroying the kingdom as people knew it but possibly preventing something worse. Now the Bell Towers are activating again, and nobody appears to be ringing them.

## First Vertical Slice

Valen Outskirts validates the core loop before broader content:

walk -> gather wood -> gather stone -> view inventory -> place cabin -> begin construction -> continue exploring -> save -> reload -> construction progress restores -> cabin completes.
