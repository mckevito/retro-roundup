# Handoff (last updated: 30 October 2026, setup round)

The one source of truth for this project. Every round updates it in the same pull request: state, history row,
open work, traps. Story and Planning owns it; Building appends its slice (history row, traps).

## 1. The owner and how to work with them

- One owner. They set the direction and the order of work, judge looks and feel from pictures, pick on decision
  cards and merge every pull request.
- Plain words and full sentences. Lead with what you need from them, then the result, with links.
- Everything waiting on them is a multiple-choice question with one recommended option, not buried in a message.
  Asking is not approving: only their own words, or an option they picked, approve something.
- Pictures before anything big is built; real art before code that shows it.
- Their other working preferences live in project memory, not in this public repo.

## 2. Decisions

Owner decisions are in bold, with the date. Defaults are marked as defaults.

- **Engine: Godot 4.7.2 .NET with C#** (30 Oct 2026). Forward+ renderer.
- **No browser build** (30 Oct 2026). Windows first.
- **Look: outline art**: thin glowing magenta lines on black faces (30 Oct 2026, from the owner's reference picture
  of a 1970s concrete tower seen from the street).
- **The repository is public, `main` is protected** (30 Oct 2026): changes only through pull requests, no force
  pushes, no branch deletion.
- **No personal information in the repository, ever** (30 Oct 2026).
- Default: two roles to start (Story and Planning, Building); Art and Sound joins when real art work begins.
- Default: line colour, glow and every street measurement live in `data/` so the look can be tuned without code.

## 3. Current state (what is on `main` after the setup pull request)

- A walkable proof of concept: `scenes/main.tscn` builds a street from `data/street.json` (13 box buildings, a
  36-floor tower with balconies on the right, lower blocks on the left, road markings, a crosswalk and street
  lights) in the outline look from `data/look.json`, with a first-person player from `data/player.json`.
- Outlines are real line meshes over black faces that are pulled in 2 cm, so edges always draw in front. One
  faces mesh and one lines mesh for the whole street: two draw calls.
- Six headless tests (data sanity, street build, determinism, player setup) and CI on GitHub's runners.
- Story, premise and format: **not decided**. See `docs/STORY.md`.

## 4. History

| Date | Round | PR | What changed |
| --- | --- | --- | --- |
| 30 Oct 2026 | setup | (this PR) | Godot .NET project, outline street proof of concept, tests, CI, day-one docs |

## 5. Open work and plans

- The owner picks the premise and the format (`docs/STORY.md` lists three original pitches).
- Then Story and Planning writes the build order's stages for that format (`docs/plan/BUILD_ORDER.md`).

## 6. Next steps (owner-agreed order; strike done items)

1. Owner merges the setup pull request.
2. Owner picks premise and format.
3. Story and Planning writes the story doc and stage 1's order.

## 7. How to run and show work

See `CLAUDE.md` (build, test, capture). Review pictures come from the capture line; put them on the pull request.

## 8. Traps already found

- **Windows: an apostrophe in the user folder path breaks the C# build** (Godot.NET.Sdk's reference check fails
  with MSB4113). `RetroRoundup.csproj` restores NuGet packages to a folder without one when that happens. Keep it.
- **Godot loads the last built DLL.** After editing any `.cs` file, run `dotnet build` before Godot, or you test
  stale code.
- **Thin lines shimmer at a distance.** MSAA 4x is on in `project.godot`. If shimmer becomes a problem as the
  world grows, fade small details (windows) out with distance rather than thickening lines.
- **Cloud captures use OpenGL** (no Vulkan in the cloud), so they render through Godot's Compatibility path. The
  look matches; Forward+-only effects won't show in cloud captures.
- **A scene path given to Godot must exist and build.** If the C# build fails, `godot --headless
  res://tests/TestRunner.tscn` can hang instead of exiting; build first and use a `timeout`.
