namespace ApiInator.Web.Helpers {
    using System.Security.Claims;
    using Microsoft.AspNet.Mvc.Rendering;
    using Microsoft.AspNet.Mvc.ViewFeatures;

    public static class UserHelpers {

        // FRAGILE: Duplicates UserCurrentService.cs

        private static ClaimsPrincipal GetUser(IHtmlHelper HtmlHelper) {
            return HtmlHelper.ViewContext.HttpContext.User;
        }

        public static bool IsAuthenticated(this IHtmlHelper HtmlHelper) {
            return GetUser(HtmlHelper).Identity.IsAuthenticated;
        }

        public static int? CurrentUserId(this IHtmlHelper HtmlHelper) {
            if (!HtmlHelper.IsAuthenticated()) {
                return null;
            }
            int userId = 0;
            if (!int.TryParse(GetUser(HtmlHelper).Identity.Name, out userId)) {
                return null;
            }
            return userId;
        }

        public static string CurrentUserName(this IHtmlHelper HtmlHelper) {
            if (!HtmlHelper.IsAuthenticated()) {
                return null;
            }
            var user = GetUser(HtmlHelper);
            const string githubNameKey = "urn:github:name";
            bool hasClaim = user.HasClaim(c => c.Type == githubNameKey);
            string name = null;
            if (hasClaim) {
                name = user.FindFirst(c => c.Type == githubNameKey).Value;
            }
            return name;
        }

        public static string CurrentUserAvitarUrl(this IHtmlHelper HtmlHelper) {
            if (!HtmlHelper.IsAuthenticated()) {
                return null;
            }
            var user = GetUser(HtmlHelper);
            const string githubNameKey = "urn:github:avitar";
            bool hasClaim = user.HasClaim(c => c.Type == githubNameKey);
            string name = null;
            if (hasClaim) {
                name = user.FindFirst(c => c.Type == githubNameKey).Value;
            }
            return name;
        }

    }
}
