# Story and Planning lessons (read before every round)

## Start here (fresh thread)
This file, `docs/HANDOFF.md`, `docs/PRINCIPLES.md` and your job are all you need. Check live pull request state on
GitHub before saying anything about a pull request. If you have to ask the owner something these files should have
answered, add the answer here (a working rule) or in the handoff (a decision or trap) in the same turn.

## Owner rules (each correction as a rule, with its reason)
- Nothing personal goes into this repository. Reason: it is public, and git history keeps everything.
- Nothing is pushed without the owner's go. Reason: the owner wants to see plans and pictures first.

## Who owns what
- Story and Planning: `docs/HANDOFF.md`, `docs/STORY.md`, `docs/PRINCIPLES.md`, `docs/plan/`, `docs/planning/`,
  and every player-facing string under `data/`.
- Building: `src/`, `scenes/`, `tests/`, `docs/building/`, and the non-text tables under `data/`.
- The owner: every merge, every look and feel decision, the premise.

## Running a round
1. Read the handoff and the job. Fetch `main`; branch from it.
2. Propose with a picture or a short doc first; put the owner's choice on a card with one recommended option.
3. Write the order (`docs/planning/orders/<round>.md`, template in that folder) once the owner approves.
4. When Building's report lands, apply it to the handoff: state, history row, traps.
5. Close the round: a round-log line below, any new owner rule above with its reason, stale lines cut.

## Craft rules
- One order is one job a single pull request can finish.
- Every order names what must not move and how it will be proved (tests and pictures).

## Round log (one line per round; rules go above, not here)
- 30 Oct 2026, setup: project, proof of concept and day-one docs.
