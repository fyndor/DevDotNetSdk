using System.Reflection;

namespace DevDotNetSdk.Templating;

internal class IfTemplateItem(
    string conditionExpression,
    object subTemplateInstance,
    MethodInfo renderMethod,
    string? inputExpression = null
) : TemplateItem
{
    public string ConditionExpression { get; } = conditionExpression;
    public object SubTemplateInstance { get; } = subTemplateInstance;
    public MethodInfo RenderMethod { get; } = renderMethod;
    public string? InputExpression { get; } = inputExpression;
}