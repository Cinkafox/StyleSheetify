using Content.StyleSheetify.Client.StyleSheet;
using Robust.Shared.ContentPack;
using Robust.Shared.IoC;

namespace Content.StyleSheetify.Client;

public class EntryPoint: GameShared
{
    public override void PreInit()
    {
        DependencyRegistration.Register(Dependencies);
        IoCManager.BuildGraph();
        IoCManager.Resolve<IContentStyleSheetManagerInternal>().Initialize();
    }
}
