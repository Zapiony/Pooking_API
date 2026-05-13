using System;
using System.Linq;
using Microsoft.OpenApi;

class Program {
    static void Main() {
        var asm = typeof(Microsoft.OpenApi.OpenApiSpecVersion).Assembly;
        foreach (var t in asm.GetTypes().Where(t => t.Name.Contains("OpenApiInfo") || t.Name.Contains("OpenApiSecurityScheme"))) {
            Console.WriteLine(t.FullName);
        }
    }
}
