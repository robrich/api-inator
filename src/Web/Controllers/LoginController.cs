namespace ApiInator.Web.Controllers {
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNet.Authentication.Cookies;
    using Microsoft.AspNet.Hosting;
    using Microsoft.AspNet.Http.Authentication;
    using Microsoft.AspNet.Mvc;

    public class LoginController : Controller {
        private readonly IHostingEnvironment env;

        public LoginController(IHostingEnvironment Env) {
            if (Env == null) {
                throw new ArgumentNullException(nameof(Env));
            }
            this.env = Env;
        }

        public AuthenticationManager Authentication {
            get { return this.HttpContext.Authentication; }
        }

        public IActionResult Index() {
            if (this.Request.Host.ToString() != "www.api-inator.com" && !this.env.IsDevelopment()) {
                return this.Redirect("http://www.api-inator.com/login"); // the url must leave from the domain it should return to
            }
            return new ChallengeResult("GitHub", new AuthenticationProperties {RedirectUri = "/"});
        }
        
        public async Task<IActionResult> Logout() {
            await this.Authentication.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return this.RedirectToAction(nameof(HomeController.Index), "Home");
        }

    }
}
