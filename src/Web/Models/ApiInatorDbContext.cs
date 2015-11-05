namespace ApiInator.Web.Models {
    using Microsoft.Data.Entity;

    public class ApiInatorDbContext : DbContext {
        public DbSet<Inator> Inators { get; set; }
        public DbSet<Endpoint> Endpoints { get; set; }
    }
}
