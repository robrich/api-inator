namespace ApiInator.Web.Models {
    using System;
    using System.Collections.Generic;
    using Microsoft.Data.Entity;
    using System.Linq;

    public interface IEndpointRepository {
        int Save(Endpoint Endpoint);
        List<Endpoint> GetByInatorId(int InatorId);
        Endpoint GetById(int EndpointId);
        Endpoint GetByIdInclude(int EndpointId);
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
                db.Endpoints.Attach(Endpoint);
                db.Entry(Endpoint).State = EntityState.Modified;
            } else {
                db.Endpoints.Add(Endpoint);
            }
            return db.SaveChanges();
        }

        public List<Endpoint> GetByInatorId(int InatorId) {
            return (
                from i in db.Endpoints
                where i.InatorId == InatorId
                orderby i.Url
                select i
            ).ToList();
        }

        public Endpoint GetById(int EndpointId) {
            return (
                from i in db.Endpoints
                where i.EndpointId == EndpointId
                select i
            ).FirstOrDefault();
        }

        public Endpoint GetByIdInclude(int EndpointId) {
            return (
                from i in db.Endpoints.Include(e => e.Inator)
                where i.EndpointId == EndpointId
                select i
            ).FirstOrDefault();
        }

    }
}
