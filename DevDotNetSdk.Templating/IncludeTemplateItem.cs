using System.Reflection;

namespace DevDotNetSdk.Templating;

internal class IncludeTemplateItem(
    object subTemplateInstance,
    MethodInfo renderMethod,
    string? inputExpression = null
) : TemplateItem
{
    public object SubTemplateInstance { get; } = subTemplateInstance;
    public MethodInfo RenderMethod { get; } = renderMethod;
    public string? InputExpression { get; } = inputExpression;
}