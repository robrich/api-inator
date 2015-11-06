namespace ApiInator.Web.Models {
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class User {
        [Key]
        public int UserId { get; set; }

        public int GitHubId { get; set; }

        [Required]
        [StringLength(50)]
        public string Login { get; set; }

        [StringLength(400)]
        public string Name { get; set; }

        [StringLength(500)]
        public string AvitarUrl { get; set; }

        public bool IsAdmin { get; set; }

        public List<Inator> Inators { get; set; }
    }

    public class Inator {
        [Key]
        public int InatorId { get; set; }

        [Required]
        [StringLength(50)]
        public string Subdomain { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public List<Endpoint> Endpoints { get; set; }
    }

    public class Endpoint {
        public int EndpointId { get; set; }
        public int InatorId { get; set; }
        public Inator Inator { get; set; }

        [Required]
        [StringLength(10)]
        public string Method { get; set; }

        [Required]
        [StringLength(400)]
        public string Url { get; set; }

        public int StatusCode { get; set; }

        [Required]
        [StringLength(200)]
        public string ContentType { get; set; }

        [Column("ResponseTypeId")]
        public ResponseType ResponseType { get; set; }

        public string ResponseContent { get; set; }
    }

    public enum ResponseType {
        Static = 1,
        JavaScript = 2,
        CSharp = 3
    }
}
