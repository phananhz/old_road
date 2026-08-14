# Quest and Objectives

- Version: 0.1
- Status: Draft
- Last updated: 2026-08-15
- Purpose: Define the lightweight objective tracker for the current vertical slice.

The current prototype uses a computed objective tracker rather than a full quest system. Objective completion is derived from existing gameplay state:

- landmark discovery count;
- inventory quantities;
- crafted item quantities;
- active or completed construction jobs.
- simple prototype story/progression flags such as speaking with a villager.

The UI reads objective text from the vertical-slice controller and does not directly mutate objective state. This keeps the first implementation small while making the playable flow clearer.

Landmark inspection is a timed prototype interaction. The journal entry and objective state update only after the world-space countdown above the landmark completes.

Current objective sequence:

1. Inspect an old-road landmark.
2. Open an old chest.
3. Recover Father's Roadwarden journal page.
4. Speak with a village NPC.
5. Craft a worn axe.
6. Gather 3 wood.
7. Gather 2 stone.
8. Craft a stone pick.
9. Mine iron ore.
10. Forage any wild food or herb.
11. Craft 1 cabin plank.
12. Start cabin construction.
13. Complete the first cabin.
14. Find the first bell fragment.
15. Build a campfire.
16. Cook one meal.
17. Build an animal pen.

Future expansion should replace this with authored quest definitions, rewards, prerequisites, and localized text.
