using Avalonia.NodeEditor.Mvvm;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Avalonia.NodeEditor.Core.Mvvm.Json;

public class ObservableNodeConverter : JsonConverter<INode>
{
    public override INode? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return JsonSerializer.Deserialize<ObservableNode>(ref reader, options);
    }

    public override void Write(Utf8JsonWriter writer, INode value, JsonSerializerOptions options)
    {
        if (value is ObservableNode node) {
            JsonSerializer.Serialize(writer, node, options);
            return;
        }

        throw new NotSupportedException($"""
            The serialization of other {nameof(INode)} implementations is not supported
            """);
    }
}
