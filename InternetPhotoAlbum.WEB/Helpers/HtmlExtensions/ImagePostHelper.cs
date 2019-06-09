using InternetPhotoAlbum.BLL.DTO;
using InternetPhotoAlbum.WEB.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;

namespace InternetPhotoAlbum.WEB.Helpers
{
    public static class ImagePostHelper
    {
        // Возвратить div tag с вложеным названием, описанием и самой картинкой
        public static MvcHtmlString ImagePost(this HtmlHelper html, string imageRelativeUrl,
            string imageName, string imageDescription)
        {
            StringBuilder innerHtml = new StringBuilder();
            var divBuilder = new TagBuilder("div");
            divBuilder.AddCssClass("div-image-post");

            var pNameBuilder = new TagBuilder("p");
            pNameBuilder.MergeAttribute("name", "image-post-name");
            pNameBuilder.AddCssClass("image-post-name");
            pNameBuilder.InnerHtml = imageName;

            var pDescriptionBuilder = new TagBuilder("p");
            pDescriptionBuilder.MergeAttribute("name", "image-post-description");
            pDescriptionBuilder.AddCssClass("image-post-description");
            pDescriptionBuilder.InnerHtml = imageDescription;

            var imageTag = html.Image(imageRelativeUrl, imageName, new { @class = "img-responsive image-avatar img-rounded center-block" });
            innerHtml.Append(pNameBuilder.ToString());
            innerHtml.Append(pDescriptionBuilder.ToString());
            innerHtml.Append(imageTag);
            innerHtml.Append(new TagBuilder("br").ToString(TagRenderMode.SelfClosing));

            divBuilder.InnerHtml = innerHtml.ToString();
            return MvcHtmlString.Create(divBuilder.ToString());
        }
    }
}