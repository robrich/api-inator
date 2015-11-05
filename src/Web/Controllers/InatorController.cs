namespace ApiInator.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using ApiInator.Web.Models;
    using Microsoft.AspNet.Mvc;

    public class InatorController : Controller
    {
        private readonly IInatorRepository inatorRepository;

        public InatorController(IInatorRepository InatorRepository) {
            if (InatorRepository == null) {
                throw new ArgumentNullException(nameof(InatorRepository));
            }
            this.inatorRepository = InatorRepository;
        }

        public IActionResult Index() {
            List<Inator> inators = this.inatorRepository.GetAll(); // TODO: for this user
            return this.View(inators);
        }

        public IActionResult Edit(int id) {
            Inator inator = this.inatorRepository.GetById(id) ?? new Inator(); // TODO: for this user
            return this.View(inator);
        }

        public IActionResult Edit(int id, Inator Inator) {
            if (!this.ModelState.IsValid) {
                return this.View(Inator); // fix your errors
            }
            this.inatorRepository.Save(Inator);
            return RedirectToAction("Index");
        }

    }
}
