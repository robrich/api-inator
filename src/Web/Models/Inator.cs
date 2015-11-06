namespace ApiInator.Web.Models {
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

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
}
