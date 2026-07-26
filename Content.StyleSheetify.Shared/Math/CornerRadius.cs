using JetBrains.Annotations;
using Robust.Shared.Maths;
using Robust.Shared.Utility;

namespace Content.StyleSheetify.Shared.Math;

[PublicAPI]
public struct CornerRadius : ISpanFormattable
{
    public float TopLeft;
    public float TopRight;
    public float BottomRight;
    public float BottomLeft;

    public CornerRadius(float radius)
    {
        TopLeft =
            TopRight =
                BottomRight =
                    BottomLeft = radius;
    }

    public CornerRadius(
        float topLeft,
        float topRight,
        float bottomRight,
        float bottomLeft)
    {
        TopLeft = topLeft;
        TopRight = topRight;
        BottomRight = bottomRight;
        BottomLeft = bottomLeft;
    }

    public CornerRadius Scale(float value)
    {
        return new CornerRadius(TopLeft * value, TopRight * value, BottomRight * value, BottomLeft * value);
    }
    
    public CornerRadius Shrink(Thickness thickness)
    {
        return new CornerRadius(
            MathF.Max(0f, TopLeft - MathF.Max(thickness.Left, thickness.Top)),
            MathF.Max(0f, TopRight - MathF.Max(thickness.Right, thickness.Top)),
            MathF.Max(0f, BottomRight - MathF.Max(thickness.Right, thickness.Bottom)),
            MathF.Max(0f, BottomLeft - MathF.Max(thickness.Left, thickness.Bottom)));
    }
    
    public readonly override string ToString()
    {
        return $"{TopLeft},{TopRight},{BottomLeft},{BottomRight}";
    }

    public readonly string ToString(string? format, IFormatProvider? formatProvider)
    {
        return ToString();
    }

    public readonly bool TryFormat(
        Span<char> destination,
        out int charsWritten,
        ReadOnlySpan<char> format,
        IFormatProvider? provider)
    {
        return FormatHelpers.TryFormatInto(
            destination,
            out charsWritten,
            $"{TopLeft},{TopRight},{BottomLeft},{BottomRight}");
    }

    public static CornerRadius Parse(string value, IFormatProvider provider)
    {
        var parts = value.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        static float ParseFloat(string s, IFormatProvider provider)
        {
            return float.Parse(s, provider);
        }

        return parts.Length switch
        {
            1 => new CornerRadius(
                ParseFloat(parts[0], provider)),

            2 => new CornerRadius(
                topLeft: ParseFloat(parts[0], provider),
                topRight: ParseFloat(parts[1], provider),
                bottomRight: ParseFloat(parts[0], provider),
                bottomLeft: ParseFloat(parts[1], provider)),

            3 => new CornerRadius(
                topLeft: ParseFloat(parts[0], provider),
                topRight: ParseFloat(parts[1], provider),
                bottomRight: ParseFloat(parts[2], provider),
                bottomLeft: ParseFloat(parts[1], provider)),

            4 => new CornerRadius(
                topLeft: ParseFloat(parts[0], provider),
                topRight: ParseFloat(parts[1], provider),
                bottomRight: ParseFloat(parts[2], provider),
                bottomLeft: ParseFloat(parts[3], provider)),

            _ => throw new Exception("CornerRadius must contain between 1 and 4 values.")
        };
    }
}