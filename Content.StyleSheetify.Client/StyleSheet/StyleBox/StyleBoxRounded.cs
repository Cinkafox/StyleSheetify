using System.Numerics;
using Content.StyleSheetify.Shared.Math;
using Robust.Client.Graphics;
using Robust.Shared.Maths;

namespace Content.StyleSheetify.Client.StyleSheet.StyleBox;

public sealed class StyleBoxRound : Robust.Client.Graphics.StyleBox
{
    public Color BackgroundColor { get; set; } 
    public Color BorderColor { get; set; }
    public Thickness BorderThickness { get; set; }
    public CornerRadius CornerRadius { get; set; } = new(4f);

    public StyleBoxRound()
    {
    }

    public StyleBoxRound(StyleBoxRound other) : base(other)
    {
        BackgroundColor = other.BackgroundColor;
        BorderColor = other.BorderColor;
        BorderThickness = other.BorderThickness;
        CornerRadius = other.CornerRadius;
    }

    protected override void DoDraw(DrawingHandleScreen handle, UIBox2 box, float uiScale)
    {
        var radius = CornerRadius.Scale(uiScale);
        var thickness = BorderThickness.Scale(uiScale);
        
        if (BorderThickness.SumHorizontal > 0 ||  BorderThickness.SumVertical > 0)
        {
            DrawFilledRoundedRect(handle, box, BorderColor, radius);

            var innerBox = new UIBox2(
                box.Left + thickness.Left,
                box.Top + thickness.Top,
                box.Right - thickness.Right,
                box.Bottom - thickness.Bottom);

            var innerRadii = radius.Shrink(thickness);

            DrawFilledRoundedRect(handle, innerBox, BackgroundColor, innerRadii);
        }
        else
        {
            DrawFilledRoundedRect(handle, box, BackgroundColor, radius);
        }
    }
    
    private static void DrawFilledRoundedRect(
        DrawingHandleScreen handle,
        UIBox2 rect,
        Color color,
        CornerRadius radii,
        int segments = 8)
    {
        var verts = new Vector2[(segments * 4 + 4) * 3];

        var count = ShapeBuilder.BuildRoundedRectTriangles(
            verts,
            rect,
            radii,
            segments);

        handle.DrawPrimitives(
            DrawPrimitiveTopology.TriangleList,
            verts.AsSpan(0, count),
            color);
    }

    protected override float GetDefaultContentMargin(Margin margin)
    {
        return margin switch
        {
            Margin.Top => BorderThickness.Top,
            Margin.Bottom => BorderThickness.Bottom,
            Margin.Right => BorderThickness.Right,
            Margin.Left => BorderThickness.Left,
            _ => throw new Exception()
        };
    }
}