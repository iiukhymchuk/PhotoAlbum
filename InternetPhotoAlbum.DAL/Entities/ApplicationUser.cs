﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Security.Claims;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InternetPhotoAlbum.DAL.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(128)]
        [RequiredAttribute]
        public string NickName { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int UserSpaceId { get; set; }

        public virtual ICollection<ImageDescription> UserImages { get; set; }
    }
}