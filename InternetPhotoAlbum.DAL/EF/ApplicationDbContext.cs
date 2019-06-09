using System;

using Microsoft.AspNet.Identity.EntityFramework;
using InternetPhotoAlbum.DAL.Entities;
using System.Data.Entity;


namespace InternetPhotoAlbum.DAL.EF
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<ImageDescription> ImageDescriptions { get; set; }
    }
}
