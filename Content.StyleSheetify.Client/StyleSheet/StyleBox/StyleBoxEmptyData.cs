using Robust.Client.Graphics;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.StyleSheetify.Client.StyleSheet.StyleBox;

[Serializable, DataDefinition]
public sealed partial class StyleBoxEmptyData : StyleBoxData
{
    public static implicit operator StyleBoxEmpty(StyleBoxEmptyData data)
    {
        var box = new StyleBoxEmpty();
        data.SetBaseParam(ref box);
        return box;
    }
}
