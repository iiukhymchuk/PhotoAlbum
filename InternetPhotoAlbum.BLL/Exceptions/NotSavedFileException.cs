using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternetPhotoAlbum.BLL
{
    public class NotSavedFileException : Exception
    {
        public NotSavedFileException()
        {
        }

        public NotSavedFileException(string message)
            : base(message)
        {
        }

        public NotSavedFileException(string message, Exception ex)
            : base(message, ex)
        {
        }
    }
}
