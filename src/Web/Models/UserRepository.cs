namespace ApiInator.Web.Models {
    using System;
    using System.Collections.Generic;
    using Microsoft.Data.Entity;
    using System.Linq;

    public interface IUserRepository {
        int Save(User User);
        User GetById(int UserId);
        User GetByGitHubId(int UserId);
    }

    public class UserRepository : IUserRepository {
        private readonly ApiInatorDbContext db;

        public UserRepository(ApiInatorDbContext ApiUserDbContext) {
            if (ApiUserDbContext == null) {
                throw new ArgumentNullException(nameof(ApiUserDbContext));
            }
            this.db = ApiUserDbContext;
        }

        public int Save(User User) {
            if (User.UserId > 0) {
                db.Users.Attach(User);
                db.Entry(User).State = EntityState.Modified;
            } else {
                db.Users.Add(User);
            }
            return db.SaveChanges();
        }
        
        public User GetById(int UserId) {
            if (UserId < 1) {
                return null;
            }
            return (
                from i in db.Users
                where i.UserId == UserId
                select i
            ).FirstOrDefault();
        }

        public User GetByGitHubId(int GitHubId) {
            if (GitHubId < 1) {
                return null;
            }
            return (
                from i in db.Users
                where i.GitHubId == GitHubId
                select i
            ).FirstOrDefault();
        }
    }
}
