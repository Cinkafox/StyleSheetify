using Content.StyleSheetify.Client.StyleSheet;
using Robust.Shared.ContentPack;
using Robust.Shared.IoC;

namespace Content.StyleSheetify.Client;

public class EntryPoint: GameShared
{
    [Dependency] private readonly IContentStyleSheetManagerInternal _contentStyleSheetManagerInternal = default!;
    public override void PreInit()
    {
        DependencyRegistration.Register(Dependencies);
        IoCManager.BuildGraph();
        IoCManager.InjectDependencies(this);
        _contentStyleSheetManagerInternal.Initialize();
    }

    public override void PostInit()
    {
        _contentStyleSheetManagerInternal.PostInitialize();
    }
}
