using Avalonia.NodeEditor.Core;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Avalonia.NodeEditor.Mvvm;

public partial class ObservableNodeEditor : ObservableObject, INodeTemplatesHost, IEditor
{
    [ObservableProperty]
    private INodeSerializer? _serializer;

    [ObservableProperty]
    private INodeFactory? _factory;

    [ObservableProperty]
    private IList<INodeTemplate>? _templates;

    [ObservableProperty]
    private IDrawingNode? _drawing;
}
