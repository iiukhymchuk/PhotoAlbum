using InternetPhotoAlbum.DAL.Entities;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;

namespace InternetPhotoAlbum.BLL.DTO
{
    public class ApplicationUserDTO
    {
        public virtual string NickName { get; set; }
        public virtual int UserSpaceId { get; set; }
        public virtual ICollection<ImageDescription> UserImages { get; set; }
        public virtual int AccessFailedCount { get; set; }
        public virtual ICollection<IdentityUserClaim> Claims { get; set; }
        public virtual string Email { get; set; }
        public virtual bool EmailConfirmed { get; set; }
        public virtual string Id { get; set; }
        public virtual bool LockoutEnabled { get; set; }
        public virtual DateTime? LockoutEndDateUtc { get; set; }
        public virtual ICollection<IdentityUserLogin> Logins { get; set; }
        public virtual string PasswordHash { get; set; }
        public virtual string PhoneNumber { get; set; }
        public virtual bool PhoneNumberConfirmed { get; set; }
        public virtual ICollection<IdentityUserRole> Roles { get; set; }
        public virtual string SecurityStamp { get; set; }
        public virtual bool TwoFactorEnabled { get; set; }
        public virtual string UserName { get; set; }
    }
}
