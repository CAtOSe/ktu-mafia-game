using Mafia.Server.Models.AbstractFactory.Roles;

/*namespace Mafia.Server.Models.Flyweight;

public class RoleImageFactory
{
    private static readonly Dictionary<string, string> _roleImages = new();
    public static string GetRoleImage(string roleName)
    {
        if (!_roleImages.ContainsKey(roleName))
        {
            // Replace with actual path logic
            var imagePath = $"/pictures/{roleName.ToLower()}.png";
            _roleImages[roleName] = imagePath;
            Console.WriteLine("Show role" + roleName +" image, which location is: " + imagePath);
        }
        return _roleImages[roleName];
    }
}*/
namespace Mafia.Server.Models.Flyweight;

public static class RoleImageFactory
{
    private static readonly Dictionary<string, RoleImage> _roleImageCache = new(); // Cache of shared images
    private const string DefaultImagePath = "/pictures/default.png"; // Fallback image path

    public static IRoleImage GetRoleImage(string roleName)
    {
        // Check if the image already exists in the cache
        if (!_roleImageCache.ContainsKey(roleName))
        {
            string imagePath = Path.Combine("Models", "Flyweight", "pictures", $"{roleName.ToLower()}.png");

            if (File.Exists(imagePath))
            {
                // Add to cache if it exists
                var roleImage = new RoleImage(imagePath);
                _roleImageCache[roleName] = roleImage;
                Console.WriteLine($"Cached role image: {roleName} -> {imagePath}");
            }
            else
            {
                // Log missing image and return an UnsharedConcreteFlyweight
                Console.WriteLine($"Image not found for role: {roleName}, using default.");
                return new UnsharedRoleImage(DefaultImagePath);
            }
        }

        // Return the shared image from the cache
        return _roleImageCache[roleName];
    }
}
