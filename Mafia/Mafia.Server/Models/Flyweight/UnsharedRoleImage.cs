namespace Mafia.Server.Models.Flyweight;

public class UnsharedRoleImage : IRoleImage
{
    private readonly string _uniqueImagePath; // Unique, non-shared state

    public UnsharedRoleImage(string uniqueImagePath)
    {
        _uniqueImagePath = uniqueImagePath;
    }

    public string GetImagePath()
    {
        return _uniqueImagePath;
    }

    public void Render()
    {
        Console.WriteLine($"Rendering unshared image from path: {_uniqueImagePath}");
    }
}
