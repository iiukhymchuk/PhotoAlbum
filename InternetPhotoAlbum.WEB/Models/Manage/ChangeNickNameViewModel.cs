using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InternetPhotoAlbum.WEB.Models
{
    public class ChangeNickNameViewModel : SharedLayoutViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Текущий пароль")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(16, ErrorMessage = "{0} должно быть минимум {2} по длине", MinimumLength = 3)]
        [Display(Name = "Новое имя пользователя")]
        public string NewNickName { get; set; }
    }
}