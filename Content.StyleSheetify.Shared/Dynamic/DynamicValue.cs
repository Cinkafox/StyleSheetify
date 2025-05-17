using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.StyleSheetify.Shared.Dynamic;

public sealed class DynamicValue
{
    public static string ReadByPrototypeCommand = "readByPrototype";

    private string _valueType = ReadByPrototypeCommand;
    private object _value = default!;

    public object GetValueObject()
    {
        if (_value is LazyDynamicValue lazyDynamicValue)
            _value = lazyDynamicValue.Object;

        return _value;
    }

    public string GetValueType()
    {
        return _valueType;
    }

    public DynamicValue(string valueType, object value)
    {
        _valueType = valueType;
        _value = value;
    }
}
