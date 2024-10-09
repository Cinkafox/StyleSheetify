using Robust.Client.UserInterface;
using Robust.Shared.Prototypes;

namespace StyleSheetify.Client.StyleSheet;

public interface IStyleSheetManager
{
    public void ApplyStyleSheet(StyleSheetPrototype stylePrototype);
    public void ApplyStyleSheet(string prototype);
    public IEnumerable<StyleRule> GetStyleRules(ProtoId<StyleSheetPrototype> protoId);
    public List<StyleRule> GetStyleRules(StyleSheetPrototype stylePrototype);
    public MutableSelectorElement GetElement(string type, StyleSheetPrototype? prototype = null);
}