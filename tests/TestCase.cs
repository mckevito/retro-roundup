using System.Collections.Generic;
using Godot;

namespace RetroRoundup.Tests;

/// <summary>Base for tests. Every public method whose name starts with "Test" runs once on a fresh instance.</summary>
public abstract class TestCase
{
    public readonly List<string> Failures = new();

    /// <summary>The runner node, for tests that need the scene tree.</summary>
    public Node Host;

    protected void Check(bool condition, string message)
    {
        if (!condition)
            Failures.Add(message);
    }
}
