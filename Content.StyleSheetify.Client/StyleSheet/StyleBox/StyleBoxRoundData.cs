using Content.StyleSheetify.Shared.Math;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.StyleSheetify.Client.StyleSheet.StyleBox;

[Serializable, DataDefinition, SerializedType(nameof(StyleBoxRoundData))]
public sealed partial class StyleBoxRoundData : StyleBoxData
{
    [DataField] public Color BackgroundColor { get; set; } 
    [DataField] public Color BorderColor { get; set; }
    [DataField] public Thickness BorderThickness { get; set; }
    [DataField] public CornerRadius CornerRadius { get; set; } = new(4f);
    
    public static implicit operator StyleBoxRound(StyleBoxRoundData data)
    {
        var styleBox = new StyleBoxRound();
        data.SetBaseParam(ref styleBox);
        styleBox.BackgroundColor = data.BackgroundColor;
        styleBox.BorderColor = data.BorderColor;
        styleBox.BorderThickness = data.BorderThickness;
        styleBox.CornerRadius = data.CornerRadius;
        return styleBox;
    }
    
    public static StyleBoxRoundData From(StyleBoxRound value)
    {
        var styleBox = new StyleBoxRoundData();
        styleBox.GetBaseParam(value);
        styleBox.BackgroundColor = value.BackgroundColor;
        styleBox.BorderColor = value.BorderColor;
        styleBox.BorderThickness = value.BorderThickness;
        styleBox.CornerRadius = value.CornerRadius;
        return styleBox;
    }
}