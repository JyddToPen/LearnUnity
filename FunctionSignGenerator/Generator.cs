using FunctionSignGenerator.Processor;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FunctionSignGenerator
{
    /// <summary>
    /// 目标方法签名
    /// </summary>
    struct TargetMethod
    {
        /// <summary>
        /// 方法名
        /// </summary>
        public string MethodName { get; set; }
        /// <summary>
        /// 返回类型
        /// </summary>
        public string ReturnType { get; set; }
        /// <summary>
        /// 方法参数
        /// </summary>
        public string ParameterString { get; set; }
        /// <summary>
        /// 方法体
        /// </summary>
        public string Body { get; set; }
    }

    /// <summary>
    /// 产生结果
    /// </summary>
    public struct GeneratorResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    public class Generator
    {
        /// <summary>
        /// 产生空方法体
        /// </summary>
        /// <param name="analysisFolder"></param>
        /// <param name="exportFolder"></param>
        public static GeneratorResult GenerateFunctionSign(string analysisFolder, string exportFolder, string libName, LibLanguageType libLanguageType)
        {
            GeneratorResult generatorResult = new GeneratorResult();
            HashSet<TargetMethod> methods = new HashSet<TargetMethod>();
            LibConvertProcessor libConvertProcessor = null;
            switch (libLanguageType)
            {
                case LibLanguageType.JsLib:
                    libConvertProcessor = new JsLibProcessor();
                    break;
                default:
                    libConvertProcessor = new JsLibProcessor();
                    break;
            }
            if (libConvertProcessor == null)
            {
                generatorResult.Message = $"processor of {libLanguageType}  is null";
                return generatorResult;
            }
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
                        if (!methodDeclaration.AttributeLists.Any(item => item.ToString().Contains("DllImport")))
                        {
                            continue;
                        }
                        TargetMethod jsMethod = libConvertProcessor.GetTargetMethod(classDeclaration, methodDeclaration);
                        methods.Add(jsMethod);
                    }
                }
            }
            bool createFile = libConvertProcessor.GenerateFullFile(methods, exportFolder, libName);
            if (!createFile)
            {
                generatorResult.Message = $"There is not any extern function in folder:{analysisFolder}!";
            }
            else
            {
                generatorResult.Success = true;
            }
            return generatorResult;
        }
    }
}
