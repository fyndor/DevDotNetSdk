using System.Reflection;

namespace DevDotNetSdk.Templating;

internal class LoopTemplateItem(
    string collectionExpression, 
    object subTemplateInstance, 
    MethodInfo buildPromptMethod
) : TemplateItem
{
    public string CollectionExpression { get; } = collectionExpression;
    public object SubTemplateInstance { get; } = subTemplateInstance;
    public MethodInfo BuildPromptMethod { get; } = buildPromptMethod;
}
