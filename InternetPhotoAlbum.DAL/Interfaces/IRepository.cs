using System;
using System.Collections.Generic;
using InternetPhotoAlbum.DAL.Entities;

namespace InternetPhotoAlbum.DAL.Interfaces
{
    public interface IRepository<T> : IDisposable
                                      where T: class
    {

        IEnumerable<T> GetList { get; }
        T Get(string id);
        void Create(T item);
        void Update(T item);
        void Delete(string id);
        void Save();

    }
}