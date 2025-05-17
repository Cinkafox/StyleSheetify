using Robust.Client.Graphics;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.StyleSheetify.Client.StyleSheet.StyleBox;

public class StyleBoxLayers: Robust.Client.Graphics.StyleBox
{
    public List<Robust.Client.Graphics.StyleBox> Layers { get; init; } = new();
    protected override void DoDraw(DrawingHandleScreen handle, UIBox2 box, float uiScale)
    {
        foreach (var layer in Layers)
        {
            layer.Draw(handle, box, uiScale);
        }
    }
}
