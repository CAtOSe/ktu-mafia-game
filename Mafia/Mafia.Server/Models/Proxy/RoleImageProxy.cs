using Mafia.Server.Models.Flyweight;

namespace Mafia.Server.Models.Proxy
{
    public class RoleImageProxy : IRoleImage
    {
        private readonly string _imagePath;
        private RoleImage _realRoleImage;

        public RoleImageProxy(string imagePath)
        {
            _imagePath = imagePath;
            _realRoleImage = null;
        }

        public string GetImagePath()
        {
            if (_realRoleImage == null)
            {
                _realRoleImage = new RoleImage(_imagePath);
            }

            return _realRoleImage.GetImagePath();
        }

        public void Render()
        {
            _realRoleImage ??= new RoleImage(_imagePath);

            _realRoleImage.Render();
        }
    }
}
