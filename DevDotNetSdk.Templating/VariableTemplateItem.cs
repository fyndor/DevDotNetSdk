namespace DevDotNetSdk.Templating;

internal class VariableTemplateItem(string expression) : TemplateItem
{
    public string Expression { get; } = expression;
}
