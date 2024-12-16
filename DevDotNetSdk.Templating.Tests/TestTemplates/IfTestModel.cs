namespace DevDotNetSdk.Templating.Tests.TestTemplates;

public class IfTemplate(TemplateProvider templateProvider) : TemplateBase<IfTestModel>(templateProvider)
{
}

public class IfSubTemplate(TemplateProvider templateProvider) : TemplateBase<IfSubModel>(templateProvider)
{
}

public class IfSubNoInputTemplate(TemplateProvider templateProvider) : TemplateBase<object>(templateProvider)
{
}

public class IfTestModel
{
    public bool Condition { get; set; }
    public IfSubModel SubModel { get; set; } = new IfSubModel();
}

public class IfSubModel
{
    public string Name { get; set; } = string.Empty;
}