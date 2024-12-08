using System.Diagnostics.CodeAnalysis;

namespace DevDotNetSdk.Templating;

public abstract class TemplateSource(bool cacheTemplate)
{
    private readonly bool _cacheTemplate = cacheTemplate;
    private readonly Dictionary<string, string> _templateCache = [];

    public bool TryGetTemplateContent(string name, [NotNullWhen(true)] out string? templateContent)
    {
        if (_cacheTemplate && _templateCache.TryGetValue(name, out templateContent))
        {
            return true;
        }
        if (!DoTryGetTemplateContent(name, out templateContent))
        {
            return false;
        }
        if (_cacheTemplate)
        {
            _templateCache[name] = templateContent;
        }
        return true;
    }

    public bool TryGetTemplateType(string name, [NotNullWhen(true)] out Type? templateType)
    {
        if (!DoTryGetTemplateType(name, out templateType))
        {
            return false;
        }
        return true;
    }

    protected abstract bool DoTryGetTemplateType(string name, [NotNullWhen(true)] out Type? templateType);
    
    protected abstract bool DoTryGetTemplateContent(string name, [NotNullWhen(true)] out string? template);
}