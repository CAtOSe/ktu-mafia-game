﻿// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace Mafia.Analyzer.Sample;

// If you don't see warnings, build the Analyzers Project.

public class Examples
{
    public class MyCompanyClass // Try to apply quick fix using the IDE.
    {
    }

    public void ToStars()
    {
        var spaceship = new SpaceshipObserver();
        spaceship.SetSpeed(300000000); // Invalid value, it should be highlighted.
        spaceship.SetSpeed(42);
    }
}