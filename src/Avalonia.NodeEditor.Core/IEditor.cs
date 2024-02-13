namespace Avalonia.NodeEditor.Core;

public interface IEditor
{
    IList<INodeTemplate>? Templates { get; set; }
    IDrawingNode? Drawing { get; set; }
}
