using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot;

namespace RetroRoundup.Tests;

/// <summary>
/// Headless test runner: godot --headless res://tests/TestRunner.tscn [-- --filter=Name]
/// Runs every Test* method on every TestCase, writes tests/results.txt and exits 1 on any failure.
/// </summary>
public partial class TestRunner : Node
{
    public override void _Ready()
    {
        string filter = OS.GetCmdlineUserArgs()
            .Where(a => a.StartsWith("--filter="))
            .Select(a => a["--filter=".Length..])
            .FirstOrDefault() ?? "";
        var lines = new List<string>();
        int passed = 0, failed = 0;
        var cases = typeof(TestRunner).Assembly.GetTypes()
            .Where(t => t.IsSubclassOf(typeof(TestCase)) && !t.IsAbstract)
            .OrderBy(t => t.Name);
        foreach (var type in cases)
        {
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(m => m.Name.StartsWith("Test") && m.GetParameters().Length == 0)
                .OrderBy(m => m.MetadataToken);
            foreach (var method in methods)
            {
                string name = $"{type.Name}.{method.Name}";
                if (filter != "" && !name.Contains(filter))
                    continue;
                var test = (TestCase)Activator.CreateInstance(type);
                test.Host = this;
                try
                {
                    method.Invoke(test, null);
                }
                catch (TargetInvocationException e)
                {
                    test.Failures.Add($"threw {e.InnerException?.GetType().Name}: {e.InnerException?.Message}");
                }
                if (test.Failures.Count == 0)
                {
                    passed++;
                    lines.Add($"PASS {name}");
                }
                else
                {
                    failed++;
                    lines.AddRange(test.Failures.Select(f => $"FAIL {name}: {f}"));
                }
            }
        }
        lines.Add($"{passed} passed, {failed} failed");
        string report = string.Join("\n", lines);
        GD.Print(report);
        using (var file = FileAccess.Open("res://tests/results.txt", FileAccess.ModeFlags.Write))
            file?.StoreString(report + "\n");
        GetTree().Quit(failed > 0 || passed == 0 ? 1 : 0);
    }
}
