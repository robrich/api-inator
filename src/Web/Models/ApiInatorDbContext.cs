namespace ApiInator.Web.Models {
    using Microsoft.Data.Entity;

    public class ApiInatorDbContext : DbContext {
        public DbSet<User> Users { get; set; } 
        public DbSet<Inator> Inators { get; set; }
        public DbSet<Endpoint> Endpoints { get; set; }
    }
}
