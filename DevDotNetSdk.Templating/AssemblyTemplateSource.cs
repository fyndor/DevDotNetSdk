using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace DevDotNetSdk.Templating;

public class AssemblyTemplateSource(Assembly assembly, bool cacheTemplate = true) : TemplateSource(cacheTemplate)
{
    private readonly Assembly _assembly = assembly;

    protected override bool DoTryGetTemplateContent(string name, [NotNullWhen(true)] out string? template) 
    {
        var templateFileName = $"{name}.md";
        foreach (var resourceName in _assembly.GetManifestResourceNames())
        {
            if (!resourceName.EndsWith($".{templateFileName}"))
            {
                continue;
            }
            using var stream = _assembly.GetManifestResourceStream(resourceName);
            using var reader = new StreamReader(stream!);
            template = reader.ReadToEnd();
            return true;
        }
        template = null;
        return false;
    }

    protected override bool DoTryGetTemplateType(string name, [NotNullWhen(true)] out Type? templateType)
    {
        var assemblyTypes = _assembly.GetTypes();
        templateType = assemblyTypes.FirstOrDefault(t => t.Name == name && TypeUtils.IsSubclassOfRawGeneric(typeof(TemplateBase<>), t))
                    ?? throw new InvalidOperationException($"Template '{name}' not found.");
        return templateType != null;
    } 
}