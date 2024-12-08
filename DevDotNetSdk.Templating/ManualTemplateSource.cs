using System.Diagnostics.CodeAnalysis;

namespace DevDotNetSdk.Templating;

public class ManualTemplateSource(bool cacheTemplate = false) : TemplateSource(cacheTemplate)
{
    private Dictionary<string, string> _contentCache = [];
    private Dictionary<string, Type> _typeCache = [];

    public ManualTemplateSource AddTemplate<TTemplate, TModel>(string content) where TTemplate : TemplateBase<TModel>
    {
        var templateType = typeof(TTemplate);
        var templateName = templateType.Name;
        if (_contentCache.ContainsKey(templateName))
        {
            throw new InvalidOperationException($"Template '{templateName}' already exists.");
        }
        _contentCache[templateName] = content;
        _typeCache[templateName] = templateType;
        return this;
    }

    protected override bool DoTryGetTemplateContent(string name, [NotNullWhen(true)] out string? template) 
    {
        if (_contentCache.TryGetValue(name, out var content))
        {
            template = content;
            return true;
        }
        template = null;
        return false;
    }

    protected override bool DoTryGetTemplateType(string name, [NotNullWhen(true)] out Type? templateType)
    {
        if (_typeCache.TryGetValue(name, out var type))
        {
            templateType = type;
            return true;
        }
        templateType = null;
        return false;
    } 
}