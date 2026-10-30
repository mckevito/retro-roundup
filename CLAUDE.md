# Retro Roundup: notes for Claude

A small story game in Godot 4.7.2 .NET (C#, net8.0, built with the .NET 10 SDK), Forward+ renderer, Windows first.
Its look is outline art: black faces with thin glowing magenta edges.

Start with **`docs/HANDOFF.md`** (decisions, current state, open work, traps), then your role's lessons file
(`docs/planning/LESSONS.md` or `docs/building/LESSONS.md`), then **`docs/PRINCIPLES.md`** (what good looks like).
Your job is one file: an order in `docs/planning/orders/`, or the plan in `docs/plan/BUILD_ORDER.md`.
Work from these files, never from old chats.

## Hard rules
- **This repository is public.** Never put personal information in it: no real names, emails, home or user-folder
  paths, locations, schedules, account details, or quotes from chat. Docs say "the owner". The owner's working
  preferences live in project memory, not here. Read your diff for anything personal before every push.
- **Nothing is pushed without the owner's go.** One job, one branch from the latest `main`, one pull request.
  `main` only changes through pull requests, and the owner merges. Never stack a pull request on another open one;
  if one depends on another, its body says "Merge after #NNN".
- Every number, colour and string lives in a table under `data/`, never in code.
- Every change ships with headless tests for what it adds. Build and test before pushing; CI is a confirmation.
- Anything the owner can see gets a picture (a capture) in its pull request; motion gets a GIF or video.
- Story, names and designs are original. No real brands, and no recreations of existing games.
- Don't edit this file for status or progress; that goes in `docs/HANDOFF.md`. Edit this file only when a rule,
  the build steps or a top-level folder changes.

## Build and test
Cloud sessions install the tools with `.claude/hooks/session-start.sh` (progress in `/tmp/session-start.log`).
```
dotnet build
godot --headless --import
godot --headless res://tests/TestRunner.tscn               # every test; add -- --filter=Name for some
xvfb-run -a -s "-screen 0 1600x900x24" godot --path . --rendering-driver opengl3 -- --shot="$PWD/captures/street.png"
```
Results go to `tests/results.txt`. Godot loads the last built DLL, so run `dotnet build` after any `.cs` change.
The capture line saves one frame of the main scene (the cloud renders through OpenGL, so it uses the Compatibility
path; the game itself runs Forward+). CI (`.github/workflows/ci.yml`) builds and runs the tests on GitHub's own
runners for every pull request; docs-only changes skip it.

## Layout
- `src/Core`: shared plumbing: loading the `data/` tables (`GameData`, `Tables.cs`).
- `src/World`: the outline renderer (`OutlineBuilder`) and the street built from data (`OutlineStreet`).
- `src/Characters`: the first-person player.
- `scenes/`: `main.tscn`, the main scene.
- `data/`: every tuning table: `look.json`, `player.json`, `street.json`.
- `tests/`: the headless runner (`TestRunner.tscn`) and tests, one folder per system.
- `docs/`: the handoff, principles, story, build order, and each role's lessons, orders and reports.
