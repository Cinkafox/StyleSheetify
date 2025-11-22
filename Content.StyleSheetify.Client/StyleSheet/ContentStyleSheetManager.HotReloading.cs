using System.Diagnostics;
using Robust.Client.UserInterface;
using Robust.Shared.Prototypes;


namespace Content.StyleSheetify.Client.StyleSheet;


internal sealed partial class ContentStyleSheetManager
{
    private void OnPrototypeReloaded(PrototypesReloadedEventArgs args)
    {
        if(!args.ByType.TryGetValue(typeof(StyleSheetPrototype), out var styleSheetPrototype))
            return;

        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var changedStyles = new Dictionary<Stylesheet, Stylesheet>();

        foreach (var (modifiedPrototype, _) in styleSheetPrototype.Modified)
        {
            if(!_styleCacne.TryGetValue(modifiedPrototype, out var cache))
                continue;

            var outdatedStyle = cache.Updated;
            var updatedStyle = MergeStyles(cache.Original, modifiedPrototype);
            changedStyles[outdatedStyle] = updatedStyle;
        }

        if (_userInterfaceManager.Stylesheet != null &&
            changedStyles.TryGetValue(_userInterfaceManager.Stylesheet, out var userStyleSheet))
        {
            _userInterfaceManager.Stylesheet = userStyleSheet;
        }

        foreach (var rootControl in _userInterfaceManager.AllRoots)
        {
            UpdateControlStylesheet(rootControl, changedStyles);
        }

        _logger.Info($"Updated stylesheets in {stopwatch.ElapsedMilliseconds} ms");
    }

    private void UpdateControlStylesheet(Control control, Dictionary<Stylesheet, Stylesheet> changedStyles)
    {
        if (control.Stylesheet != null &&
            changedStyles.TryGetValue(control.Stylesheet, out var updatedStyle))
        {
            control.Stylesheet = updatedStyle;
        }

        control.InvalidateStyleSheet();

        foreach (var child in control.Children)
        {
            UpdateControlStylesheet(child, changedStyles);
        }
    }
}
