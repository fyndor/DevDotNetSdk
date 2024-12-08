namespace DevDotNetSdk.Templating;

public class TemplateProvider
{
    private readonly List<TemplateSource> _sources;

    public TemplateProvider()
    {
        _sources = [];
    }

    public void AddSource(TemplateSource source)
    {
        _sources.Add(source);
    }

    public string? GetTemplate(string typeName)
    {
        for (var i = _sources.Count - 1; i >= 0; i--)
        {
            if (_sources[i].TryGetTemplateContent(typeName, out var template))
            {
                return template;
            }
        }
        return null;
    }

    public Type? GetTemplateType(string typeName)
    {
        for (var i = _sources.Count - 1; i >= 0; i--)
        {
            if (_sources[i].TryGetTemplateType(typeName, out var templateType))
            {
                return templateType;
            }
        }
        return null;
    }
}