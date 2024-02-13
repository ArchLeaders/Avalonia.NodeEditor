using Avalonia.NodeEditor.Core;
using Avalonia.NodeEditor.Core.Mvvm.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Avalonia.NodeEditor.Demo.Services;

public class JsonNodeSerializer : INodeSerializer
{
    private static readonly JsonSerializerOptions _options = new() {
        NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals
    };

    private static readonly Lazy<JsonNodeSerializer> _shared = new(() => new());
    public static JsonNodeSerializer Shared => _shared.Value;

    static JsonNodeSerializer()
    {
        _options.Converters.Add(new ObservableCollectionConverter());
        _options.Converters.Add(new ObservableNodeConverter());
    }

    public T? Deserialize<T>(string text)
    {
        return JsonSerializer.Deserialize<T>(text, _options);
    }

    public string Serialize<T>(T value)
    {
        return JsonSerializer.Serialize(value, _options);
    }
}
