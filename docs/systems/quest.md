# Quest and Story

- Version: 0.2
- Status: Draft
- Last updated: 2026-08-15
- Purpose: Define the opening story arc and lightweight quest tracker for the current prototype.

The current prototype now uses authored opening story chapters backed by stable step IDs. This is still not the final full RPG quest system, but it gives the player a real narrative path through the available prototype mechanics.

Quest completion is derived from existing gameplay state:

- landmark discovery count;
- inventory quantities;
- crafted item quantities;
- active or completed construction jobs.
- simple prototype story/progression flags such as speaking with a villager.

The UI reads objective text from the vertical-slice controller and does not directly mutate objective state. Completed story step IDs are saved so unlocked journal story entries remain unlocked even if later tuning changes the exact requirement.

Landmark inspection is a timed prototype interaction. The journal entry and objective state update only after the world-space countdown above the landmark completes.

Current opening story arc:

## Chapter I - The Bell Beyond Valen

The player proves that the old road has returned and recovers the first piece of Father's Roadwarden trail.

1. Inspect an old-road landmark.
2. Open an old chest.
3. Recover Father's Roadwarden journal page.
4. Speak with a village NPC.

## Chapter II - Roadwarden's Burden

The player prepares tools and supplies before travelling farther from Valen.

1. Craft a worn axe.
2. Gather 3 wood and 2 stone.
3. Craft a stone pick.
4. Mine iron ore.
5. Forage food or herbs.

## Chapter III - Fire Against The Dark

The player creates a return point and begins making the road survivable again.

1. Craft 1 cabin plank.
2. Start construction on a cabin or stone cottage.
3. Find the first bell fragment.
4. Build a campfire or cooking hearth.
5. Cook one meal.
6. Build an animal pen.

## Chapter IV - Blackwood Omen

The player discovers the next major adventure direction.

1. Find Blackwood Cave.
2. Read the dragon-scarred ridge.

Future expansion should replace the current plain-code quest definitions with authored quest assets, rewards, prerequisites, dialogue trees, and fully localized story text.

The cave and dragon steps are story hooks only in this prototype. They establish the future direction for cave exploration and a dragon confrontation without implementing full combat, boss AI, dungeon generation, or authored quest rewards yet.
