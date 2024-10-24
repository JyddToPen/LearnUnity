using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

struct JsMethod
{
    public string MethodName { get; set; }
    public string ReturnType { get; set; }
    public string ParameterString { get; set; }
    public string Body { get; set; }
}

class Program
{
    static void Main(string[] args)
    {
        if (args == null || args.Length < 2)
        {
            return;
        }
        string[] analysisFolders = {
            "C:\\work\\cfmMaster\\Assets\\Scripts\\GCloud",
            "C:\\work\\cfmMaster\\Assets\\Scripts\\GCloudVoice"
        };
        string exportFolder = "C:\\work\\cfmMaster\\Assets\\Plugins\\WebGL";
        foreach (var analysisFolder in analysisFolders)
        {
            GenerateEmptyFunction(analysisFolder, exportFolder);
        }
    }



    /// <summary>
    /// 产生空方法体
    /// </summary>
    /// <param name="analysisFolder"></param>
    /// <param name="exportFolder"></param>
    private static void GenerateEmptyFunction(string analysisFolder, string exportFolder)
    {
        HashSet<JsMethod> methods = new HashSet<JsMethod>();
        foreach (var item in Directory.GetFiles(analysisFolder, "*.cs", SearchOption.AllDirectories))
        {
            string code = File.ReadAllText(item);
            string fileName = Path.GetFileNameWithoutExtension(item);
            SyntaxTree tree = CSharpSyntaxTree.ParseText(code);
            var root = tree.GetRoot();
            var classDeclarations = root.DescendantNodes().OfType<ClassDeclarationSyntax>();
            foreach (var classDeclaration in classDeclarations)
            {
                var methodDeclarations = classDeclaration.Members.OfType<MethodDeclarationSyntax>();
                foreach (var methodDeclaration in methodDeclarations)
                {
                    if (!methodDeclaration.Modifiers.Any(item => item.ToString() == "extern"))
                    {
                        continue;
                    }
                    JsMethod jsMethod = new JsMethod();
                    //sign
                    string methodName = methodDeclaration.Identifier.Text;
                    jsMethod.MethodName = methodName;
                    //return value
                    var returnType = methodDeclaration.ReturnType.ToString();
                    string jsReturn = "";
                    if (returnType != "void")
                    {
                        Type type = Type.GetType(returnType);
                        if (type != null)
                        {
                            jsReturn = type.IsValueType ? Activator.CreateInstance(type).ToString() : null;
                        }
                    }
                    jsMethod.ReturnType = !string.IsNullOrEmpty(jsReturn) ? "return " + jsReturn + ";" : "";
                    //param
                    var parameter2js = methodDeclaration.ParameterList.Parameters.Count > 0 ? string.Join(",", methodDeclaration.ParameterList.Parameters.Select(item => item.Identifier.ToString())) : "";
                    var attributes = methodDeclaration.AttributeLists;
                    jsMethod.ParameterString = parameter2js;
                    jsMethod.Body = @$"    {jsMethod.MethodName}: function({jsMethod.ParameterString}) {{ 
        {jsMethod.ReturnType}
    }}";
                    methods.Add(jsMethod);
                    //Console.WriteLine($"File:{fileName} class:{classDeclaration.Identifier.Text} method:{methodName} returnType:{returnType} parameter2js:{parameter2js}");
                }
            }
        }
        if (methods.Count > 0)
        {
            string functionBody = string.Join(",\n\n", methods.Select(item => item.Body));
            string targetContent = @$"
mergeInto(LibraryManager.library, {{
{functionBody}
}});
";
            if (!Directory.Exists(exportFolder))
            {
                Directory.CreateDirectory(exportFolder);
            }
            string analysisName = Path.GetFileName(analysisFolder).ToLower();
            File.WriteAllText(exportFolder + "\\" + analysisName + ".jslib", targetContent);
        }
    }
}