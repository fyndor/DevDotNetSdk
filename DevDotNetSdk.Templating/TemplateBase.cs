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
    private TemplateItem[] _templateItems = [];
    private static readonly IDeserializer YamlDeserializer = new DeserializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .Build();

    protected TemplateBase(TemplateProvider templateProvider)
    {
        _templateProvider = templateProvider;
        
    }

    public string Render(TInput? input)
    {
        if (_templateItems.Length == 0)
        {
            _templateItems = ParseTemplate(GetTemplateContent());
        }
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
                    ProcessLoopTemplateItem(loopItem, input!, builder);
                    break;
                case IfTemplateItem ifItem:
                    ProcessIfTemplateItem(ifItem, input!, builder);
                    break;
                case IncludeTemplateItem includeItem:
                    ProcessIncludeTemplateItem(includeItem, input!, builder);
                    break;
            }
        }
        return builder.ToString();
    }

    private static void ProcessIncludeTemplateItem(
        IncludeTemplateItem includeItem,
        TInput input,
        StringBuilder builder)
    {
        var templateInput = includeItem.InputExpression != null
            ? GetValueFromInput(includeItem.InputExpression, input)
            : input;

        var subContent = includeItem.RenderMethod.Invoke(includeItem.SubTemplateInstance, [templateInput]) as string
            ?? throw new InvalidOperationException("Sub-template did not return a valid string.");
        builder.Append(subContent);
    }

    private static void ProcessIfTemplateItem(
        IfTemplateItem ifItem,
        TInput input,
        StringBuilder builder)
    {
        var conditionValue = GetValueFromInput(ifItem.ConditionExpression, input) as bool?;
        if (conditionValue == true)
        {
            var templateInput = ifItem.InputExpression != null
                ? GetValueFromInput(ifItem.InputExpression, input)
                : null;

            var subContent = ifItem.RenderMethod.Invoke(ifItem.SubTemplateInstance, [templateInput]) as string
                ?? throw new InvalidOperationException("Sub-template did not return a valid string.");
            builder.Append(subContent);
        }
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
            if (expression.StartsWith("include:", StringComparison.OrdinalIgnoreCase))
            {
                var parts = expression.Split(':', StringSplitOptions.TrimEntries);
                for (var i = 1; i < parts.Length; i++)
                {
                    if (!string.IsNullOrWhiteSpace(parts[i]))
                    {
                        continue;
                    }
                    throw new InvalidOperationException($"Invalid include syntax: {expression}");
                }
                if (parts.Length < 2 || parts.Length > 3)
                {
                    throw new InvalidOperationException($"Invalid include syntax: {expression}");
                }

                var subTemplateName = parts[1];
                var inputExpression = parts.Length == 3 ? parts[2] : null;

                if (inputExpression == "this")
                {
                    inputExpression = null;
                }
                var subTemplateType = _templateProvider.GetTemplateType(subTemplateName)
                    ?? throw new InvalidOperationException($"Sub-template '{subTemplateName}' not found.");

                var subTemplateInstance = Activator.CreateInstance(subTemplateType, _templateProvider)
                    ?? throw new InvalidOperationException($"Unable to instantiate sub-template '{subTemplateName}'.");

                var renderMethod = subTemplateType.GetMethod(nameof(Render))
                    ?? throw new InvalidOperationException($"Sub-template '{subTemplateName}' does not have a Render method.");

                items.Add(new IncludeTemplateItem(subTemplateInstance, renderMethod, inputExpression));
            } 
            else if (expression.StartsWith("if:", StringComparison.OrdinalIgnoreCase))
            {
                var parts = expression.Split(':', StringSplitOptions.TrimEntries);
                if (parts.Length < 3 || parts.Length > 4)
                {
                    throw new InvalidOperationException($"Invalid if syntax: {expression}");
                }

                var conditionExpression = parts[1];
                var subTemplateName = parts[2];
                var inputExpression = parts.Length == 4 ? parts[3] : null;

                var subTemplateType = _templateProvider.GetTemplateType(subTemplateName)
                    ?? throw new InvalidOperationException($"Sub-template '{subTemplateName}' not found.");

                var subTemplateInstance = Activator.CreateInstance(subTemplateType, _templateProvider)
                    ?? throw new InvalidOperationException($"Unable to instantiate sub-template '{subTemplateName}'.");

                var renderMethod = subTemplateType.GetMethod(nameof(Render))
                    ?? throw new InvalidOperationException($"Sub-template '{subTemplateName}' does not have a Render method.");

                items.Add(new IfTemplateItem(conditionExpression, subTemplateInstance, renderMethod, inputExpression));
            }
            else if (expression.StartsWith("foreach:", StringComparison.OrdinalIgnoreCase))
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
