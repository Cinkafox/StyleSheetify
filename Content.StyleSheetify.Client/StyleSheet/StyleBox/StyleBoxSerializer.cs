using Content.StyleSheetify.Shared.Dynamic;
using Robust.Client.Graphics;
using Robust.Shared.IoC;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Sequence;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;

namespace Content.StyleSheetify.Client.StyleSheet.StyleBox;

[TypeSerializer]
public sealed class StyleBoxFlatSerializer : ITypeSerializer<StyleBoxFlat, MappingDataNode>, ITypeCopyCreator<StyleBoxFlat>
{
    public ValidationNode Validate(ISerializationManager serializationManager, MappingDataNode node,
        IDependencyCollection dependencies, ISerializationContext? context = null) =>
        new ValidatedValueNode(node);

    public StyleBoxFlat Read(ISerializationManager serializationManager, MappingDataNode node, IDependencyCollection dependencies,
        SerializationHookContext hookCtx, ISerializationContext? context = null, ISerializationManager.InstantiationDelegate<StyleBoxFlat>? instanceProvider = null) =>
        serializationManager.Read<StyleBoxFlatData>(node);

    public DataNode Write(ISerializationManager serializationManager, StyleBoxFlat value, IDependencyCollection dependencies,
        bool alwaysWrite = false, ISerializationContext? context = null) =>
        serializationManager.WriteValue<StyleBoxFlatData>(StyleBoxFlatData.From(value));

    public StyleBoxFlat CreateCopy(
        ISerializationManager serializationManager,
        StyleBoxFlat source,
        IDependencyCollection dependencies,
        SerializationHookContext hookCtx,
        ISerializationContext? context = null
    ) =>
        serializationManager.CreateCopy(StyleBoxFlatData.From(source));
}


[TypeSerializer]
public sealed class StyleBoxTextureSerializer : ITypeSerializer<StyleBoxTexture, MappingDataNode>, ITypeCopyCreator<StyleBoxTexture>
{
    public ValidationNode Validate(ISerializationManager serializationManager, MappingDataNode node,
        IDependencyCollection dependencies, ISerializationContext? context = null) =>
        new ValidatedValueNode(node);

    public StyleBoxTexture Read(ISerializationManager serializationManager, MappingDataNode node,
        IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null, ISerializationManager.InstantiationDelegate<StyleBoxTexture>? instanceProvider = null) =>
        serializationManager.Read<StyleBoxTextureData>(node).GetStyleboxTexture(dependencies);

    public DataNode Write(ISerializationManager serializationManager, StyleBoxTexture value, IDependencyCollection dependencies,
        bool alwaysWrite = false, ISerializationContext? context = null) =>
        serializationManager.WriteValue<StyleBoxTextureData>(StyleBoxTextureData.From(value));

    public StyleBoxTexture CreateCopy(
        ISerializationManager serializationManager,
        StyleBoxTexture source,
        IDependencyCollection dependencies,
        SerializationHookContext hookCtx,
        ISerializationContext? context = null
    ) =>
        serializationManager.CreateCopy(StyleBoxTextureData.From(source)).GetStyleboxTexture(dependencies);
}


[TypeSerializer]
public sealed class StyleBoxLayersSerializer : ITypeSerializer<StyleBoxLayers, SequenceDataNode> , ITypeCopyCreator<StyleBoxLayers>
{
    public ValidationNode Validate(ISerializationManager serializationManager,
        SequenceDataNode node,
        IDependencyCollection dependencies,
        ISerializationContext? context = null) =>
        new ValidatedValueNode(node);

    public StyleBoxLayers Read(ISerializationManager serializationManager,
        SequenceDataNode nodes,
        IDependencyCollection dependencies,
        SerializationHookContext hookCtx,
        ISerializationContext? context = null,
        ISerializationManager.InstantiationDelegate<StyleBoxLayers>? instanceProvider = null)
    {
        var styleBoxLayers = new StyleBoxLayers();
        foreach (var dataNode in nodes)
        {
            var datum = serializationManager.Read<DynamicValue>(dataNode);
            if (datum.GetValueObject() is Robust.Client.Graphics.StyleBox styleBox)
            {
                styleBoxLayers.Layers.Add(styleBox);
            }
        }

        return styleBoxLayers;
    }

    public DataNode Write(
        ISerializationManager serializationManager,
        StyleBoxLayers value,
        IDependencyCollection dependencies,
        bool alwaysWrite = false,
        ISerializationContext? context = null
    )
    {
        var seq = new SequenceDataNode();
        foreach (var layer in value.Layers)
        {
            seq.Add(serializationManager.WriteValue(layer));
        }

        return seq;
    }

    public StyleBoxLayers CreateCopy(
        ISerializationManager serializationManager,
        StyleBoxLayers source,
        IDependencyCollection dependencies,
        SerializationHookContext hookCtx,
        ISerializationContext? context = null
    )
    {
        var list = new List<Robust.Client.Graphics.StyleBox>();

        foreach (var layer in source.Layers)
        {
            list.Add(serializationManager.CreateCopy(layer));
        }

        return new StyleBoxLayers()
        {
            Layers = list
        };
    }
}
