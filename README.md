# Retro Roundup

A small story game in an outline-art look: glowing magenta lines on black. Built in Godot 4.7.2 .NET (C#).

The story and format are still being decided; see [docs/STORY.md](docs/STORY.md). Right now the project holds a
walkable proof of concept: a city street with a 1970s concrete tower, drawn entirely in outlines.

## Run it

1. Install the [.NET SDK](https://dotnet.microsoft.com/download) (8 or newer) and
   [Godot 4.7.2 .NET](https://godotengine.org/download) (the ".NET" build, not the standard one).
2. Open `project.godot` in Godot and press F5.
3. Click to capture the mouse. WASD or the arrow keys walk, Shift runs, Esc frees the mouse.

## Test it

```
dotnet build
godot --headless --import
godot --headless res://tests/TestRunner.tscn
```

## How the project is run

The project is built by a small team of Claude sessions and one owner. Start at [CLAUDE.md](CLAUDE.md).
