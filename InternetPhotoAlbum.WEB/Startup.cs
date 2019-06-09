using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(InternetPhotoAlbum.WEB.Startup))]
namespace InternetPhotoAlbum.WEB
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
