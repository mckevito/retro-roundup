using Godot;
using RetroRoundup.Core;

namespace RetroRoundup;

/// <summary>
/// The proof-of-concept scene: the outline street plus a first-person walker.
/// Run with `-- --shot=PATH` to save one frame as a PNG and quit (used for review pictures).
/// </summary>
public partial class Main : Node3D
{
    const int ShotDelayFrames = 10;

    public override void _Ready()
    {
        AddEnvironment(GameData.Load<LookTable>(LookTable.Path));
        var ground = new StaticBody3D();
        ground.AddChild(new CollisionShape3D { Shape = new WorldBoundaryShape3D() });
        AddChild(ground);
        foreach (string arg in OS.GetCmdlineUserArgs())
        {
            if (arg.StartsWith("--shot="))
                SaveShot(arg["--shot=".Length..]);
        }
    }

    void AddEnvironment(LookTable look)
    {
        var env = new Environment
        {
            BackgroundMode = Environment.BGMode.Color,
            BackgroundColor = GameData.ToColor(look.SkyColor),
            GlowEnabled = look.Glow,
            GlowIntensity = look.GlowIntensity,
            GlowBloom = look.GlowBloom,
        };
        AddChild(new WorldEnvironment { Environment = env });
    }

    async void SaveShot(string path)
    {
        for (int i = 0; i < ShotDelayFrames; i++)
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        var image = GetViewport().GetTexture().GetImage();
        DirAccess.MakeDirRecursiveAbsolute(path.GetBaseDir());
        image.SavePng(path);
        GD.Print($"Saved {path}");
        GetTree().Quit();
    }
}
