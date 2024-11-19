namespace Mafia.Server.Models.Flyweight;

public class RoleImage : IRoleImage
{
    private readonly string _imagePath; // Intrinsic state

    public RoleImage(string imagePath)
    {
        _imagePath = imagePath;
    }

    public string GetImagePath()
    {
        return _imagePath;
    }

    public void Render()
    {
        Console.WriteLine($"Rendering image from path: {_imagePath}");
    }
}
