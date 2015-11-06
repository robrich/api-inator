namespace ApiInator.Web.Repositories {
    using System;
    using System.Linq;
    using ApiInator.Web.Models;
    using Microsoft.Data.Entity;

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
                this.db.Users.Attach(User);
                this.db.Entry(User).State = EntityState.Modified;
            } else {
                this.db.Users.Add(User);
            }
            return this.db.SaveChanges();
        }
        
        public User GetById(int UserId) {
            if (UserId < 1) {
                return null;
            }
            return (
                from i in this.db.Users
                where i.UserId == UserId
                select i
            ).FirstOrDefault();
        }

        public User GetByGitHubId(int GitHubId) {
            if (GitHubId < 1) {
                return null;
            }
            return (
                from i in this.db.Users
                where i.GitHubId == GitHubId
                select i
            ).FirstOrDefault();
        }
    }
}
