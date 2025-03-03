using Robust.Shared.ContentPack;
using Robust.Shared.Log;

namespace Content.StyleSheetify.Client;

public class EntryPoint: GameShared
{
    public override void PreInit()
    {
        Logger.Info("Register some style think...");
        DependencyRegistration.Register(Dependencies);
    }
}
