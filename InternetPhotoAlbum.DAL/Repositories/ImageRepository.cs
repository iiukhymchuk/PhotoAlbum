using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using InternetPhotoAlbum.DAL.Entities;
using InternetPhotoAlbum.DAL.EF;
using InternetPhotoAlbum.DAL.Interfaces;

namespace InternetPhotoAlbum.DAL.Repositories
{
    public class ImageRepository : IRepository<ImageDescription>
    {
        private ApplicationDbContext context = ApplicationDbContext.Create();

        public IEnumerable<ImageDescription> GetList
        {
            get { return context.ImageDescriptions; }
        }

        public ImageDescription Get(string imageId)
        {
            // Преобразуем imageId обратно в Guid
            Guid guidImageId = Guid.Parse(imageId);

            return context.ImageDescriptions.Find(guidImageId);
        }

        public void Create(ImageDescription image)
        {
            context.ImageDescriptions.Add(image);
        }

        public void Update(ImageDescription image)
        {
            throw new NotImplementedException();
        }

        public void Delete(string imageId)
        {
            // Преобразуем imageId обратно в Guid
            Guid guidImageId = Guid.Parse(imageId);

            var imageToRemove = context.ImageDescriptions.SingleOrDefault(f => f.Id == guidImageId);
            if (imageToRemove != null)
            {
                context.ImageDescriptions.Remove(imageToRemove);
            }
        }

        public void Save()
        {
            context.SaveChanges();
        }


        #region IDisposable
        private bool disposed = false;

        public virtual void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                if (context != null)
                {
                    context.Dispose();
                    context = null;
                }
            }
            disposed = true;
        }
        #endregion

    }
}
