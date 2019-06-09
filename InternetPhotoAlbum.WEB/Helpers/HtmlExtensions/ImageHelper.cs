using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace InternetPhotoAlbum.WEB.Helpers
{
    public static class ImageHelper
    {
        public static MvcHtmlString Image(this HtmlHelper htmlHelper, string relativeUrl, string alternateText)
        {
            return Image(htmlHelper, relativeUrl, alternateText, null);
        }

        public static MvcHtmlString Image(this HtmlHelper htmlHelper, string relativeUrl, string alternateText, object htmlAttributes)
        {
            var builder = new TagBuilder("img");

            var urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);
            string fullUrl = urlHelper.Content(relativeUrl);

            builder.MergeAttribute("src", fullUrl);
            builder.MergeAttribute("alt", alternateText);
            builder.MergeAttributes(new RouteValueDictionary(htmlAttributes));

            return MvcHtmlString.Create(builder.ToString(TagRenderMode.SelfClosing));
        }
    }
}