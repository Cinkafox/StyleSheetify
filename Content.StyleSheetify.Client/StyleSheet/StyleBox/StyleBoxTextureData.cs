using System.Numerics;
using Robust.Client.ResourceManagement;
using Robust.Client.Utility;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;
using StyleBoxTexture = Robust.Client.Graphics.StyleBoxTexture;
using Texture = Robust.Client.Graphics.Texture;


namespace Content.StyleSheetify.Client.StyleSheet.StyleBox;

[Serializable, DataDefinition]
public sealed partial class StyleBoxTextureData : StyleBoxData
{
    // TODO: Make another logic for detect test build
    public static bool IgnoreTextures;
    public static Dictionary<int, ResPath> Paths = new();

    [DataField]
    public ResPath Texture;

    /// <summary>
    /// Left expansion size, in virtual pixels.
    /// </summary>
    /// <remarks>
    /// This expands the size of the area where the patches get drawn. This will cause the drawn texture to
    /// extend beyond the box passed to the <see cref="StyleBox.Draw"/> function. This is not affected by
    /// <see cref="TextureScale"/>.
    /// </remarks>
    [DataField] public float? ExpandMarginLeft;
    /// <summary>
    /// Top expansion size, in virtual pixels.
    /// </summary>
    /// <remarks>
    /// This expands the size of the area where the patches get drawn. This will cause the drawn texture to
    /// extend beyond the box passed to the <see cref="StyleBox.Draw"/> function. This is not affected by
    /// <see cref="TextureScale"/>.
    /// </remarks>
    [DataField] public float? ExpandMarginTop;

    /// <summary>
    /// Bottom expansion size, in virtual pixels.
    /// </summary>
    /// <remarks>
    /// This expands the size of the area where the patches get drawn. This will cause the drawn texture to
    /// extend beyond the box passed to the <see cref="StyleBox.Draw"/> function. This is not affected by
    /// <see cref="TextureScale"/>.
    /// </remarks>
    [DataField] public float? ExpandMarginBottom ;

    /// <summary>
    /// Right expansion size, in virtual pixels.
    /// </summary>
    /// <remarks>
    /// This expands the size of the area where the patches get drawn. This will cause the drawn texture to
    /// extend beyond the box passed to the <see cref="StyleBox.Draw"/> function. This is not affected by
    /// <see cref="TextureScale"/>.
    /// </remarks>
    [DataField] public float? ExpandMarginRight;

    [DataField] public float? ExpandMarginAll;

    [DataField] public StyleBoxTexture.StretchMode StretchMode = StyleBoxTexture.StretchMode.Stretch;

    /// <summary>
    /// Distance of the left patch margin from the image. In texture space.
    /// The size of this patch in virtual pixels can be obtained by scaling this with <see cref="TextureScale"/>.
    /// </summary>
    [DataField] public float? PatchMarginLeft;
    /// <summary>
    /// Distance of the right patch margin from the image. In texture space.
    /// The size of this patch in virtual pixels can be obtained by scaling this with <see cref="TextureScale"/>.
    /// </summary>
    [DataField] public float? PatchMarginRight;

    /// <summary>
    /// Distance of the top patch margin from the image. In texture space.
    /// The size of this patch in virtual pixels can be obtained by scaling this with <see cref="TextureScale"/>.
    /// </summary>
    [DataField] public float? PatchMarginTop;

    /// <summary>
    /// Distance of the bottom patch margin from the image. In texture space.
    /// The size of this patch in virtual pixels can be obtained by scaling this with <see cref="TextureScale"/>.
    /// </summary>
    [DataField] public float? PatchMarginBottom;
    [DataField] public float? PatchMarginAll;

    [DataField] public Thickness? PatchMargin;
    [DataField] public Thickness? ExpandMargin;

    [DataField] public Color Modulate = Color.White;

    /// <summary>
    /// Additional scaling to use when drawing the texture.
    /// </summary>
    [DataField] public Vector2 TextureScale  = Vector2.One;

