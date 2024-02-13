using Avalonia.NodeEditor.Core;
using Avalonia.NodeEditor.Core.Mvvm.Extensions;
using Avalonia.NodeEditor.Demo.Views.Nodes;
using Avalonia.NodeEditor.Mvvm;

namespace Avalonia.NodeEditor.Demo.ViewModels.Nodes;

public class ActionEventNode : ObservableNode, INodeTemplateProvider
{
    private const string DEFAULT_NAME = "Action Event";

    public static INodeTemplate Template { get; } = new ObservableNodeTemplate(() => new ActionEventNode()) {
        Preview = new ActionEventNode(DEFAULT_NAME),
        Title = DEFAULT_NAME
    };

    public ActionEventNode() : this(DEFAULT_NAME)
    {
    }

    public ActionEventNode(string name)
    {
        Name = name;
        SwitchEventNodeView view = new() {
            DataContext = this
        };

        Content = view;
        Width = view.Width;
        Height = view.Height;

        this.AddPin(Width / 2, 0, 10, 10, PinAlignment.Top, "Input");
        this.AddPin(Width / 2, Height, 10, 10, PinAlignment.Bottom, "Output");
    }
}
