namespace ApiInator.Web.Models {
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

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
}
