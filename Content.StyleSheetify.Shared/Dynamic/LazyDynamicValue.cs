namespace Content.StyleSheetify.Shared.Dynamic;

public sealed class LazyDynamicValue
{
    private readonly Func<object> _createObject;

    public object Object
    {
        get
        {
            field ??= _createObject.Invoke();
            return field;
        }
    }

    public LazyDynamicValue(Func<object> createObject)
    {
        _createObject = createObject;
    }
}
