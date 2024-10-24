
using FunctionSignGenerator;

string analysisFolder = "C:\\work\\cfmDev\\Assets\\Scripts\\GCloud";
string exportFolder = "C:\\work\\cfmDev\\Assets\\Plugins\\WebGL";
string libName = "gcloud_empty_function_sign.jslib";
GeneratorResult generatorResult = Generator.GenerateFunctionSign(analysisFolder, exportFolder, libName, LibLanguageType.JsLib);
Console.WriteLine($"result success:{generatorResult.Success} message:{generatorResult.Message}");

analysisFolder = "C:\\work\\cfmDev\\Assets\\Scripts\\GCloudVoice";
libName = "gcloudvoice_empty_function_sign.jslib";
generatorResult = Generator.GenerateFunctionSign(analysisFolder, exportFolder, libName, LibLanguageType.JsLib);
Console.WriteLine($"result success:{generatorResult.Success} message:{generatorResult.Message}");

//analysisFolder = "C:\\work\\cfmDev\\Assets\\Scripts\\WNGame\\CFStateMachine";
//libName = "cfstatesystem_empty_function_sign.jslib";
//generatorResult = Generator.GenerateFunctionSign(analysisFolder, exportFolder, libName, LibLanguageType.JsLib);
//Console.WriteLine($"result success:{generatorResult.Success} message:{generatorResult.Message}");

Console.ReadLine();
