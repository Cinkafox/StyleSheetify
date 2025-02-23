using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;
using Robust.Shared.Utility;

namespace Content.StyleSheetify.Client.Serializer;

[TypeSerializer]
public sealed class TextureSerializer : ITypeSerializer<Texture, ValueDataNode>, ITypeSerializer<Texture, MappingDataNode>
{
    public ValidationNode Validate(ISerializationManager serializationManager, ValueDataNode node,
        IDependencyCollection dependencies, ISerializationContext? context = null)
    {
        return new ValidatedValueNode(node);
    }

    public ValidationNode Validate(ISerializationManager serializationManager,
        MappingDataNode node,
        IDependencyCollection dependencies,
        ISerializationContext? context = null)
    {
        if (!node.TryGet("baseTexture", out var baseTexDataNode) || !node.TryGet("region", out var regionDataNode))
            return new ErrorNode(node, "no base texture or region");
        return new ValidatedValueNode(node);
    }

    public Texture Read(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies,
        SerializationHookContext hookCtx, ISerializationContext? context = null,
        ISerializationManager.InstantiationDelegate<Texture>? instanceProvider = null)
    {
        var path = serializationManager.Read<ResPath>(node);
        dependencies.Resolve<ILogManager>().RootSawmill.Info($"LOAD TEXTURE {path}");
        var tr = dependencies.Resolve<IResourceCache>().GetResource<TextureResource>(path);
        return tr.Texture;
    }

    public DataNode Write(ISerializationManager serializationManager, Texture value, IDependencyCollection dependencies,
        bool alwaysWrite = false, ISerializationContext? context = null)
    {
        throw new NotImplementedException();
    }

    public Texture Read(ISerializationManager serializationManager,
        MappingDataNode node,
        IDependencyCollection dependencies,
        SerializationHookContext hookCtx,
        ISerializationContext? context = null,
        ISerializationManager.InstantiationDelegate<Texture>? instanceProvider = null)
    {
        if (!node.TryGet("baseTexture", out var baseTexDataNode) || !node.TryGet("region", out var regionDataNode))
            throw new Exception();

        var texture = serializationManager.Read<Texture>(baseTexDataNode);
        var region = serializationManager.Read<UIBox2>(regionDataNode);

        return new AtlasTexture(texture, region);
    }
}
