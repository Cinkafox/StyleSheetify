using System.Globalization;
using Content.StyleSheetify.Shared.Math;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;
using Robust.Shared.Utility;

namespace Content.StyleSheetify.Shared.Serializer;

[TypeSerializer]
public sealed class CornerRadiusSerializer : ITypeSerializer<CornerRadius, ValueDataNode>, ITypeCopyCreator<CornerRadius>
{
    public ValidationNode Validate(ISerializationManager serializationManager, ValueDataNode node,
        IDependencyCollection dependencies, ISerializationContext? context = null)
    {
        if (!VectorSerializerUtility.TryParseArgs(node.Value, 4, out var args))
        {
            throw new InvalidMappingException($"Could not parse {nameof(Thickness)}: '{node.Value}'");
        }

        return float.TryParse(args[0], NumberStyles.Any, CultureInfo.InvariantCulture, out _) &&
               float.TryParse(args[1], NumberStyles.Any, CultureInfo.InvariantCulture, out _) &&
               float.TryParse(args[2], NumberStyles.Any, CultureInfo.InvariantCulture, out _) &&
               float.TryParse(args[3], NumberStyles.Any, CultureInfo.InvariantCulture, out _)
            ? new ValidatedValueNode(node)
            : new ErrorNode(node, "Failed parsing values for Thickness.");
    }

    public CornerRadius Read(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies,
        SerializationHookContext hookCtx, ISerializationContext? context = null, ISerializationManager.InstantiationDelegate<CornerRadius>? instanceProvider = null)
    {
        if (!VectorSerializerUtility.TryParseArgs(node.Value, 4, out var args))
        {
            throw new InvalidMappingException($"Could not parse {nameof(CornerRadius)}: '{node.Value}'");
        }

        var x = float.Parse(args[0], CultureInfo.InvariantCulture);
        var y = float.Parse(args[1], CultureInfo.InvariantCulture);
        var z = float.Parse(args[2], CultureInfo.InvariantCulture);
        var w = float.Parse(args[3], CultureInfo.InvariantCulture);

        return new CornerRadius(x, y, z, w);
    }

    public DataNode Write(ISerializationManager serializationManager, CornerRadius value, IDependencyCollection dependencies,
        bool alwaysWrite = false, ISerializationContext? context = null)
    {
        return new ValueDataNode($"{value.TopLeft.ToString(CultureInfo.InvariantCulture)}," +
                                 $"{value.TopRight.ToString(CultureInfo.InvariantCulture)}," +
                                 $"{value.BottomRight.ToString(CultureInfo.InvariantCulture)}," +
                                 $"{value.BottomLeft.ToString(CultureInfo.InvariantCulture)}");
    }

    public CornerRadius CreateCopy(ISerializationManager serializationManager, CornerRadius source,
        IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null)
    {
        return new CornerRadius(source.TopLeft, source.TopRight, source.BottomRight, source.BottomLeft);
    }
}