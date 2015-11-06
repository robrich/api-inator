namespace ApiInator.Web.Controllers {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using ApiInator.Web.Models;
    using ApiInator.Web.Services;
    using Microsoft.AspNet.Hosting;
    using Microsoft.AspNet.Http;
    using Microsoft.AspNet.Mvc;
    using Microsoft.AspNet.Mvc.Infrastructure;
    using Microsoft.AspNet.Mvc.Routing;
    using Microsoft.AspNet.Routing;
    using System.Linq;
    using System.Threading.Tasks;
    using ApiInator.Web.Repositories;

    // FRAGILE: this class is too big and duplicates code a lot

    public class HandleApiController : Controller {
        private readonly IEndpointRepository endpointRepository;
        private readonly ICsharpCompileHelper csharpCompileHelper;
        private readonly IJavaScriptCompileHelper javaScriptCompileHelper;
        private readonly IHostingEnvironment env;

        private readonly Regex subdomainRegex = new Regex("\\.api-inator\\.com$");

        public HandleApiController(IEndpointRepository EndpointRepository, ICsharpCompileHelper CsharpCompileHelper, IJavaScriptCompileHelper JavaScriptCompileHelper, IHostingEnvironment Env) {
            if (EndpointRepository == null) {
                throw new ArgumentNullException(nameof(EndpointRepository));
            }
            if (CsharpCompileHelper == null) {
                throw new ArgumentNullException(nameof(CsharpCompileHelper));
            }
            if (JavaScriptCompileHelper == null) {
                throw new ArgumentNullException(nameof(JavaScriptCompileHelper));
            }
            if (Env == null) {
                throw new ArgumentNullException(nameof(Env));
            }
            this.endpointRepository = EndpointRepository;
            this.csharpCompileHelper = CsharpCompileHelper;
            this.javaScriptCompileHelper = JavaScriptCompileHelper;
            this.env = Env;
        }

        public async Task<string> Index(string pathInfo/*FRAGILE: matches route setup */) {

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
            
            string response = null;
            switch (endpoint.ResponseType) {
                case ResponseType.Static:
                    response = endpoint.ResponseContent;
                    break;
                case ResponseType.JavaScript:
                    RequestInfo requestInfo = new RequestInfo {
                        Method = method,
                        Path = url,
                        Headers = (
                            from q in this.Request.Headers.Keys
                            select new Tuple<string, string>(q, this.Request.Query[q])
                        ).ToDictionary(t => t.Item1, t => t.Item2),
                        Query = (
                            from q in this.Request.Query.Keys
                            select new Tuple<string, string>(q, this.Request.Query[q])
                        ).ToDictionary(t => t.Item1, t => t.Item2)
                    };
                    response = await this.javaScriptCompileHelper.GetResult(requestInfo, endpoint.EndpointId, endpoint.ResponseContent);
                    break;
                case ResponseType.CSharp:
                    response = this.csharpCompileHelper.GetResult(Request, endpoint.ResponseContent);
                    break;
            }
            
            return response;
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

        public bool Match(HttpContext httpContext, IRouter route, string routeKey, IDictionary<string, object> values, RouteDirection routeDirection) {
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

            Endpoint endpoint = null;
            try {
                endpoint = this.endpointRepository.GetMatch(subdomain, method, url);
            } catch (Exception) {
                // FRAGILE: swallow
                endpoint = null;
            }

            if (endpoint != null) {
                values.Add("endpoint", endpoint);
                return true;
            }

            return false;
        }
    }
}
