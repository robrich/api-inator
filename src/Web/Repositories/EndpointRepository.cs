namespace ApiInator.Web.Repositories {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ApiInator.Web.Models;
    using Microsoft.Data.Entity;

    public interface IEndpointRepository {
        int Save(Endpoint Endpoint);
        List<Endpoint> GetByInatorId(int InatorId);
        Endpoint GetById(int EndpointId);
        Endpoint GetByIdInclude(int EndpointId);
        Endpoint GetMatch(string Subdomain, string Method, string Url);
    }

    public class EndpointRepository : IEndpointRepository {
        private readonly ApiInatorDbContext db;

        public EndpointRepository(ApiInatorDbContext ApiEndpointDbContext) {
            if (ApiEndpointDbContext == null) {
                throw new ArgumentNullException(nameof(ApiEndpointDbContext));
            }
            this.db = ApiEndpointDbContext;
        }

        public int Save(Endpoint Endpoint) {
            if (Endpoint.EndpointId > 0) {
                this.db.Endpoints.Attach(Endpoint);
                this.db.Entry(Endpoint).State = EntityState.Modified;
            } else {
                this.db.Endpoints.Add(Endpoint);
            }
            return this.db.SaveChanges();
        }

        public List<Endpoint> GetByInatorId(int InatorId) {
            if (InatorId < 1) {
                return null;
            }
            return (
                from i in this.db.Endpoints
                where i.InatorId == InatorId
                orderby i.Url
                select i
            ).ToList();
        }

        public Endpoint GetById(int EndpointId) {
            if (EndpointId < 1) {
                return null;
            }
            return (
                from i in this.db.Endpoints
                where i.EndpointId == EndpointId
                select i
            ).FirstOrDefault();
        }

        public Endpoint GetByIdInclude(int EndpointId) {
            if (EndpointId < 1) {
                return null;
            }
            return (
                from i in this.db.Endpoints.Include(e => e.Inator)
                where i.EndpointId == EndpointId
                select i
            ).FirstOrDefault();
        }

        // TODO: cache this heavily
        public Endpoint GetMatch(string Subdomain, string Method, string Url) {
            if (string.IsNullOrEmpty(Subdomain) || string.IsNullOrEmpty(Method) || string.IsNullOrEmpty(Url)) {
                return null;
            }
            return (
                from i in this.db.Endpoints.Include(e => e.Inator)
                where i.Inator.Subdomain == Subdomain
                && i.Method == Method
                && i.Url == Url
                select i
            ).FirstOrDefault();
        }

    }
}
