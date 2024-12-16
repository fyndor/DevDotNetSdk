using System.Reflection;
using DevDotNetSdk.Templating.Tests.TestTemplates;

namespace DevDotNetSdk.Templating.Tests;

public class BasicTemplateTests
{
    [Fact]
    public void HelloWorld_GetsExpectedResult()
    {
        var message = "Hello, World!";
        var model = new HelloWorldModel { Message = message };

        var templateProvider = new TemplateProvider();
        var templateSource = new AssemblyTemplateSource(Assembly.GetExecutingAssembly());
        templateProvider.AddSource(templateSource);
        var template = new HelloWorld(templateProvider);

        var result = template.Render(model);

        Assert.Equal(message, result);
    }

    [Fact]
    public void HelloWorld_BuilderAssemblyTest_GetsExpectedResult()
    {
        var message = "Hello, World!";
        var model = new HelloWorldModel { Message = message };

        var templateBuilder = new TemplateBuilder()
            .AddAssemblyTemplateSource(Assembly.GetExecutingAssembly());

        var result = templateBuilder.Render<HelloWorld, HelloWorldModel>(model);

        Assert.Equal(message, result);
    }

    [Fact]
    public void HelloWorld_BuilderManualTest_GetsExpectedResult()
    {
        var message = "Hello, World!";
        var templateContent = "{{Message}}";
        var model = new HelloWorldModel { Message = message };

        var templateBuilder = new TemplateBuilder()
            .AddManualTemplate<HelloWorld, HelloWorldModel>(templateContent);

        var result = templateBuilder.Render<HelloWorld, HelloWorldModel>(model);

        Assert.Equal(message, result);
    }

    [Fact]
    public void HelloWorld_BuilderWithSubTemplates_GetsExpectedResult()
    {
        var name = "Hello, World!";
        var model = new HelloWorldModel
        { 
            Message = "doesnt matter, not part of template", 
            Items =
            [
                new() { Name = name } 
            ] 
        };

        var templateBuilder = new TemplateBuilder()
            .AddAssemblyTemplateSource(Assembly.GetExecutingAssembly());
        var result = templateBuilder.Render<HelloWorldWithSubTemplate, HelloWorldModel>(model);

        Assert.Equal(name, result);
    }

    [Fact]
    public void IfStatement_RendersCorrectly()
    {
        const string NAME = "Hello, World!";
        var model = new IfTestModel
        {
            Condition = true,
            SubModel = new IfSubModel { Name = NAME }
        };

        var templateContent = "{{if:Condition:IfSubTemplate:SubModel}}";
        var templateBuilder = new TemplateBuilder()
            .AddManualTemplate<IfTemplate, IfTestModel>(templateContent)
            .AddManualTemplate<IfSubTemplate, IfSubModel>("{{Name}}");

        var result = templateBuilder.Render<IfTemplate, IfTestModel>(model);

        Assert.Equal(NAME, result);
    }

    [Fact]
    public void IfStatement_WithNoSubTemplateInput_RendersCorrectly()
    {
        const string NAME = "Hello, World!";
        var model = new IfTestModel
        {
            Condition = true
        };

        var templateContent = "{{if:Condition:IfSubNoInputTemplate}}";
        var templateBuilder = new TemplateBuilder()
            .AddManualTemplate<IfTemplate, IfTestModel>(templateContent)
            .AddManualTemplate<IfSubNoInputTemplate, object>(NAME);

        var result = templateBuilder.Render<IfTemplate, IfTestModel>(model);

        Assert.Equal(NAME, result);
    }

    [Fact]
    public void IncludeStatement_RendersSubTemplate()
    {
        var model = new IncludeTestModel
        {
            SubModel = new IncludeSubModel { Content = "Wont be shown" }
        };
        var templateContent = "{{include:IncludeSubNoInputTemplate}}";
        var subTemplateContent = "Static SubTemplate Content";

        var templateBuilder = new TemplateBuilder()
            .AddManualTemplate<IncludeTemplate, IncludeTestModel>(templateContent)
            .AddManualTemplate<IncludeSubNoInputTemplate, object>(subTemplateContent);

        var result = templateBuilder.Render<IncludeTemplate, IncludeTestModel>(model);

        Assert.Equal(subTemplateContent, result);
    }

    [Fact]
    public void IncludeStatement_WithInput_RendersSubTemplate()
    {
        var content = "Hello World";
        var model = new IncludeTestModel
        {
            SubModel = new IncludeSubModel { Content = content }
        };
        var templateContent = "{{include:IncludeSubTemplate:SubModel}}";
        var subTemplateContent = "{{Content}}";

        var templateBuilder = new TemplateBuilder()
            .AddManualTemplate<IncludeTemplate, IncludeTestModel>(templateContent)
            .AddManualTemplate<IncludeSubTemplate, IncludeSubModel>(subTemplateContent);

        var result = templateBuilder.Render<IncludeTemplate, IncludeTestModel>(model);

        Assert.Equal(content, result);
    }
    
    [Fact]
    public void IncludeStatement_MissingSubTemplateName_Throws()
    {
        var templateContent = "{{include:}}";

        var templateBuilder = new TemplateBuilder()
            .AddManualTemplate<HelloWorld, HelloWorldModel>(templateContent);

        Assert.Throws<InvalidOperationException>(() =>
            templateBuilder.Render<HelloWorld, HelloWorldModel>(new HelloWorldModel()));
    }

    [Fact]
    public void IncludeStatement_InvalidInputExpression_Throws()
    {
        var templateContent = "{{include:HelloWorldSubNoInputTemplate:InvalidPropertyName}}";

        var model = new HelloWorldModel { Message = "Hello!" };

        var templateBuilder = new TemplateBuilder()
            .AddManualTemplate<HelloWorld, HelloWorldModel>(templateContent)
            .AddManualTemplate<HelloWorldSubNoInputTemplate, object>("");

        Assert.Throws<InvalidOperationException>(() =>
            templateBuilder.Render<HelloWorld, HelloWorldModel>(model));
    }

    [Fact]
    public void IncludeStatement_WithThis_RendersSubTemplate()
    {
        var message = "Hello, World!";
        var model = new HelloWorldModel { Message = message };

        var templateContent = "{{include:HelloWorldSuperSubTemplate:this}}";
        var subTemplateContent = "{{Message}}";

        var templateBuilder = new TemplateBuilder()
            .AddManualTemplate<HelloWorld, HelloWorldModel>(templateContent)
            .AddManualTemplate<HelloWorldSuperSubTemplate, HelloWorldModel>(subTemplateContent);

        var result = templateBuilder.Render<HelloWorld, HelloWorldModel>(model);

        Assert.Equal(message, result);
    }
}
