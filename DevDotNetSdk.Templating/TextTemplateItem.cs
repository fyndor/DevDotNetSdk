namespace DevDotNetSdk.Templating;

internal class TextTemplateItem(string text) : TemplateItem
{
    public string Text { get; } = text;
}
