using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InternetPhotoAlbum.WEB.Helpers
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class FileLengthAttribute : ValidationAttribute
    {
        private int megabytes { get; set; }

        public FileLengthAttribute(int megabytes)
        {
            this.megabytes = megabytes;
        }

        public FileLengthAttribute(int megabytes, string errorMessage)
            : this(megabytes)
        {
            ErrorMessage = errorMessage;
        }

        public override bool IsValid(object value)
        {
            HttpPostedFileBase file = value as HttpPostedFileBase;
            if (file != null)
            {
                return file.ContentLength < megabytes * 1024 * 1024;
            }
            // Вернуть true если файл null
            // Чтобы не влиять на другие валидации
            return true;
        }

    }
}