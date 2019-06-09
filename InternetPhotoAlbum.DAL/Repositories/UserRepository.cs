using System;
using System.Collections.Generic;
using System.Data.Entity;
using InternetPhotoAlbum.DAL.Entities;
using InternetPhotoAlbum.DAL.EF;
using InternetPhotoAlbum.DAL.Interfaces;
using System.Linq;

namespace InternetPhotoAlbum.DAL.Repositories
{
    public class UserRepository : IRepository<ApplicationUser>
    {
        private ApplicationUser fakeUser = new ApplicationUser();

        private ApplicationDbContext context = ApplicationDbContext.Create();

        public IEnumerable<ApplicationUser> GetList
        {
            get { return context.Users; }
        }

        public ApplicationUser Get(string userId)
        {
            var user = context.Users.Find(userId);
            if (user == null) return fakeUser;
            return user;
        }

        public void Create(ApplicationUser user)
        {
            context.Users.Add(user);
        }

        public void Update(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public void Delete(string userId)
        {
            // Приводим список пользователей к IQueryable<ApplicationUser>, чтобы воспользоваться методом SingleOrDefault
            IQueryable<ApplicationUser> users = context.Users;
            var userToRemove = users.SingleOrDefault(f => f.Id == userId);
            if (userToRemove != null)
            {
                context.Users.Remove(userToRemove);
                // Будут ли удаляться из базы картинки?
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
