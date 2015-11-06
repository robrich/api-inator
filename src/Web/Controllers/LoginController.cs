namespace ApiInator.Web.Controllers {
    using System.Threading.Tasks;
    using Microsoft.AspNet.Authentication.Cookies;
    using Microsoft.AspNet.Http.Authentication;
    using Microsoft.AspNet.Mvc;

    public class LoginController : Controller {

        public AuthenticationManager Authentication {
            get { return this.HttpContext.Authentication; }
        }

        public IActionResult Index() {
            return new ChallengeResult("GitHub", new AuthenticationProperties {RedirectUri = "/"});
        }
        
        public async Task<IActionResult> Logout() {
            await this.Authentication.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return this.RedirectToAction(nameof(HomeController.Index), "Home");
        }

    }
}
