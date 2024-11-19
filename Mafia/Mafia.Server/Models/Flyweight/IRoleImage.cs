namespace Mafia.Server.Models.Flyweight;

public interface IRoleImage
{
    string GetImagePath(); // Returns the image file path
    void Render(); // Optional: Simulate rendering the image
}