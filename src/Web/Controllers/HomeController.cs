namespace ApiInator.Web.Controllers {
    using Microsoft.AspNet.Mvc;

    public class HomeController : Controller {

        public IActionResult Index() {
            return this.View();
        }

        public IActionResult About() {
            return this.View();
        }

        public IActionResult Error() {
            return this.View("~/Views/Shared/Error.cshtml");
        }

        public IActionResult NotFound() {
            return this.View("NotFound");
        }

    }
}
