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
}
