using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.StyleSheetify.Shared.Dynamic;

[Prototype("dynamicValue")]
public sealed partial class DynamicValuePrototype: IPrototype
{
    [IdDataField]
    public string ID { get; private set; } = default!;

    [DataField] public DynamicValue Value = default!;
}