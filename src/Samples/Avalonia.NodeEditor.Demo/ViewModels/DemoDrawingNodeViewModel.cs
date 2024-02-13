using Avalonia.NodeEditor.Core;
using Avalonia.NodeEditor.Demo.Services;
using Avalonia.NodeEditor.Mvvm;

namespace Avalonia.NodeEditor.Demo.ViewModels;

public class DemoDrawingNodeViewModel : ObservableDrawingNode
{
    public DemoDrawingNodeViewModel(double width, double height)
    {
        SetSerializer(JsonNodeSerializer.Shared);
        Width = width;
        Height = height;
    }

    public override bool CanConnectPin(IPin pin)
    {
        bool isConnecting = _editor.IsConnectorMoving();
        if (pin.Name is "Input") {
            return isConnecting;
        }

        if (pin.Parent is INode node && node.Name == "Fork") {
            return true;
        }

        return base.CanConnectPin(pin);
    }
}
