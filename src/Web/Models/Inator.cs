using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiInator.Web.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Inator
    {
        [Key]
        public int InatorId { get; set; }
        [Required]
        [StringLength(50)]
        public string Subdomain { get; set; }
        public int? UserId { get; set; }

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
        [Required]
        [StringLength(200)]
        public string ContentType { get; set; }
        [Column("ResponseTypeId")]
        public ResponseType ResponseType { get; set; }
        public string ResponseContent { get; set; }
    }

    public enum ResponseType {
        Json = 1,
        JavaScript = 2,
        CSharp = 3
    }
}
