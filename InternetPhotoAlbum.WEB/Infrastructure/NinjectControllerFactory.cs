using InternetPhotoAlbum.BLL.Interfaces;
using InternetPhotoAlbum.BLL.Services;
using InternetPhotoAlbum.DAL.Entities;
using InternetPhotoAlbum.DAL.Interfaces;
using InternetPhotoAlbum.DAL.Repositories;
using InternetPhotoAlbum.WEB;
using Ninject;
using System;
using System.Web.Mvc;
using System.Web.Routing;
public class NinjectControllerFactory : DefaultControllerFactory
{
    private IKernel ninjectKernel;

    public NinjectControllerFactory()
    {
        ninjectKernel = new StandardKernel();
        AddBindings();
    }

    protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
    {
        return controllerType == null
                   ? null
                   : (IController)ninjectKernel.Get(controllerType);
    }

    private void AddBindings()
    {
        ninjectKernel.Bind<IImageService>().To<ImageService>();
        ninjectKernel.Bind<IUserService>().To<UserService>();
        ninjectKernel.Bind<IRepository<ImageDescription>>().To<ImageRepository>();
        ninjectKernel.Bind<IRepository<ApplicationUser>>().To<UserRepository>();
    }
}