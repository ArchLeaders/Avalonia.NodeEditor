namespace Avalonia.NodeEditor.Core;

public interface INodeFactory
{
    IList<INodeTemplate> CreateTemplates();
    IDrawingNode CreateDrawing(string? name = null);
}
