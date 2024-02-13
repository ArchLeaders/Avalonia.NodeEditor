using Avalonia.NodeEditor.Demo.Services;
using Avalonia.NodeEditor.Mvvm;

namespace Avalonia.NodeEditor.Demo.ViewModels;

public partial class MainViewModel : ObservableNodeEditor
{
    public MainViewModel()
    {
        Factory = new NodeFactory();
        Serializer = JsonNodeSerializer.Shared;
        Drawing = Factory.CreateDrawing();
        Templates = Factory.CreateTemplates();
    }
}
