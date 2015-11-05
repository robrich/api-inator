namespace ApiInator.Web.Controllers {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using ApiInator.Web.Models;
    using Microsoft.AspNet.Hosting;
    using Microsoft.AspNet.Http;
    using Microsoft.AspNet.Mvc;
    using Microsoft.AspNet.Mvc.Infrastructure;
    using Microsoft.AspNet.Mvc.Routing;
    using Microsoft.AspNet.Routing;

    public class HandleApiController : Controller {
        private readonly IEndpointRepository endpointRepository;
        private readonly IHostingEnvironment env;

        private readonly Regex subdomainRegex = new Regex("\\.api-inator\\.com$");

        public HandleApiController(IEndpointRepository EndpointRepository, IHostingEnvironment Env) {
            if (EndpointRepository == null) {
                throw new ArgumentNullException(nameof(EndpointRepository));
            }
            this.endpointRepository = EndpointRepository;
            this.env = Env;
        }

        public string Index(string pathInfo/*FRAGILE: matches route setup */) {

            // TODO: how to pass the found Endpoint from the route constraint to the controller?

            string method = this.HttpContext.Request.Method;
            string host = this.HttpContext.Request.Host.ToString();
            string url = this.HttpContext.Request.Path;

            if (this.env.IsDevelopment()) {
                host = "one.api-inator.com"; // TODO: switch/case on port?
            }
            string subdomain = this.subdomainRegex.Replace(host, "");

            Endpoint endpoint = this.endpointRepository.GetMatch(subdomain, method, url);

            this.Response.ContentType = endpoint.ContentType;
            this.Response.StatusCode = endpoint.StatusCode;
            return endpoint.ResponseContent;
        }

    }

    public class InatorConstraint : IRouteConstraint {
        private readonly IEndpointRepository endpointRepository;
        private readonly IHostingEnvironment env;

        private readonly Regex subdomainRegex = new Regex("\\.api-inator\\.com$");

        public InatorConstraint(IEndpointRepository EndpointRepository, IHostingEnvironment Env) {
            if (EndpointRepository == null) {
                throw new ArgumentNullException(nameof(EndpointRepository));
            }
            if (Env == null) {
                throw new ArgumentNullException(nameof(Env));
            }
            this.endpointRepository = EndpointRepository;
            this.env = Env;
        }

        public bool Match(HttpContext httpContext, IRouter route, string routeKey, IDictionary<string,object> values, RouteDirection routeDirection) {
            string method = httpContext.Request.Method;
            string host = httpContext.Request.Host.ToString();
            string url = httpContext.Request.Path;

            if (this.env.IsDevelopment()) {
                host = "one.api-inator.com"; // TODO: switch/case on port?
            }
            string subdomain = this.subdomainRegex.Replace(host, "");

            if (string.IsNullOrEmpty(subdomain) || subdomain == "www") {
                return false; // PERF: regular site
            }

            Endpoint endpoint = this.endpointRepository.GetMatch(subdomain, method, url);

            if (endpoint != null) {
                values.Add("endpoint", endpoint);
                return true;
            }

            return false;
        }

    }
}
