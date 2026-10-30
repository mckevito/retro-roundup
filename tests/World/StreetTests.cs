using System.Linq;
using Godot;
using RetroRoundup.Characters;
using RetroRoundup.Core;
using RetroRoundup.World;

namespace RetroRoundup.Tests.World;

/// <summary>The street builds its meshes and collision from the data; the player sets up from its table.</summary>
public class StreetTests : TestCase
{
    public void TestStreetBuilds()
    {
        var street = new OutlineStreet();
        street.Build();
        int expected = GameData.Load<StreetTable>(StreetTable.Path).Buildings.Count;
        Check(street.BuildingCount == expected, $"built {street.BuildingCount} buildings, expected {expected}");
        Check(street.LineSegmentCount > 1000, $"only {street.LineSegmentCount} outline segments");
        Check(street.GetNodeOrNull<MeshInstance3D>("Lines")?.Mesh.GetSurfaceCount() == 1, "outline mesh missing");
        Check(street.GetNodeOrNull<MeshInstance3D>("Faces")?.Mesh.GetSurfaceCount() == 1, "face mesh missing");
        int bodies = street.GetChildren().OfType<StaticBody3D>().Count();
        Check(bodies == expected, $"every building needs collision ({bodies} of {expected})");
        street.Free();
    }

    public void TestSameDataSameStreet()
    {
        var a = new OutlineStreet();
        a.Build();
        var b = new OutlineStreet();
        b.Build();
        Check(a.LineSegmentCount == b.LineSegmentCount, "the street must build the same way every time");
        a.Free();
        b.Free();
    }

    public void TestPlayerSetsUpFromData()
    {
        var player = new Player();
        Host.AddChild(player);
        var data = GameData.Load<PlayerTable>(PlayerTable.Path);
        Check(player.Camera != null, "player has no camera");
        Check(Mathf.IsEqualApprox(player.Camera.Position.Y, data.EyeHeightM), "camera should sit at eye height");
        foreach (var action in Player.Actions.Keys)
            Check(InputMap.HasAction(action), $"missing input action {action}");
        player.Look(new Vector2(0, -100000));
        Check(player.Camera.Rotation.X <= Mathf.DegToRad(data.PitchLimitDeg) + 0.001f, "looking up must stop at the pitch limit");
        player.Free();
    }
}
