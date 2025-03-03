using Robust.Shared.Prototypes;

namespace Content.StyleSheetify.Shared;

public static class StylePrototypeIgnorance
{
    public static void Register(IPrototypeManager prototypes)
    {
        prototypes.RegisterIgnore("styleSheet");
        prototypes.RegisterIgnore("dynamicValue");
    }
}
