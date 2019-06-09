using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing.Imaging;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace InternetPhotoAlbum.WEB.Helpers
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ImageExtensionsAttribute : ValidationAttribute
    {
        private List<string> ValidExtensions { get; set; }
        private List<ImageFormat> ValidImageFormats { get; set; }

        public ImageExtensionsAttribute(string fileExtensions)
        {
            ValidExtensions = fileExtensions.Split('|').ToList();
            // Заменить jpg на jpeg, так как нет такого ImageFormat.Jpg
            var v = fileExtensions.Split('|').ToList();
            if (v.Contains("jpg"))
            {
                v.Add("jpeg");
                v.Remove("jpg");
            }
            
            ValidImageFormats = v.Select(x =>(ImageFormat)(typeof(ImageFormat))
                .GetProperty(x.Substring(0, 1).ToUpper() + x.Substring(1).ToLower())
                .GetValue(null)).ToList();
        }

        public ImageExtensionsAttribute(string fileExtensions, string errorMessage)
            : this(fileExtensions)
        {
            ErrorMessage = errorMessage;
        }

        public override bool IsValid(object value)
        {
            HttpPostedFileBase file = value as HttpPostedFileBase;
            if (file != null)
            {
                // Проверка на окончание названия файла
                var fileName = file.FileName;
                var isValidExtension = ValidExtensions.Any(y => fileName.EndsWith(y, StringComparison.OrdinalIgnoreCase));
                if (!isValidExtension)
                {
                    return false;
                }
                try
                {
                    using (var img = System.Drawing.Image.FromStream(file.InputStream))
                    {
                        return ValidImageFormats.Contains(img.RawFormat);
                    }
                }
                catch
                {
                    return false;
                }
            }
            // Вернуть true если файл null
            // Чтобы не влиять на другие валидации
            return true;
        }
    }
}