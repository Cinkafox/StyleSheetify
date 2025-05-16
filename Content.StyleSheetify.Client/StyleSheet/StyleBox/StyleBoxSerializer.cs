using Content.StyleSheetify.Shared.Dynamic;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
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
public sealed class StyleBoxSerializer : ITypeSerializer<StyleBoxFlat, MappingDataNode>, ITypeSerializer<StyleBoxTexture, MappingDataNode>, ITypeSerializer<StyleBoxLayers, SequenceDataNode>
{
    public ValidationNode Validate(ISerializationManager serializationManager, MappingDataNode node,
        IDependencyCollection dependencies, ISerializationContext? context = null) =>
        new ValidatedValueNode(node);

    public StyleBoxFlat Read(ISerializationManager serializationManager, MappingDataNode node, IDependencyCollection dependencies,
        SerializationHookContext hookCtx, ISerializationContext? context = null, ISerializationManager.InstantiationDelegate<StyleBoxFlat>? instanceProvider = null) =>
        serializationManager.Read<StyleBoxFlatData?>(node)!;

    public DataNode Write(ISerializationManager serializationManager, StyleBoxFlat value, IDependencyCollection dependencies,
        bool alwaysWrite = false, ISerializationContext? context = null) =>
        throw new NotImplementedException();

    public StyleBoxTexture Read(ISerializationManager serializationManager, MappingDataNode node,
        IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null, ISerializationManager.InstantiationDelegate<StyleBoxTexture>? instanceProvider = null) =>
        serializationManager.Read<StyleBoxTextureData?>(node)!.GetStyleboxTexture(dependencies);

    public DataNode Write(ISerializationManager serializationManager, StyleBoxTexture value, IDependencyCollection dependencies,
        bool alwaysWrite = false, ISerializationContext? context = null) =>
        throw new NotImplementedException();

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

    public DataNode Write(ISerializationManager serializationManager,
        StyleBoxLayers value,
        IDependencyCollection dependencies,
        bool alwaysWrite = false,
        ISerializationContext? context = null) =>
        throw new NotImplementedException();
}
