namespace ApiInator.Web.Services {
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using ApiInator.Web.Models;
    using Microsoft.AspNet.Http;
    using Microsoft.AspNet.NodeServices;
    using Newtonsoft.Json;

    public interface IJavaScriptCompileHelper {
        Task<string> GetResult(RequestInfo RequestInfo, int EndpointId, string ResponseContent);
    }

    public class JavaScriptCompileHelper : IJavaScriptCompileHelper {
        private readonly INodeServices nodeServices;

        public JavaScriptCompileHelper(INodeServices NodeServices) {
            if (NodeServices == null) {
                throw new ArgumentNullException(nameof(NodeServices));
            }
            this.nodeServices = NodeServices;
        }

        public async Task<string> GetResult(RequestInfo RequestInfo, int EndpointId, string ResponseContent) {

            //string fileContent = "module.exports = function (cb, req) { cb(null, JSON.stringify({some:'obj'}));};";
            string fileContent = "module.exports = " + ResponseContent + ";";
            using (StringAsTempFile temp = new Microsoft.AspNet.NodeServices.StringAsTempFile(fileContent)) {

                try {
                    return await this.nodeServices.Invoke<string>(temp.FileName, RequestInfo);
                } catch (Exception ex) {
                    return ex.ToString();
                }
            }
        }

    }
}
