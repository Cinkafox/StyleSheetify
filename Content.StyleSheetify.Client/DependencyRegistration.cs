using Robust.Shared.IoC;
using StyleSheetify.Client.StyleSheet;

namespace StyleSheetify.Client;

public static class DependencyRegistration
{
    public static void Register(IDependencyCollection dc)
    {
        dc.Register<IStyleSheetManager, StyleSheetManager>();
    }
}