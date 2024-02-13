namespace Avalonia.NodeEditor.Core;

public interface INodeTemplate
{
    string? Title { get; set; }
    Func<INode> Template { get; set; }
    INode? Preview { get; set; }
}
