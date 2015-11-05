namespace ApiInator.Web.Models {
    using System.Collections.Generic;

    public class RequestInfo {
        public string Method { get; set; }
        public string Path { get; set; }
        public Dictionary<string, string> Query { get; set; }
        public Dictionary<string, string> Headers { get; set; }
    }
}
