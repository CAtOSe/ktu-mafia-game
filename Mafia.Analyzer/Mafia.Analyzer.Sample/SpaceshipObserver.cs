using System;

namespace Mafia.Analyzer.Sample;

public class SpaceshipObserver : ITestObserver
{
    public void SetSpeed(long speed)
    {
        if (speed > 299_792_458)
            throw new ArgumentOutOfRangeException(nameof(speed));
    }

    public void Log(string level, string message, string dateTime)
    {
        throw new NotImplementedException();
    }
}