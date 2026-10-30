using System.Collections.Generic;

namespace RetroRoundup.Core;

// Typed copies of the data/ tables. Property names map to the JSON's snake_case keys.

/// <summary>data/look.json: colours and line settings for the outline look.</summary>
public class LookTable
{
    public const string Path = "res://data/look.json";
    public float[] LineColor { get; set; } = { 0.88f, 0.27f, 0.9f };
    public float[] FaceColor { get; set; } = { 0, 0, 0 };
    public float[] SkyColor { get; set; } = { 0, 0, 0 };
    public bool Glow { get; set; }
    public float GlowIntensity { get; set; }
    public float GlowBloom { get; set; }
    public float FaceInsetM { get; set; }
    public float DetailOffsetM { get; set; }
}

/// <summary>data/player.json: first-person controls.</summary>
public class PlayerTable
{
    public const string Path = "res://data/player.json";
    public float EyeHeightM { get; set; }
    public float WalkSpeed { get; set; }
    public float RunSpeed { get; set; }
    public float Acceleration { get; set; }
    public float MouseSensitivity { get; set; }
    public float PitchLimitDeg { get; set; }
    public float CapsuleRadiusM { get; set; }
    public float CapsuleHeightM { get; set; }
    public float Gravity { get; set; }
    public float[] StartPosition { get; set; } = { 0, 0, 0 };
    public LookAngles StartLookDeg { get; set; } = new();

    public class LookAngles
    {
        public float Yaw { get; set; }
        public float Pitch { get; set; }
    }
}

/// <summary>data/street.json: the proof-of-concept street.</summary>
public class StreetTable
{
    public const string Path = "res://data/street.json";
    public float RoadHalfWidthM { get; set; }
    public float CurbHeightM { get; set; }
    public float GutterOffsetM { get; set; }
    public float BuildingLineM { get; set; }
    public float StreetLengthM { get; set; }
    public float RoadBlockingAllowedBeyondM { get; set; }
    public DashTable CentreDash { get; set; } = new();
    public float CentreLineGapM { get; set; }
    public float PavementJointEveryM { get; set; }
    public float PavementJointsUntilM { get; set; }
    public CrosswalkTable Crosswalk { get; set; } = new();
    public LightsTable StreetLights { get; set; } = new();
    public FacadeTable Facade { get; set; } = new();
    public List<Building> Buildings { get; set; } = new();

    public class DashTable
    {
        public float LengthM { get; set; }
        public float GapM { get; set; }
    }

    public class CrosswalkTable
    {
        public float Z0 { get; set; }
        public float Z1 { get; set; }
        public int Stripes { get; set; }
        public float StripeWidthM { get; set; }
        public float StripeStepM { get; set; }
    }

    public class LightsTable
    {
        public float HeightM { get; set; }
        public float ArmM { get; set; }
        public float ArmRiseM { get; set; }
        public float HeadHalfLengthM { get; set; }
        public float HeadHalfWidthM { get; set; }
        public float[] LeftZ { get; set; } = System.Array.Empty<float>();
        public float[] RightZ { get; set; } = System.Array.Empty<float>();
        public float OffsetFromCurbM { get; set; }
    }

    public class FacadeTable
    {
        public float WindowSkipChance { get; set; }
        public float WindowWidthShare { get; set; }
        public float WindowMaxWidthM { get; set; }
        public float WindowHeightShare { get; set; }
        public float WindowSillShare { get; set; }
        public float ShopWindowHeightShare { get; set; }
        public float ShopWindowSillM { get; set; }
        public float DoorWidthM { get; set; }
        public float DoorHeightM { get; set; }
        public float AwningShare { get; set; }
        public float BalconyDepthM { get; set; }
        public float BalconyRailM { get; set; }
        public float BalconyGapM { get; set; }
        public float BalconyDoorWidthM { get; set; }
        public float BalconyDoorHeightShare { get; set; }
        public ulong Seed { get; set; }
    }

    /// <summary>A box building: x0..x1 across the street, z0..z1 along it, metres.</summary>
    public class Building
    {
        public string Name { get; set; } = "";
        public float X0 { get; set; }
        public float X1 { get; set; }
        public float Z0 { get; set; }
        public float Z1 { get; set; }
        public float Height { get; set; }
        public int Floors { get; set; }
        /// <summary>"left" when the building stands right of the road (its left face looks onto it), else "right".</summary>
        public string StreetSide { get; set; } = "left";
        public int BaysSide { get; set; }
        public int BaysFront { get; set; }
        public int[] BalconyBays { get; set; } = System.Array.Empty<int>();
        public bool Shop { get; set; }
    }
}
