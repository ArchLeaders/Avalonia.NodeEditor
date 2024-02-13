using Avalonia.NodeEditor.Core;
using Avalonia.NodeEditor.Demo.ViewModels;
using Avalonia.NodeEditor.Demo.ViewModels.Nodes;

namespace Avalonia.NodeEditor.Demo.Services;

public class NodeFactory : INodeFactory
{
    public IDrawingNode CreateDrawing(string? name = null)
    {
        return new DemoDrawingNodeViewModel(900, 600) {
            EnableGrid = false,
            EnableMultiplePinConnections = false,
            EnableSnap = true,
            GridCellHeight = 15.0,
            GridCellWidth = 15.0,
            SnapX = 15.0,
            SnapY = 15.0
        };
    }

    public IList<INodeTemplate> CreateTemplates()
    {
        return [
            ActionEventNode.Template,
            SwitchEventNode.Template
        ];
    }
}
