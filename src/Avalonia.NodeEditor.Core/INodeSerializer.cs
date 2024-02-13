namespace Avalonia.NodeEditor.Core;

public interface INodeSerializer
{
    string Serialize<T>(T value);
    T? Deserialize<T>(string text);
}