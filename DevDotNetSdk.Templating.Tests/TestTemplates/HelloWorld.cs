namespace DevDotNetSdk.Templating.Tests.TestTemplates;

public class HelloWorld(TemplateProvider templateProvider) : TemplateBase<HelloWorldModel>(templateProvider)
{
}

public class HelloWorldModel
{
    public string Message { get; set; } = string.Empty;
    public List<HelloWorldItem> Items { get; set; } = [];
}

public class HelloWorldItem
{
    public string Name { get; set; } = string.Empty;
}

public class HelloWorldSuperSubTemplate(TemplateProvider templateProvider) : TemplateBase<HelloWorldModel>(templateProvider)
{
}

public class HelloWorldWithSubTemplate(TemplateProvider templateProvider) : TemplateBase<HelloWorldModel>(templateProvider)
{
}

public class HelloWorldSubTemplate(TemplateProvider templateProvider) : TemplateBase<HelloWorldItem>(templateProvider)
{
}

public class HelloWorldSubNoInputTemplate(TemplateProvider templateProvider) : TemplateBase<object>(templateProvider)
{
}