namespace DevDotNetSdk.Templating.Tests.TestTemplates;

public class IncludeTemplate(TemplateProvider templateProvider) : TemplateBase<IncludeTestModel>(templateProvider)
{
}

public class IncludeSubTemplate(TemplateProvider templateProvider) : TemplateBase<IncludeSubModel>(templateProvider)
{
}

public class IncludeSubNoInputTemplate(TemplateProvider templateProvider) : TemplateBase<object>(templateProvider)
{
}

public class IncludeTestModel
{
    public IncludeSubModel SubModel { get; set; } = new IncludeSubModel();
}

public class IncludeSubModel
{
    public string Content { get; set; } = string.Empty;
}