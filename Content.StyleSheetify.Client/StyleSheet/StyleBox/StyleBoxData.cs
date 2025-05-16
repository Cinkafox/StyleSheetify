using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.StyleSheetify.Client.StyleSheet.StyleBox;

[Serializable]
public abstract class StyleBoxData
{
        /// <summary>
        /// Left content margin, in virtual pixels.
        /// </summary>
        [DataField] public float? ContentMarginLeftOverride;

        /// <summary>
        /// Top content margin, in virtual pixels.
        /// </summary>
        [DataField] public float? ContentMarginTopOverride;

        /// <summary>
        /// Right content margin, in virtual pixels.
        /// </summary>
        [DataField] public float? ContentMarginRightOverride;

        /// <summary>
        /// Bottom content margin, in virtual pixels.
        /// </summary>
        [DataField] public float? ContentMarginBottomOverride;

        [DataField] public float? ContentMarginOverrideAll;

        /// <summary>
        /// Left padding, in virtual pixels.
        /// </summary>
        [DataField]  public float PaddingLeft;

        /// <summary>
        /// Bottom padding, in virtual pixels.
        /// </summary>
        [DataField] public float PaddingBottom;

        /// <summary>
        /// Right padding, in virtual pixels.
        /// </summary>
        [DataField] public float PaddingRight;

        /// <summary>
        /// Top padding, in virtual pixels.
        /// </summary>
        [DataField] public float PaddingTop;

        /// <summary>
        /// Padding, in virtual pixels.
        /// </summary>
        [DataField] public Thickness? Padding;

        [DataField] public float? PaddingAll;

        [DataField] public float? ContentMarginVerticalOverride;
        [DataField] public float? ContentMarginHorizontalOverride;

        public void SetBaseParam<T>(ref T styleBox) where T : Robust.Client.Graphics.StyleBox
        {
            if (ContentMarginOverrideAll is { } all)
            {
                styleBox.ContentMarginBottomOverride = all;
                styleBox.ContentMarginLeftOverride = all;
                styleBox.ContentMarginTopOverride = all;
                styleBox.ContentMarginRightOverride = all;
            }

            if (ContentMarginBottomOverride is not null)
                styleBox.ContentMarginBottomOverride = ContentMarginBottomOverride;
            if (ContentMarginLeftOverride is not null)
                styleBox.ContentMarginLeftOverride = ContentMarginLeftOverride;
            if (ContentMarginTopOverride is not null)
                styleBox.ContentMarginTopOverride = ContentMarginTopOverride;
            if (ContentMarginRightOverride is not null)
                styleBox.ContentMarginRightOverride = ContentMarginRightOverride;

            if (ContentMarginVerticalOverride is { } verticalOverride)
            {
                styleBox.SetContentMarginOverride(Robust.Client.Graphics.StyleBox.Margin.Vertical, verticalOverride);
            }

            if (ContentMarginHorizontalOverride is { } horizontalOverride)
            {
                styleBox.SetContentMarginOverride(Robust.Client.Graphics.StyleBox.Margin.Horizontal, horizontalOverride);
            }

            if (Padding is not null)
            {
                styleBox.Padding = Padding.Value;
                return;
            }

            if (PaddingAll is { } paddingAll)
            {
                styleBox.PaddingBottom = paddingAll;
                styleBox.PaddingLeft = paddingAll;
                styleBox.PaddingRight = paddingAll;
                styleBox.PaddingTop = paddingAll;
            }

            styleBox.PaddingBottom = PaddingBottom;
            styleBox.PaddingLeft = PaddingLeft;
            styleBox.PaddingRight = PaddingRight;
            styleBox.PaddingTop = PaddingTop;
        }

        public void GetBaseParam<T>(T styleBox) where T : Robust.Client.Graphics.StyleBox
        {
            ContentMarginLeftOverride = styleBox.ContentMarginLeftOverride;
            ContentMarginTopOverride = styleBox.ContentMarginTopOverride;
            ContentMarginRightOverride = styleBox.ContentMarginRightOverride;
            ContentMarginBottomOverride = styleBox.ContentMarginBottomOverride;
            PaddingLeft = styleBox.PaddingLeft;
            PaddingTop = styleBox.PaddingTop;
            PaddingRight = styleBox.PaddingRight;
            PaddingBottom = styleBox.PaddingBottom;
        }
}

