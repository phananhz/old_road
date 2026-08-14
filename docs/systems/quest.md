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

The UI reads objective text from the vertical-slice controller and does not directly mutate objective state. This keeps the first implementation small while making the playable flow clearer.

Landmark inspection is a timed prototype interaction. The journal entry and objective state update only after the world-space countdown above the landmark completes.

Current objective sequence:

1. Inspect an old-road landmark.
2. Gather 3 wood.
3. Gather 2 stone.
4. Craft 1 cabin plank.
5. Start cabin construction.
6. Complete the first cabin.

Future expansion should replace this with authored quest definitions, rewards, prerequisites, and localized text.
