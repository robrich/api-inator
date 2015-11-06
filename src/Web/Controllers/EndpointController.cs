namespace ApiInator.Web.Controllers {
    using System;
    using System.Collections.Generic;
    using ApiInator.Web.Models;
    using ApiInator.Web.Repositories;
    using ApiInator.Web.Services;
    using Microsoft.AspNet.Authorization;
    using Microsoft.AspNet.Mvc;

    [Authorize]
    public class EndpointController : Controller {
        private readonly IEndpointRepository endpointRepository;
        private readonly IInatorRepository inatorRepository;
        private readonly IUserCurrentService userCurrentService;

        public EndpointController(IEndpointRepository EndpointRepository, IInatorRepository InatorRepository, IUserCurrentService UserCurrentService) {
            if (EndpointRepository == null) {
                throw new ArgumentNullException(nameof(EndpointRepository));
            }
            if (InatorRepository == null) {
                throw new ArgumentNullException(nameof(InatorRepository));
            }
            if (UserCurrentService == null) {
                throw new ArgumentNullException(nameof(UserCurrentService));
            }
            this.endpointRepository = EndpointRepository;
            this.inatorRepository = InatorRepository;
            this.userCurrentService = UserCurrentService;
        }

        public IActionResult Index(int id) {
            Inator inator = this.inatorRepository.GetById(id);
            if (inator == null) {
                return View("NotFound");
            }
            if (inator.UserId != this.userCurrentService.UserId && !this.userCurrentService.IsAdmin) {
                return this.View("NotFound"); // You don't own it therefore it doesn't exist for you
            }
            inator.Endpoints = this.endpointRepository.GetByInatorId(id);
            return this.View(inator);
        }

        [HttpGet]
        public IActionResult Add(int id) {
            Inator inator = this.inatorRepository.GetById(id);
            if (inator == null) {
                return View("NotFound");
            }
            if (inator.UserId != this.userCurrentService.UserId && !this.userCurrentService.IsAdmin) {
                return this.View("NotFound"); // You don't own it therefore it doesn't exist for you
            }
            Endpoint endpoint = new Endpoint {
                InatorId = id,
                ContentType = "application/json",
                Method = "GET",
                ResponseType = ResponseType.Static,
                StatusCode = 200,
                Url = "/",
                Inator = inator
            };
            return this.View("Edit", endpoint);
        }

        [HttpGet]
        public IActionResult Edit(int id) {
            Endpoint endpoint = this.endpointRepository.GetByIdInclude(id);
            if (endpoint == null) {
                return this.View("NotFound");
            }
            if (endpoint.Inator.UserId != this.userCurrentService.UserId && !this.userCurrentService.IsAdmin) {
                return this.View("NotFound"); // You don't own it therefore it doesn't exist for you
            }
            return this.View(endpoint);
        }

        [HttpPost]
        public IActionResult Edit(int id, Endpoint Model) {
            if (!this.ModelState.IsValid) {
                return this.View(Model); // fix your errors
            }
            Endpoint endpoint = this.endpointRepository.GetByIdInclude(id);
            if (endpoint == null) {
                if (id > 0) {
                    return this.View("NotFound");
                }
                endpoint = new Endpoint {
                    InatorId = Model.InatorId,
                    Inator = this.inatorRepository.GetById(Model.InatorId)
                };
                if (endpoint.Inator == null) {
                    return View("NotFound");
                }
            }
            if (endpoint.Inator.UserId != this.userCurrentService.UserId && !this.userCurrentService.IsAdmin) {
                return this.View("NotFound"); // You don't own it therefore it doesn't exist for you
            }
            endpoint.Inator = null; // so EF doesn't attach and save it

            endpoint.ContentType = Model.ContentType;
            endpoint.Method = Model.Method;
            endpoint.ResponseContent = Model.ResponseContent;
            endpoint.ResponseType = Model.ResponseType;
            endpoint.StatusCode = Model.StatusCode;
            endpoint.Url = Model.Url;

            this.endpointRepository.Save(endpoint);
            return RedirectToAction("Index", new {id=endpoint.InatorId});
        }

    }
}
