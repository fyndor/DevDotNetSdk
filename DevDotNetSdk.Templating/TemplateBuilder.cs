using System.Reflection;

namespace DevDotNetSdk.Templating;

public class TemplateBuilder
{
    private readonly TemplateProvider _provider;
    private readonly ManualTemplateSource _manualSource;
    private Dictionary<Type, object> _templates;

    public TemplateBuilder()
    {
        _provider = new TemplateProvider();
        _manualSource = new ManualTemplateSource();
        _provider.AddSource(_manualSource);
        _templates = [];
    }

    public TemplateBuilder AddManualTemplate<TTemplate, TModel>(string content) where TTemplate : TemplateBase<TModel>
    {
        _manualSource.AddTemplate<TTemplate, TModel>(content);
        return this;
    }

    public TemplateBuilder AddAssemblyTemplateSource(Assembly assembly)
    {
        _provider.AddSource(new AssemblyTemplateSource(assembly));
        return this;
    }

    public string Render<TTemplate, TModel>(TModel model) where TTemplate : TemplateBase<TModel>
    {
        var type = typeof(TTemplate);
        if (!_templates.TryGetValue(type, out var templateObj))
        {
            templateObj = (TTemplate)Activator.CreateInstance(type, _provider)!;
            _templates[type] = templateObj;
        }       
        var template = (TTemplate)templateObj;
        return template.Render(model);
    }
}