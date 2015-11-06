namespace ApiInator.Web.Models {
    using System;
    using System.Collections.Generic;
    using Microsoft.Data.Entity;
    using System.Linq;

    public interface IInatorRepository {
        int Save(Inator Inator);
        Inator GetById(int InatorId);
        List<Inator> GetAll();
        List<Inator> GetByUserId(int UserId);
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

        public Inator GetById(int InatorId) {
            if (InatorId < 1) {
                return null;
            }
            return (
                from i in db.Inators
                where i.InatorId == InatorId
                select i
            ).FirstOrDefault();
        }

        public List<Inator> GetAll() {
            return (
                from i in db.Inators
                orderby i.Subdomain
                select i
            ).ToList();
        }

        public List<Inator> GetByUserId(int UserId) {
            if (UserId < 1) {
                return new List<Inator>();
            }
            return (
                from i in db.Inators
                where i.UserId == UserId
                orderby i.Subdomain
                select i
            ).ToList();
        }

    }
}
