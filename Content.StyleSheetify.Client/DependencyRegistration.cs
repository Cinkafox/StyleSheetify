using Robust.Shared.IoC;
using Content.StyleSheetify.Client.StyleSheet;

namespace Content.StyleSheetify.Client;

public static class DependencyRegistration
{
    public static void Register(IDependencyCollection dc)
    {
        dc.Register<IContentStyleSheetManager, ContentStyleSheetManager>();
    }
}
