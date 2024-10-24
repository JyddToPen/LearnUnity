using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionSignGenerator.Processor
{
    internal abstract class LibConvertProcessor
    {
        private static readonly Dictionary<Type, string> TypeToShortName = new Dictionary<Type, string>
    {
        { typeof(int), "int" },
        { typeof(long), "long" },
        { typeof(short), "short" },
        { typeof(byte), "byte" },
        { typeof(sbyte), "sbyte" },
        { typeof(uint), "uint" },
        { typeof(ulong), "ulong" },
        { typeof(ushort), "ushort" },
        { typeof(float), "float" },
        { typeof(double), "double" },
        { typeof(decimal), "decimal" },
        { typeof(char), "char" },
        { typeof(string), "string" },
        { typeof(object), "object" },
        // Add other mappings as necessary
    };

        /// <summary>
        /// 获取目标方法体
        /// </summary>
        /// <param name="methodDeclaration"></param>
        /// <returns></returns>
        public virtual TargetMethod GetTargetMethod(ClassDeclarationSyntax classDeclarationSyntax, MethodDeclarationSyntax methodDeclaration)
        {

            if (!methodDeclaration.Modifiers.Any(item => item.ToString() == "extern"))
            {
                return default;
            }
            TargetMethod targetMethod = new TargetMethod();
            //sign
            string methodName = methodDeclaration.Identifier.Text;
            targetMethod.MethodName = methodName;
            //return value
            var returnType = methodDeclaration.ReturnType.ToString();
            targetMethod.ReturnType = GetEmptyFunctionReturnValue(classDeclarationSyntax, returnType);
            //param
            targetMethod.ParameterString = GetParameterString(methodDeclaration.ParameterList);
            //body
            targetMethod.Body = GetFullBody(targetMethod);
            return targetMethod;
        }


        /// <summary>
        /// 获取空方法的返回值
        /// </summary>
        /// <param name="nativeReturnValue"></param>
        /// <returns></returns>
        protected virtual string GetEmptyFunctionReturnValue(ClassDeclarationSyntax classDeclarationSyntax, string nativeReturnValue)
        {
            Type type = null;
            string libReturn = "";
            if (nativeReturnValue != "void")
            {
                type = Type.GetType(nativeReturnValue);
                if (type == null)
                {
                    NamespaceDeclarationSyntax namespaceName = classDeclarationSyntax.Ancestors()
            .OfType<NamespaceDeclarationSyntax>()
            .FirstOrDefault();
                    if (namespaceName != null && !string.IsNullOrEmpty(namespaceName.Name.ToString()))
                    {
                        //第一次尝试
                        try
                        {
                            type = Type.GetType($"{namespaceName}.{nativeReturnValue}");
                        }
                        catch (Exception e)
                        {

                        }
                    }
                }
                if (type == null)
                {
                    //第二次尝试
                    foreach (var item in classDeclarationSyntax.SyntaxTree
        .GetRoot()
        .DescendantNodes()
        .OfType<UsingDirectiveSyntax>())
                    {
                        try
                        {
                            type = Type.GetType($"{item.Name}.{nativeReturnValue}");
                        }
                        catch (Exception e)
                        {

                        }
                        if (type != null)
                        {
                            break;
                        }
                    }
                }
                if (type == null)
                {
                    //第三次尝试
                    foreach (var item in TypeToShortName)
                    {
                        if (item.Value == nativeReturnValue)
                        {
                            type = item.Key;
                            break;
                        }
                    }
                }
                if (type != null)
                {
                    libReturn = type.IsValueType ? Activator.CreateInstance(type).ToString() : null;
                }
            }
            return !string.IsNullOrEmpty(libReturn) ? "return " + libReturn + ";" : "";
        }

        /// <summary>
        /// 获取空方法的返回值
        /// </summary>
        /// <param name="nativeReturnValue"></param>
        /// <returns></returns>
        protected virtual string GetParameterString(ParameterListSyntax parameterListSyntax)
        {
            var parameterStr = parameterListSyntax.Parameters.Count > 0 ? string.Join(",", parameterListSyntax.Parameters.Select(item => item.Identifier.ToString())) : "";
            return parameterStr;
        }

        /// <summary>
        /// 获取完整的空方法体
        /// </summary>
        /// <param name="targetMethod"></param>
        /// <returns></returns>
        protected virtual string GetFullBody(TargetMethod targetMethod)
        {
            return @$"    {targetMethod.MethodName}: function({targetMethod.ParameterString}) {{ 
        {targetMethod.ReturnType}
    }}";
        }

        /// <summary>
        /// 产生完整文件
        /// </summary>
        /// <param name="methods"></param>
        /// <returns></returns>
        public virtual bool GenerateFullFile(HashSet<TargetMethod> methods, string exportFolder, string libName)
        {
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
                File.WriteAllText(exportFolder + "\\" + libName, targetContent);
                return true;
            }
            return false;
        }
    }
}
