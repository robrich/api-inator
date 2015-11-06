namespace ApiInator.Web.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using Microsoft.AspNet.Http;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Emit;
    using Newtonsoft.Json;

    public interface ICsharpCompileHelper {
        string GetResult(HttpRequest Request, string ResponseContent);
    }

    // http://www.tugberkugurlu.com/archive/compiling-c-sharp-code-into-memory-and-executing-it-with-roslyn
    public class CsharpCompileHelper : ICsharpCompileHelper {


        public string GetResult(HttpRequest Request, string ResponseContent) {

            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(@"
using System;
using Microsoft.AspNet.Http;
using System.Collections.Generic;

public class TheGenerator {" +
ResponseContent+
"}");
            string assemblyName = Path.GetRandomFileName();
            MetadataReference[] references = new MetadataReference[] {
                MetadataReference.CreateFromFile(typeof (object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof (List<int>).Assembly.Location),
                MetadataReference.CreateFromFile(typeof (HttpRequest).Assembly.Location)
            };

            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: new[] {syntaxTree},
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
            );

            Assembly assembly = null;
            using (var ms = new MemoryStream()) {
                EmitResult result = compilation.Emit(ms);

                if (!result.Success) {
                    IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                        diagnostic.IsWarningAsError ||
                        diagnostic.Severity == DiagnosticSeverity.Error);

                    StringBuilder sb = new StringBuilder();
                    foreach (Diagnostic diagnostic in failures) {
                        sb.AppendLine(string.Format("{0}: {1}", diagnostic.Id, diagnostic.GetMessage()));
                    }
                    return sb.ToString();
                } else {
                    ms.Seek(0, SeekOrigin.Begin);
                    assembly = Assembly.Load(ms.ToArray());
                }
            }

            Type type = assembly.GetType("TheGenerator");
            object obj = Activator.CreateInstance(type);
            object outputObj = type.InvokeMember("GetResponse",
                BindingFlags.Default | BindingFlags.InvokeMethod,
                null,
                obj,
                new object[] {Request}
            );

            string output = "";
            if (outputObj != null) {
                output = JsonConvert.SerializeObject(outputObj);
            }

            return output;
        }


    }
}
