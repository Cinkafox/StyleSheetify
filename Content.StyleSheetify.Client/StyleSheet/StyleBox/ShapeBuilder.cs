using System.Numerics;
using Content.StyleSheetify.Shared.Math;
using Robust.Shared.Maths;

namespace Content.StyleSheetify.Client.StyleSheet.StyleBox;

internal static class ShapeBuilder
{
    public static int BuildRoundedRectTriangles(
        Span<Vector2> output,
        UIBox2 rect,
        CornerRadius radii,
        int segments = 8)
    {
        var contour = new Vector2[segments * 4 + 4];

        var contourCount = BuildRoundedRect(
            contour,
            rect,
            radii,
            segments);

        var center = rect.Center;

        var index = 0;

        for (var i = 0; i < contourCount; i++)
        {
            var a = contour[i];
            var b = contour[(i + 1) % contourCount];

            output[index++] = center;
            output[index++] = a;
            output[index++] = b;
        }

        return index;
    }
    
    public static int BuildRoundedRect(
        Span<Vector2> output,
        UIBox2 rect,
        CornerRadius radii,
        int segments = 8)
    {
        var r = Normalize(rect, radii);

        var count = 0;

        count += BuildArc(
            output[count..],
            new Vector2(rect.Left + r.TopLeft, rect.Top + r.TopLeft),
            r.TopLeft,
            MathF.PI,
            MathF.PI * 1.5f,
            segments,
            includeFirst: true);

        count += BuildArc(
            output[count..],
            new Vector2(rect.Right - r.TopRight, rect.Top + r.TopRight),
            r.TopRight,
            MathF.PI * 1.5f,
            MathF.PI * 2f,
            segments,
            includeFirst: false);

        count += BuildArc(
            output[count..],
            new Vector2(rect.Right - r.BottomRight, rect.Bottom - r.BottomRight),
            r.BottomRight,
            0,
            MathF.PI * 0.5f,
            segments,
            includeFirst: false);

        count += BuildArc(
            output[count..],
            new Vector2(rect.Left + r.BottomLeft, rect.Bottom - r.BottomLeft),
            r.BottomLeft,
            MathF.PI * 0.5f,
            MathF.PI,
            segments,
            includeFirst: false);

        return count;
    }

    private static int BuildArc(
        Span<Vector2> output,
        Vector2 center,
        float radius,
        float startAngle,
        float endAngle,
        int segments,
        bool includeFirst)
    {
        if (radius <= 0f)
        {
            output[0] = center;
            return 1;
        }

        var step = (endAngle - startAngle) / segments;

        var start = includeFirst ? 0 : 1;
        var written = 0;

        for (var i = start; i <= segments; i++)
        {
            var angle = startAngle + step * i;

            output[written++] = center +
                                new Vector2(
                                    MathF.Cos(angle),
                                    MathF.Sin(angle)) * radius;
        }

        return written;
    }

    private static CornerRadius Normalize(UIBox2 rect, CornerRadius r)
    {
        var tl = MathF.Max(0, r.TopLeft);
        var tr = MathF.Max(0, r.TopRight);
        var br = MathF.Max(0, r.BottomRight);
        var bl = MathF.Max(0, r.BottomLeft);

        var scale = 1f;

        if (tl + tr > rect.Width)
            scale = MathF.Min(scale, rect.Width / (tl + tr));

        if (bl + br > rect.Width)
            scale = MathF.Min(scale, rect.Width / (bl + br));

        if (tl + bl > rect.Height)
            scale = MathF.Min(scale, rect.Height / (tl + bl));

        if (tr + br > rect.Height)
            scale = MathF.Min(scale, rect.Height / (tr + br));

        if (scale < 1f)
        {
            tl *= scale;
            tr *= scale;
            br *= scale;
            bl *= scale;
        }

        return new CornerRadius(tl, tr, br, bl);
    }
}