using System;
using System.Linq;
using Godot;
using RetroRoundup.Core;

namespace RetroRoundup.World;

/// <summary>
/// Builds the proof-of-concept street from data/street.json in the outline look from data/look.json.
/// Street coordinates: x metres right of the road's centre, y up, z metres ahead of the start.
/// Godot looks down -Z, so street z maps to Godot -z (see P).
/// </summary>
public partial class OutlineStreet : Node3D
{
    public StreetTable Street { get; private set; }
    public LookTable Look { get; private set; }
    public int BuildingCount { get; private set; }
    public int LineSegmentCount { get; private set; }

    OutlineBuilder _out;
    RandomNumberGenerator _rng;

    /// <summary>A point on a building face: u metres along it, y up, out metres in front of it.</summary>
    delegate Vector3 Face(float u, float y, float outward);

    public override void _Ready() => Build();

    public void Build()
    {
        Street = GameData.Load<StreetTable>(StreetTable.Path);
        Look = GameData.Load<LookTable>(LookTable.Path);
        _out = new OutlineBuilder { FaceInset = Look.FaceInsetM };
        _rng = new RandomNumberGenerator { Seed = Street.Facade.Seed };
        AddGround();
        foreach (var b in Street.Buildings)
            AddBuilding(b);
        AddStreetLights();
        MakeMeshes();
        BuildingCount = Street.Buildings.Count;
        LineSegmentCount = _out.Lines.Count / 2;
    }

    static Vector3 P(float x, float y, float z) => new(x, y, -z);

    // --- ground ----------------------------------------------------------------------------------

    void AddGround()
    {
        var s = Street;
        foreach (float side in new[] { -1f, 1f })
        {
            _out.AddLine(P(side * s.RoadHalfWidthM, s.CurbHeightM, 0), P(side * s.RoadHalfWidthM, s.CurbHeightM, s.StreetLengthM));
            float gutter = s.RoadHalfWidthM + s.GutterOffsetM;
            _out.AddLine(P(side * gutter, 0, 0), P(side * gutter, 0, s.StreetLengthM));
            _out.AddLine(P(side * s.BuildingLineM, s.CurbHeightM, 0), P(side * s.BuildingLineM, s.CurbHeightM, s.StreetLengthM));
        }
        for (float z = 1; z < s.StreetLengthM; z += s.CentreDash.LengthM + s.CentreDash.GapM)
        {
            foreach (float x in new[] { -s.CentreLineGapM, s.CentreLineGapM })
                _out.AddLine(P(x, 0, z), P(x, 0, z + s.CentreDash.LengthM));
        }
        for (float z = s.PavementJointEveryM; z < s.PavementJointsUntilM; z += s.PavementJointEveryM)
        {
            foreach (float side in new[] { -1f, 1f })
                _out.AddLine(P(side * s.RoadHalfWidthM, s.CurbHeightM, z), P(side * s.BuildingLineM, s.CurbHeightM, z));
        }
        var cw = s.Crosswalk;
        float first = -s.RoadHalfWidthM + (2 * s.RoadHalfWidthM - (cw.Stripes - 1) * cw.StripeStepM - cw.StripeWidthM) * 0.5f;
        for (int i = 0; i < cw.Stripes; i++)
        {
            float x0 = first + i * cw.StripeStepM;
            float x1 = x0 + cw.StripeWidthM;
            _out.AddLoop(P(x0, 0, cw.Z0), P(x1, 0, cw.Z0), P(x1, 0, cw.Z1), P(x0, 0, cw.Z1));
        }
    }

    // --- buildings -------------------------------------------------------------------------------

    void AddBuilding(StreetTable.Building b)
    {
        _out.AddBox(P(b.X0, 0, b.Z1), P(b.X1, b.Height, b.Z0));
        float fh = b.Height / b.Floors;
        bool rightOfRoad = b.StreetSide == "left";
        float streetX = rightOfRoad ? b.X0 : b.X1;
        float normalX = rightOfRoad ? -1f : 1f;

        Face streetFace = (u, y, o) => P(streetX + normalX * o, y, b.Z0 + u);
        Face frontFace = (u, y, o) => P(b.X0 + u, y, b.Z0 - o);
        Face backFace = (u, y, o) => P(b.X0 + u, y, b.Z1 + o);

        AddFacade(streetFace, b.Z1 - b.Z0, b.BaysSide, b.Floors, fh, b.Shop, b.BalconyBays);
        AddFacade(frontFace, b.X1 - b.X0, b.BaysFront, b.Floors, fh, false, Array.Empty<int>());
        AddFacade(backFace, b.X1 - b.X0, b.BaysFront, b.Floors, fh, false, Array.Empty<int>());
        AddBalconies(streetFace, b, fh);
        AddCollision(b);
    }

