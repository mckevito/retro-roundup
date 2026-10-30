using System.Collections.Generic;
using Godot;
using RetroRoundup.Core;

namespace RetroRoundup.Characters;

/// <summary>
/// First-person walker. Numbers come from data/player.json.
/// Click to capture the mouse, Esc to release it. WASD or arrows to move, Shift to run.
/// </summary>
public partial class Player : CharacterBody3D
{
    public static readonly Dictionary<string, Key[]> Actions = new()
    {
        ["move_forward"] = new[] { Key.W, Key.Up },
        ["move_back"] = new[] { Key.S, Key.Down },
        ["move_left"] = new[] { Key.A, Key.Left },
        ["move_right"] = new[] { Key.D, Key.Right },
        ["run"] = new[] { Key.Shift },
        ["release_mouse"] = new[] { Key.Escape },
    };

    public PlayerTable Data { get; private set; }
    public Camera3D Camera { get; private set; }
    float _pitch;

    public override void _Ready()
    {
        Data = GameData.Load<PlayerTable>(PlayerTable.Path);
        EnsureActions();
        var shape = new CapsuleShape3D { Radius = Data.CapsuleRadiusM, Height = Data.CapsuleHeightM };
        AddChild(new CollisionShape3D { Shape = shape, Position = new Vector3(0, shape.Height * 0.5f, 0) });
        Camera = new Camera3D { Position = new Vector3(0, Data.EyeHeightM, 0), Current = true };
        AddChild(Camera);
        var start = Data.StartPosition;
        Position = new Vector3(start[0], start[1], start[2]);
        Rotation = new Vector3(0, Mathf.DegToRad(Data.StartLookDeg.Yaw), 0);
        _pitch = Mathf.DegToRad(Data.StartLookDeg.Pitch);
        Camera.Rotation = new Vector3(_pitch, 0, 0);
    }

    static void EnsureActions()
    {
        foreach (var (action, keys) in Actions)
        {
            if (InputMap.HasAction(action))
                continue;
            InputMap.AddAction(action);
            foreach (var key in keys)
                InputMap.ActionAddEvent(action, new InputEventKey { PhysicalKeycode = key });
        }
    }

    public override void _UnhandledInput(InputEvent e)
    {
        if (e is InputEventMouseButton { Pressed: true })
            Input.MouseMode = Input.MouseModeEnum.Captured;
        else if (e.IsActionPressed("release_mouse"))
            Input.MouseMode = Input.MouseModeEnum.Visible;
        else if (e is InputEventMouseMotion motion && Input.MouseMode == Input.MouseModeEnum.Captured)
            Look(motion.Relative);
    }

    /// <summary>Turns the body left and right and tilts the camera up and down, stopping at the pitch limit.</summary>
    public void Look(Vector2 relative)
    {
        float sens = Data.MouseSensitivity;
        RotateY(-relative.X * sens);
        float limit = Mathf.DegToRad(Data.PitchLimitDeg);
        _pitch = Mathf.Clamp(_pitch - relative.Y * sens, -limit, limit);
        Camera.Rotation = new Vector3(_pitch, 0, 0);
    }

    public override void _PhysicsProcess(double delta)
    {
        float dt = (float)delta;
        Vector2 input = Input.GetVector("move_left", "move_right", "move_forward", "move_back");
        float speed = Input.IsActionPressed("run") ? Data.RunSpeed : Data.WalkSpeed;
        Vector3 wish = (Transform.Basis * new Vector3(input.X, 0, input.Y)).Normalized() * speed;
        float step = Data.Acceleration * speed * dt;
        Vector3 v = Velocity;
        v.X = Mathf.MoveToward(v.X, wish.X, step);
        v.Z = Mathf.MoveToward(v.Z, wish.Z, step);
        if (!IsOnFloor())
            v.Y -= Data.Gravity * dt;
        Velocity = v;
        MoveAndSlide();
    }
}
