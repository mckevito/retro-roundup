# Build order

The plan of record. Stages are built in this order; each has a proof table (tests a machine runs, plus the owner's
own try) and is signed off by the owner before the next starts. Departures are marked "Proposed".

## Stage 0: project setup (done in the setup pull request)

| Proof | How |
| --- | --- |
| The project builds and runs on Windows | Owner opens `project.godot` and presses F5 |
| Tests run headless in CI | `.github/workflows/ci.yml` green on the pull request |
| Tables load and are sane | `DataTests` |
| The outline street builds the same way every time | `StreetTests` |

## Stage 1 onward

Written by Story and Planning once the owner picks the premise and the format (`docs/STORY.md`). Expected shape:
the core interaction (talking, choosing, moving) in a test section of the main scene, then saving, then a
vertical slice of one scene with real art, then the rest of the story, then polish and release.