    void AddFacade(Face face, float width, int bays, int floors, float fh, bool shop, int[] skipBays)
    {
        var f = Street.Facade;
        float off = Look.DetailOffsetM;
        for (int k = 1; k < floors; k++)
            _out.AddLine(face(0, k * fh, off), face(width, k * fh, off));

        float bay = width / bays;
        float winW = Mathf.Min(bay * f.WindowWidthShare, f.WindowMaxWidthM);
        int firstFloor = shop ? 1 : 0;
        for (int k = firstFloor; k < floors; k++)
        {
            float y = k * fh + fh * f.WindowSillShare;
            for (int i = 0; i < bays; i++)
            {
                if (skipBays.Contains(i) || _rng.Randf() < f.WindowSkipChance)
                    continue;
                Window(face, bay * (i + 0.5f), y, winW, fh * f.WindowHeightShare, true);
            }
        }
        if (!shop)
            return;
        for (int i = 0; i < bays; i++)
        {
            if (i == bays / 2)
                Window(face, bay * (i + 0.5f), 0, f.DoorWidthM, f.DoorHeightM, true);
            else
                Window(face, bay * (i + 0.5f), f.ShopWindowSillM, bay * 0.8f, fh * f.ShopWindowHeightShare, false);
        }
        float awning = fh * f.AwningShare;
        _out.AddLine(face(0, awning, off), face(width, awning, off));
    }

    void Window(Face face, float centre, float y, float w, float h, bool twin)
    {
        float off = Look.DetailOffsetM;
        float l = centre - w * 0.5f;
        float r = centre + w * 0.5f;
        _out.AddLoop(face(l, y, off), face(l, y + h, off), face(r, y + h, off), face(r, y, off));
        if (twin)
            _out.AddLine(face(centre, y, off), face(centre, y + h, off));
    }

    void AddBalconies(Face face, StreetTable.Building b, float fh)
    {
        var f = Street.Facade;
        float bay = (b.Z1 - b.Z0) / b.BaysSide;
        int firstFloor = b.Shop ? 1 : 0;
        foreach (int i in b.BalconyBays)
        {
            float u0 = bay * i + f.BalconyGapM;
            float u1 = bay * (i + 1) - f.BalconyGapM;
            for (int k = firstFloor; k < b.Floors; k++)
            {
                float y = k * fh;
                _out.AddBox(face(u0, y, 0), face(u1, y + f.BalconyRailM, f.BalconyDepthM));
                foreach (float t in new[] { 0.33f, 0.66f })
                {
                    float u = Mathf.Lerp(u0, u1, t);
                    _out.AddLine(face(u, y, f.BalconyDepthM), face(u, y + f.BalconyRailM, f.BalconyDepthM));
                }
                Window(face, (u0 + u1) * 0.5f, y + 0.05f, f.BalconyDoorWidthM, fh * f.BalconyDoorHeightShare, true);
            }
        }
    }

    void AddCollision(StreetTable.Building b)
    {
        Vector3 lo = P(b.X0, 0, b.Z1);
        Vector3 hi = P(b.X1, b.Height, b.Z0);
        var body = new StaticBody3D { Position = (lo + hi) * 0.5f };
        body.AddChild(new CollisionShape3D { Shape = new BoxShape3D { Size = hi - lo } });
        AddChild(body);
    }

    // --- street furniture ------------------------------------------------------------------------

    void AddStreetLights()
    {
        var sl = Street.StreetLights;
        float x = Street.RoadHalfWidthM + sl.OffsetFromCurbM;
        foreach (float z in sl.LeftZ)
            Lamp(-x, z, sl.ArmM);
        foreach (float z in sl.RightZ)
            Lamp(x, z, -sl.ArmM);
    }

    void Lamp(float x, float z, float arm)
    {
        var sl = Street.StreetLights;
        float h = sl.HeightM;
        _out.AddLine(P(x, Street.CurbHeightM, z), P(x, h, z));
        _out.AddLine(P(x, h, z), P(x + arm, h + sl.ArmRiseM, z));
        _out.AddLine(P(x + arm, h + sl.ArmRiseM, z), P(x + arm, h + sl.ArmRiseM * 0.25f, z));
        float hx = x + arm;
        _out.AddBox(P(hx - sl.HeadHalfLengthM, h - 0.05f, z - sl.HeadHalfWidthM), P(hx + sl.HeadHalfLengthM, h + 0.1f, z + sl.HeadHalfWidthM));
    }

    // --- meshes ----------------------------------------------------------------------------------

    void MakeMeshes()
    {
        var faceMat = new StandardMaterial3D
        {
            ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
            AlbedoColor = GameData.ToColor(Look.FaceColor),
            CullMode = BaseMaterial3D.CullModeEnum.Disabled,
        };
        var lineMat = new StandardMaterial3D
        {
            ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
            AlbedoColor = GameData.ToColor(Look.LineColor),
        };
        AddChild(new MeshInstance3D { Name = "Faces", Mesh = _out.BuildFacesMesh(faceMat) });
        AddChild(new MeshInstance3D { Name = "Lines", Mesh = _out.BuildLinesMesh(lineMat) });
    }
}
