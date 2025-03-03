using Robust.Client.UserInterface;
using Robust.Shared.Prototypes;
using Content.StyleSheetify.Client.StyleSheet;

namespace Content.StyleSheetify.Client.StyleSheet;

public interface IContentStyleSheetManager
{
    public void ApplyStyleSheet(StyleSheetPrototype stylePrototype);
    public void ApplyStyleSheet(string prototype);
    public IEnumerable<StyleRule> GetStyleRules(ProtoId<StyleSheetPrototype> protoId);
    public List<StyleRule> GetStyleRules(StyleSheetPrototype stylePrototype);
    public MutableSelector GetElement(string type, StyleSheetPrototype? prototype = null);
}
