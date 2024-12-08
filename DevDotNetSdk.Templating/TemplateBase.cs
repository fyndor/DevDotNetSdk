using System.Collections;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using DevDotNetSdk.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace DevDotNetSdk.Templating;

public abstract partial class TemplateBase<TInput>
{
    private static readonly Dictionary<string, string> TemplateCache = [];
    private readonly TemplateProvider _templateProvider;
    private readonly TemplateItem[] _templateItems;
    private static readonly IDeserializer YamlDeserializer = new DeserializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .Build();

    protected TemplateBase(TemplateProvider templateProvider)
    {
        _templateProvider = templateProvider;
        _templateItems = ParseTemplate(GetTemplateContent());
    }

    public string Render(TInput input)
    {
        var builder = new StringBuilder();
        foreach (var item in _templateItems)
        {
            switch (item)
            {
                case TextTemplateItem textItem:
                    builder.Append(textItem.Text);
                    break;

                case VariableTemplateItem variableItem:
                    var value = GetValueFromInput(variableItem.Expression, input);
                    builder.Append(value?.ToString() ?? $"{{{{{variableItem.Expression}}}}}");
                    break;

                case LoopTemplateItem loopItem:
                    ProcessLoopTemplateItem(loopItem, input, builder);
                    break;
            }
        }
        return builder.ToString();
    }

    private static void ProcessLoopTemplateItem(
        LoopTemplateItem loopItem,
        TInput input,
        StringBuilder builder)
    {
        var collection = GetValueFromInput(loopItem.CollectionExpression, input) as IEnumerable
            ?? throw new InvalidOperationException($"Variable '{loopItem.CollectionExpression}' is not a collection.");
        var subTemplateInstance = loopItem.SubTemplateInstance;
        var buildPromptMethod = loopItem.BuildPromptMethod;
        foreach (var item in collection)
        {
            var subContent = buildPromptMethod.Invoke(subTemplateInstance, [item]) as string
                ?? throw new InvalidOperationException($"Sub-template did not return a valid list of LLMMessage.");
            builder.Append(subContent);
        }
    }

    private TemplateItem[] ParseTemplate(string templateDocument)
    {
        var items = new List<TemplateItem>();
        var frontMatterMatch = FrontMatterRegex().Match(templateDocument);
        var templateContent = frontMatterMatch.Success 
            ? frontMatterMatch.Groups[2].Value 
            : templateDocument;
        var sections = SectionSplitRegex().Split(templateContent);
        foreach (var section in sections)
        {
            items.AddRange(ParseSectionIntoItems(section));
        }
        return [.. items];
    }

    private List<TemplateItem> ParseSectionIntoItems(string sectionContent)
    {
        var items = new List<TemplateItem>();
        var matches = VariableSubstitutionRegex().Matches(sectionContent);
        int lastIndex = 0;
        foreach (Match match in matches)
        {
            if (match.Index > lastIndex)
            {
                var text = sectionContent[lastIndex..match.Index];
                items.Add(new TextTemplateItem(text));
            }
            var expression = match.Groups[1].Value.Trim();
            if (expression.StartsWith("foreach:", StringComparison.OrdinalIgnoreCase))
            {
                var parts = expression.Split(':', 3);
                if (parts.Length != 3) 
                {
                    throw new InvalidOperationException($"Invalid foreach syntax: {expression}");
                }
                var variableName = parts[1].Trim();
                var subTemplateName = parts[2].Trim();

                var subTemplateType = _templateProvider.GetTemplateType(subTemplateName)
                    ?? throw new InvalidOperationException($"Sub-template '{subTemplateName}' not found.");

                var subTemplateInstance = Activator.CreateInstance(subTemplateType, _templateProvider)
                    ?? throw new InvalidOperationException($"Unable to instantiate sub-template '{subTemplateName}'.");

                var buildPromptMethod = subTemplateType.GetMethod(nameof(Render))
                    ?? throw new InvalidOperationException($"Sub-template '{subTemplateName}' does not have a BuildPrompt method.");

                items.Add(new LoopTemplateItem(variableName, subTemplateInstance, buildPromptMethod));
            }
            else
            {
                items.Add(new VariableTemplateItem(expression));
            }
            lastIndex = match.Index + match.Length;
        }
        if (lastIndex < sectionContent.Length)
        {
            var text = sectionContent[lastIndex..];
            items.Add(new TextTemplateItem(text));
        }
        return items;
    }


   
    private static object? GetValueFromInput(string expression, object? input)
    {
        var currentObject = input;
        foreach (var propertyName in expression.Split('.'))
        {
            if (currentObject == null)
                return null;

            var type = currentObject.GetType();
            PropertyInfo? property = null;
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
            {
                if (propertyName == "Key" || propertyName == "Value")
                {
                    property = type.GetProperty(propertyName);
                }
            }
            else
            {
                property = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            }

            if (property == null)
                throw new InvalidOperationException($"Property '{propertyName}' not found on type '{type.Name}'.");

            currentObject = property.GetValue(currentObject);
        }
        return currentObject;
    }

    private string GetTemplateContent()
    {
        var typeName = GetType().Name;
        var templateContent = _templateProvider.GetTemplate(typeName)
            ?? throw new FileNotFoundException($"Template '{typeName}' not found.");
        return templateContent;
    }

    [GeneratedRegex(@"^---[\r\n]+(.*?)[\r\n]+---[\r\n]+(.*)$", RegexOptions.Singleline)]
    private static partial Regex FrontMatterRegex();

    [GeneratedRegex(@"^\s*---\s*$", RegexOptions.Multiline)]
    private static partial Regex SectionSplitRegex();

    [GeneratedRegex(@"\{\{(.*?)\}\}", RegexOptions.Singleline)]
    private static partial Regex VariableSubstitutionRegex();
}
