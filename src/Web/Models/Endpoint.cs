namespace ApiInator.Web.Models {
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

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
}
