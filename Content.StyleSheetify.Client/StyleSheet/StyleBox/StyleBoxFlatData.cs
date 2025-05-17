using Robust.Client.Graphics;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.StyleSheetify.Client.StyleSheet.StyleBox;

[Serializable, DataDefinition, SerializedType(nameof(StyleBoxFlatData))]
public sealed partial class StyleBoxFlatData : StyleBoxData
{
    [DataField] public Color BackgroundColor;
    [DataField] public Color BorderColor;

    /// <summary>
    /// Thickness of the border, in virtual pixels.
    /// </summary>
    [DataField] public Thickness BorderThickness;

    public static implicit operator StyleBoxFlat(StyleBoxFlatData data)
    {
        var styleBox = new StyleBoxFlat();
        data.SetBaseParam(ref styleBox);
        styleBox.BackgroundColor = data.BackgroundColor;
        styleBox.BorderColor = data.BorderColor;
        styleBox.BorderThickness = data.BorderThickness;
        return styleBox;
    }

    public static StyleBoxFlatData From(StyleBoxFlat value)
    {
        var styleBox = new StyleBoxFlatData();
        styleBox.GetBaseParam(value);
        styleBox.BackgroundColor = value.BackgroundColor;
        styleBox.BorderColor = value.BorderColor;
        styleBox.BorderThickness = value.BorderThickness;
        return styleBox;
    }
}
