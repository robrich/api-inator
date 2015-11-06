namespace ApiInator.Web.Controllers {
    using System;
    using System.Collections.Generic;
    using ApiInator.Web.Models;
    using ApiInator.Web.Services;
    using Microsoft.AspNet.Authorization;
    using Microsoft.AspNet.Mvc;

    [Authorize]
    public class InatorController : Controller {
        private readonly IInatorRepository inatorRepository;
        private readonly IUserCurrentService userCurrentService;

        public InatorController(IInatorRepository InatorRepository, IUserCurrentService UserCurrentService) {
            if (InatorRepository == null) {
                throw new ArgumentNullException(nameof(InatorRepository));
            }
            if (UserCurrentService == null) {
                throw new ArgumentNullException(nameof(UserCurrentService));
            }
            this.inatorRepository = InatorRepository;
            this.userCurrentService = UserCurrentService;
        }

        public IActionResult Index() {
            bool isAdmin = this.userCurrentService.IsAdmin;
            List<Inator> inators = null;
            if (isAdmin) {
                inators = this.inatorRepository.GetAll();
            } else {
                inators = this.inatorRepository.GetByUserId(this.userCurrentService.UserId ?? 0);
            }
            return this.View(inators);
        }

        [HttpGet]
        public IActionResult Edit(int id) {
            Inator inator = this.inatorRepository.GetById(id);
            if (inator == null) {
                if (id < 1) {
                    inator = new Inator {UserId = this.userCurrentService.UserId ?? 0};
                } else {
                    return View("NotFound");
                }
            }
            if (inator.UserId != this.userCurrentService.UserId && !this.userCurrentService.IsAdmin) {
                return this.View("NotFound"); // You don't own it therefore it doesn't exist for you
            }
            return this.View(inator);
        }

        [HttpPost]
        public IActionResult Edit(int id, Inator Model) {
            if (!this.ModelState.IsValid) {
                return this.View(Model); // fix your errors
            }
            Inator inator = this.inatorRepository.GetById(id);
            if (inator == null) {
                if (id < 1) {
                    inator = new Inator { UserId = this.userCurrentService.UserId ?? 0 };
                } else {
                    return View("NotFound");
                }
            }
            if (inator.UserId != this.userCurrentService.UserId && !this.userCurrentService.IsAdmin) {
                return this.View("NotFound"); // You don't own it therefore it doesn't exist for you
            }
            inator.Subdomain = Model.Subdomain;
            this.inatorRepository.Save(inator);
            return RedirectToAction("Index");
        }

    }
}