    public StyleBoxTexture GetStyleboxTexture(IDependencyCollection dependencyCollection)
    {
        var styleBox = new StyleBoxTexture();
        SetBaseParam(ref styleBox);

        var resCache = dependencyCollection.Resolve<IResourceCache>();

        if (!IgnoreTextures &&
            resCache.TryGetResource<TextureResource>(Texture, out var texture))
        {
            Paths.TryAdd(texture.GetHashCode(), Texture);
            styleBox.Texture = texture;
        }

        styleBox.Mode = StretchMode;
        styleBox.Modulate = Modulate;
        styleBox.TextureScale = TextureScale;

        if (ExpandMargin is null)
        {
            if (ExpandMarginAll is { } expandMarginAll)
            {
                styleBox.ExpandMarginBottom = expandMarginAll;
                styleBox.ExpandMarginTop = expandMarginAll;
                styleBox.ExpandMarginRight = expandMarginAll;
                styleBox.ExpandMarginLeft = expandMarginAll;
            }
        }
        else
        {
            styleBox.ExpandMarginBottom = ExpandMargin.Value.Bottom;
            styleBox.ExpandMarginTop = ExpandMargin.Value.Top;
            styleBox.ExpandMarginRight = ExpandMargin.Value.Right;
            styleBox.ExpandMarginLeft = ExpandMargin.Value.Left;
        }

        if (ExpandMarginBottom != null)
            styleBox.ExpandMarginBottom = ExpandMarginBottom.Value;
        if (ExpandMarginTop != null)
            styleBox.ExpandMarginTop = ExpandMarginTop.Value;
        if (ExpandMarginRight != null)
            styleBox.ExpandMarginRight = ExpandMarginRight.Value;
        if (ExpandMarginLeft != null)
            styleBox.ExpandMarginLeft = ExpandMarginLeft.Value;

        if (PatchMargin is null)
        {
            if (PatchMarginAll is { } patchMarginAll)
            {
                styleBox.PatchMarginBottom = patchMarginAll;
                styleBox.PatchMarginTop = patchMarginAll;
                styleBox.PatchMarginRight = patchMarginAll;
                styleBox.PatchMarginLeft = patchMarginAll;
            }
        }
        else
        {
            styleBox.PatchMarginBottom = PatchMargin.Value.Bottom;
            styleBox.PatchMarginTop = PatchMargin.Value.Top;
            styleBox.PatchMarginRight = PatchMargin.Value.Right;
            styleBox.PatchMarginLeft = PatchMargin.Value.Left;
        }


        if (PatchMarginBottom != null)
            styleBox.PatchMarginBottom = PatchMarginBottom.Value;
        if (PatchMarginTop != null)
            styleBox.PatchMarginTop = PatchMarginTop.Value;
        if (PatchMarginRight != null)
            styleBox.PatchMarginRight = PatchMarginRight.Value;
        if (PatchMarginLeft != null)
            styleBox.PatchMarginLeft = PatchMarginLeft.Value;

        return styleBox;
    }

    public static StyleBoxTextureData From(StyleBoxTexture value)
    {
        var styleBox = new StyleBoxTextureData();

        styleBox.GetBaseParam(value);
        if (Paths.TryGetValue(value.GetHashCode(), out var path))
            styleBox.Texture = path;

        styleBox.TextureScale = value.TextureScale;
        styleBox.Modulate = value.Modulate;
        styleBox.ExpandMarginBottom = value.ExpandMarginBottom;
        styleBox.ExpandMarginTop = value.ExpandMarginTop;
        styleBox.ExpandMarginRight = value.ExpandMarginRight;
        styleBox.ExpandMarginLeft = value.ExpandMarginLeft;

        styleBox.PatchMarginBottom = value.PatchMarginBottom;
        styleBox.PatchMarginTop = value.PatchMarginTop;
        styleBox.PatchMarginRight = value.PatchMarginRight;
        styleBox.PatchMarginLeft = value.PatchMarginLeft;

        return styleBox;
    }
}
