using System.Text.Json;
using Godot;

namespace RetroRoundup.Core;

/// <summary>
/// Loads the JSON tables under data/ into typed classes. Every tuning number and colour lives there, never in code.
/// JSON keys are snake_case; unknown keys (such as "_about") are ignored.
/// </summary>
public static class GameData
{
    static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        ReadCommentHandling = JsonCommentHandling.Skip,
    };

    public static T Load<T>(string path) where T : new()
    {
        string text = FileAccess.GetFileAsString(path);
        if (string.IsNullOrEmpty(text))
        {
            GD.PushError($"GameData: cannot read {path}");
            return new T();
        }
        return JsonSerializer.Deserialize<T>(text, Options) ?? new T();
    }

    public static Color ToColor(float[] rgb) => new(rgb[0], rgb[1], rgb[2]);
}
