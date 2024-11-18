using Mafia.Server.Models.AbstractFactory.Roles;

namespace Mafia.Server.Models.Flyweight;

public class RoleImageFactory
{
    private static readonly Dictionary<string, string> _roleImages = new();

    /*public static string GetRoleImage(Role role)
    {
        if (!_roleImages.ContainsKey(role.RoleType))
        {
            // Assuming a base path for images
           
            _roleImages[role.RoleType] = imagePath;
            Console.WriteLine("Show role" + role.RoleType +" image, which location is: " + imagePath);
        }
        return _roleImages[role.RoleType];
    }*/
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
}