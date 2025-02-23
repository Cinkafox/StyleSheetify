using System.Numerics;
using System.Text;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.Analyzers;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;
using Robust.Shared.Utility;

namespace Content.StyleSheetify.Client.Font;


[Serializable, DataDefinition, Virtual]
public partial class FontSpecifier : Robust.Client.Graphics.Font
{
    [DataField] public int Size;
    [DataField("font")] public List<ResPath> Path = [];

    public FontSpecifier() {}

    public FontSpecifier(List<ResPath> resPath, int size)
    {
        Path = resPath;
        Size = size;
    }

    protected Robust.Client.Graphics.Font? _font;

    public Robust.Client.Graphics.Font GetFont()
    {
        if (_font != null) return _font;
        _font = GetFont(IoCManager.Resolve<IResourceCache>(), Path, Size);
        return _font;
    }

    public override int GetAscent(float scale)
    {
        return GetFont().GetAscent(scale);
    }

    public override int GetHeight(float scale)
    {
        return GetFont().GetHeight(scale);
    }

    public override int GetDescent(float scale)
    {
        return GetFont().GetDescent(scale);
    }

    public override int GetLineHeight(float scale)
    {
        return GetFont().GetLineHeight(scale);
    }

    public override float DrawChar(DrawingHandleBase handle, Rune rune, Vector2 baseline, float scale, Color color, bool fallback = true)
    {
        return GetFont().DrawChar(handle, rune, baseline, scale, color, fallback);
    }

    public override CharMetrics? GetCharMetrics(Rune rune, float scale, bool fallback = true)
    {
        return GetFont().GetCharMetrics(rune, scale, fallback);
    }

    private static Robust.Client.Graphics.Font GetFont(IResourceCache cache, List<ResPath> path, int size)
    {
        var fs = new Robust.Client.Graphics.Font[path.Count];
        for (var i = 0; i < path.Count; i++)
            fs[i] = new VectorFont(cache.GetResource<FontResource>(path[i]), size);

        return new StackedFont(fs);
    }
}

[TypeSerializer]
public sealed class FontSerializer : ITypeSerializer<Robust.Client.Graphics.Font, MappingDataNode>
{
    public ValidationNode Validate(ISerializationManager serializationManager, MappingDataNode node,
        IDependencyCollection dependencies, ISerializationContext? context = null)
    {
        if (!node.TryGet("font", out var pathNode) || !node.TryGet("size", out var sizeNode))
            return new ErrorNode(node, "no font or size found!");
        return new ValidatedValueNode(pathNode);
    }

    public Robust.Client.Graphics.Font Read(ISerializationManager serializationManager, MappingDataNode node, IDependencyCollection dependencies,
        SerializationHookContext hookCtx, ISerializationContext? context = null, ISerializationManager.InstantiationDelegate<Robust.Client.Graphics.Font>? instanceProvider = null)
    {
        if (!node.TryGet("font", out var pathNode) || !node.TryGet("size", out var sizeNode))
            throw new Exception();
        var path = serializationManager.Read<List<ResPath>>(pathNode);
        var size = serializationManager.Read<int>(sizeNode);

        return new FontSpecifier(path, size);
    }

    public DataNode Write(ISerializationManager serializationManager, Robust.Client.Graphics.Font value, IDependencyCollection dependencies,
        bool alwaysWrite = false, ISerializationContext? context = null)
    {
        if (value is FontSpecifier fontSpecifier)
            return serializationManager.WriteValue(fontSpecifier);
        throw new Exception();
    }
}
