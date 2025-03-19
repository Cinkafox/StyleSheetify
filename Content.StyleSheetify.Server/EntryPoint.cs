using Robust.Shared.ContentPack;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.StyleSheetify.Server;

public class EntryPoint : GameShared
{
    public override void PreInit()
    {
        var prototypes = IoCManager.Resolve<IPrototypeManager>();
        Shared.StylePrototypeIgnorance.Register(prototypes);
    }
}
