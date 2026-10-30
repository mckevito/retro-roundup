# Building lessons (read before every round)

## Start here (fresh thread)
This file, `docs/HANDOFF.md`, `docs/PRINCIPLES.md` and your order are all you need. Check live pull request state
on GitHub before saying anything about a pull request. If you have to ask the owner something these files should
have answered, add the answer here or in the handoff's traps in the same turn.

## Owner rules (each correction as a rule, with its reason)
- Nothing personal goes into this repository. Reason: it is public, and git history keeps everything.
- Nothing is pushed without the owner's go. Reason: the owner wants to see plans and pictures first.

## Who owns what
- Building: `src/`, `scenes/`, `tests/`, `docs/building/`, and the non-text tables under `data/`.
- Story and Planning: the handoff, story, principles, plan, orders, and every player-facing string.
- The owner: every merge, every look and feel decision.

## Running a round
1. Read the handoff and your order. Fetch `main`; branch from it.
2. Put every new number in a `data/` table; write tests for what you add.
3. `dotnet build`, then the headless tests; capture pictures from the order's viewpoints.
4. Read the diff for anything personal. One pull request, template filled in (Before, After, How), pictures on it.
5. Write `docs/building/reports/<round>.md` (template in that folder) and add the handoff's history row and traps.
6. Close the round: a round-log line below, any new owner rule above with its reason.

## Craft rules
- Outlines: faces pulled in by `face_inset_m`, details pushed out by `detail_offset_m`, so lines never fight faces.
- Keep one lines mesh and one faces mesh per area; many small meshes cost draw calls.
- Determinism: anything random is seeded from data, so the same data builds the same world.

## Round log (one line per round; rules go above, not here)
- 30 Oct 2026, setup: C# project, outline street, six tests, CI.
