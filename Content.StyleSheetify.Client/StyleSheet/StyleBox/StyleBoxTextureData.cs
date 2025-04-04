﻿using System.Numerics;
using Content.StyleSheetify.Client.Serializer;
using Robust.Client.ResourceManagement;
using Robust.Client.Utility;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;
using IRsiStateLike = Robust.Client.Graphics.IRsiStateLike;
using StyleBoxTexture = Robust.Client.Graphics.StyleBoxTexture;
using Texture = Robust.Client.Graphics.Texture;

namespace Content.StyleSheetify.Client.StyleSheet.StyleBox;

[Serializable, DataDefinition]
public sealed partial class StyleBoxTextureData : StyleBoxData
{
    [DataField(customTypeSerializer: typeof(TextureSerializer))]
    public Texture Texture = default!;

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

    public StyleBoxTexture GetStyleboxTexture()
    {
        var styleBox = new StyleBoxTexture();
        SetBaseParam(ref styleBox);
        styleBox.Texture = Texture;
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

    private IRsiStateLike RsiStateLike(SpriteSpecifier specifier, IDependencyCollection dependencies)
    {
        var resC = dependencies.Resolve<IResourceCache>();
        switch (specifier)
        {
            case SpriteSpecifier.Texture tex:
                return tex.GetTexture(resC);
            case SpriteSpecifier.Rsi rsi:
                return rsi.GetState(resC);
            default:
                throw new NotSupportedException();
        }
    }
}
