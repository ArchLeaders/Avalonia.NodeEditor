using Avalonia.NodeEditor.Core;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Avalonia.NodeEditor.Mvvm;

public partial class ObservableNodeTemplate(Func<INode> template) : ObservableObject, INodeTemplate
{
    [ObservableProperty]
    private string? _title;

    [ObservableProperty]
    private Func<INode> _template = template;

    [ObservableProperty]
    private INode? _preview;
}
