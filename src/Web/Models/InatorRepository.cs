namespace ApiInator.Web.Models {
    using System;
    using System.Collections.Generic;
    using Microsoft.Data.Entity;
    using System.Linq;

    public interface IInatorRepository {
        int Save(Inator Inator);
        List<Inator> GetAll();
        Inator GetById(int InatorId);
    }

    public class InatorRepository : IInatorRepository {
        private readonly ApiInatorDbContext db;

        public InatorRepository(ApiInatorDbContext ApiInatorDbContext) {
            if (ApiInatorDbContext == null) {
                throw new ArgumentNullException(nameof(ApiInatorDbContext));
            }
            this.db = ApiInatorDbContext;
        }

        public int Save(Inator Inator) {
                if (Inator.InatorId > 0) {
                    db.Inators.Attach(Inator);
                    db.Entry(Inator).State = EntityState.Modified;
                } else {
                    db.Inators.Add(Inator);
                }
                return db.SaveChanges();
        }

        public List<Inator> GetAll() {
            return (
                from i in db.Inators
                orderby i.Subdomain
                select i
            ).ToList();
        }

        public Inator GetById(int InatorId) {
                return (
                    from i in db.Inators
                    where i.InatorId == InatorId
                    select i
                ).FirstOrDefault();
        }

    }
}
