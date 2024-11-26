using Mafia.Server.Models.AbstractFactory.Roles;
using Mafia.Server.Models.Proxy;

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
    private static readonly Dictionary<string, IRoleImage> _roleImageCache = new(); // Cache of shared images
    private const string DefaultImagePath = "/pictures/default.png"; // Fallback image path

    public static IRoleImage GetRoleImage(string roleName)
    {
        // Check if the image already exists in the cache
        if (!_roleImageCache.ContainsKey(roleName))
        {
            string imagePath = Path.Combine("Models", "Flyweight", "pictures", $"{roleName.ToLower()}.png");

            if (File.Exists(imagePath)) // Add to cache if it exists
            {                
                // PROXY
                var proxyImage = new RoleImageProxy(imagePath); // Lazy loading via proxy
                _roleImageCache[roleName] = proxyImage;
                Console.WriteLine($"Cached role image: {roleName} -> {imagePath}");
            }
            else
            {
                // Log missing image and return a proxy with a default image
                Console.WriteLine($"Image not found for role: {roleName}, using default.");
                var proxyImage = new RoleImageProxy(DefaultImagePath);
                _roleImageCache[roleName] = proxyImage;
            }
        }

        // Return the shared image or proxy image from the cache
        return _roleImageCache[roleName];
    }
}
