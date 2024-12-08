namespace DevDotNetSdk.Templating.Tests.TestTemplates;

internal class HelloWorld(TemplateProvider templateProvider) : TemplateBase<HelloWorldModel>(templateProvider)
{
}

internal class HelloWorldModel
{
    public string Message { get; set; } = string.Empty;
}