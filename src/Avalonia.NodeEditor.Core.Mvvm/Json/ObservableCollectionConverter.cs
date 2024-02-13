using System.Text.Json;
using System.Text.Json.Serialization;

namespace Avalonia.NodeEditor.Core.Mvvm.Json;

public class ObservableCollectionConverter : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsAssignableTo(typeof(IList<>));
    }

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        Type generic = typeToConvert.GetGenericArguments()[0];
        return Activator.CreateInstance(typeof(GenericListConverter<>)
            .MakeGenericType(generic)) as JsonConverter;
    }

    private class GenericListConverter<T> : JsonConverter<T>
    {
        public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return JsonSerializer.Deserialize<T>(ref reader, options);
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, options);
        }
    }
}
