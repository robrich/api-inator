namespace ApiInator.Web.Controllers {
    using System;
    using System.Collections.Generic;
    using ApiInator.Web.Models;
    using Microsoft.AspNet.Mvc;

    public class EndpointController : Controller {
        private readonly IEndpointRepository endpointRepository;
        private readonly IInatorRepository inatorRepository;

        public EndpointController(IEndpointRepository EndpointRepository, IInatorRepository InatorRepository) {
            if (EndpointRepository == null) {
                throw new ArgumentNullException(nameof(EndpointRepository));
            }
            if (InatorRepository == null) {
                throw new ArgumentNullException(nameof(InatorRepository));
            }
            this.endpointRepository = EndpointRepository;
            this.inatorRepository = InatorRepository;
        }

        public IActionResult Index(int id) {
            Inator inator = this.inatorRepository.GetById(id); // TODO: for this user
            if (inator == null) {
                return this.View("NotFound");
            }
            inator.Endpoints = this.endpointRepository.GetByInatorId(id);
            return this.View(inator);
        }

        [HttpGet]
        public IActionResult Add(int id) {
            Inator inator = this.inatorRepository.GetById(id);
            if (inator == null) {
                return this.View("NotFound");
            }
            Endpoint endpoint = new Endpoint {
                InatorId = id,
                ContentType = "application/json",
                Method = "GET",
                ResponseType = ResponseType.Json,
                StatusCode = 200,
                Url = "/",
                Inator = inator
            };
            return this.View("Edit", endpoint);
        }

        [HttpGet]
        public IActionResult Edit(int id) {
            Endpoint endpoint = this.endpointRepository.GetByIdInclude(id); // TODO: for this user
            if (endpoint == null) {
                return this.View("NotFound");
            }
            return this.View(endpoint);
        }

        [HttpPost]
        public IActionResult Edit(int id, Endpoint Endpoint) {
            if (!this.ModelState.IsValid) {
                return this.View(Endpoint); // fix your errors
            }
            Endpoint.EndpointId = id; // just in case
            this.endpointRepository.Save(Endpoint);
            return RedirectToAction("Index", new {id=Endpoint.InatorId});
        }

    }
}
