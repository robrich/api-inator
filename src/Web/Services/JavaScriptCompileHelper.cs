namespace ApiInator.Web.Services
{
    using System;
    using ApiInator.Web.Models;

    public interface IJavaScriptCompileHelper {
        string GetResult(RequestInfo RequestInfo, string ResponseContent);
    }

    public class JavaScriptCompileHelper : IJavaScriptCompileHelper {
        public string GetResult(RequestInfo RequestInfo, string ResponseContent) {
            throw new NotImplementedException();
        }

    }
}
