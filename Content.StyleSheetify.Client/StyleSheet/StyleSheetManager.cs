using Robust.Client.UserInterface;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Prototypes;
using Robust.Shared.Reflection;

namespace Content.StyleSheetify.Client.StyleSheet;

internal sealed class ContentStyleSheetManager : IContentStyleSheetManager
{
    [Dependency] private readonly IReflectionManager _reflectionManager = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly IUserInterfaceManager _userInterfaceManager = default!;

    public void ApplyStyleSheet(string prototype)
    {
        if(!_prototypeManager.TryIndex<StyleSheetPrototype>(prototype,out var proto))
            return;

        ApplyStyleSheet(proto);
    }

    public void ApplyStyleSheet(StyleSheetPrototype stylePrototype)
    {
        _userInterfaceManager.Stylesheet = new Stylesheet(GetStyleRules(stylePrototype));
    }

    public IEnumerable<StyleRule> GetStyleRules(ProtoId<StyleSheetPrototype> protoId)
    {
        if (!_prototypeManager.TryIndex(protoId, out var prototype))
            throw new Exception($"{protoId} not exist!");

        return GetStyleRules(prototype);
    }

    public List<StyleRule> GetStyleRules(StyleSheetPrototype stylePrototype)
    {
        var styleRule = new List<StyleRule>();

        foreach (var parent in stylePrototype.Parents)
        {
            styleRule.AddRange(GetStyleRules(parent));
        }

        foreach (var (elementPath, value) in stylePrototype.Styles)
        {
            var element = GetElement(elementPath, stylePrototype);
            foreach (var (key,dynamicValue) in value)
            {
                element.Prop(key, dynamicValue.GetValueObject());
            }

            styleRule.Add(element);
        }

        return styleRule;
    }

    public MutableSelector GetElement(string type,StyleSheetPrototype? prototype = null)
    {
        var childHandler = type.Split(' ').ToList();

        if (childHandler.Count > 1)
        {
            var child = new MutableSelectorChild();
            child.Parent(GetElement(childHandler[0]));
            childHandler.RemoveAt(0);
            child.Child(GetElement(string.Join(' ', childHandler)));
            return child;
        }

        var pseudoSeparator = type.Split(":");
        var classSeparator = pseudoSeparator[0].Split(".");
        var definedType = classSeparator[0];
        var element = new MutableSelectorElement();

        if (definedType != "*" && !string.IsNullOrEmpty(definedType))
        {
            if (prototype != null && prototype.TypeDefinition.TryGetValue(definedType, out var definition))
            {
                definedType = definition;
            }

            element.Type = _reflectionManager.GetType(definedType);
        }

        for (var i = 1; i < classSeparator.Length; i++)
        {
            element.Class(classSeparator[i]);
        }
        for (var i = 1; i < pseudoSeparator.Length; i++)
        {
            element.Pseudo(pseudoSeparator[i]);
        }

        return element;
    }

    public Stylesheet MergeStyles(Stylesheet stylesheet, string prefix)
    {
        if (!_prototypeManager.TryIndex<StyleSheetPrototype>(prefix, out var proto))
        {
            Logger.Warning($"Stylesheet merge failed! Style proto {prefix} not found!");
            return stylesheet;
        }

        var rules = stylesheet.Rules
            .GroupBy(r => r.Selector)
            .ToDictionary(g => g.Key, g => g.First());

        var newRules = GetStyleRules(proto)
            .GroupBy(r => r.Selector)
            .ToDictionary(g => g.Key, g => g.First());

        var mergedPropsCount = 0;
        var mergedStylesCount = 0;
        var addedStylesCount = 0;

        foreach (var (key, value) in newRules)
        {
            if (rules.TryGetValue(key, out var oriValue))
            {
                var oriProps = oriValue.Properties
                    .GroupBy(p => p.Name)
                    .ToDictionary(g => g.Key, g => g.First());

                foreach (var prop in value.Properties)
                {
                    oriProps[prop.Name] = prop;
                    mergedPropsCount++;
                }

                var mergedRule = new StyleRule(key, oriProps.Values.ToList());
                rules[key] = mergedRule;
                mergedStylesCount++;
            }
            else
            {
                rules[key] = value;
                addedStylesCount++;
            }
        }

        Logger.Info(
            $"Successfully merged style {prefix}: " +
            $"{mergedPropsCount} props merged, " +
            $"{mergedStylesCount} styles merged, " +
            $"{addedStylesCount} styles added."
        );

        return new Stylesheet(rules.Values.ToList());
    }
}
