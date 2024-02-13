using Avalonia.NodeEditor.Core;
using Avalonia.NodeEditor.Core.Mvvm.Extensions;
using Avalonia.NodeEditor.Demo.Views.Nodes;
using Avalonia.NodeEditor.Mvvm;
using CommunityToolkit.Mvvm.Input;

namespace Avalonia.NodeEditor.Demo.ViewModels.Nodes;

public partial class SwitchEventNode : ObservableNode, INodeTemplateProvider
{
    private const string DEFAULT_NAME = "Switch Event";

    public static INodeTemplate Template { get; } = new ObservableNodeTemplate(() => new SwitchEventNode()) {
        Preview = new SwitchEventNode(DEFAULT_NAME),
        Title = DEFAULT_NAME
    };

    public SwitchEventNode() : this(DEFAULT_NAME)
    {
    }

    public SwitchEventNode(string name)
    {
        Name = name;
        SwitchEventNodeView view = new() {
            DataContext = this
        };

        Content = view;
        Width = view.Width;
        Height = view.Height;

        this.AddPin(Width / 2, 0, 10, 10, PinAlignment.Top, "Input");
    }

    [RelayCommand]
    private void Add()
    {
        ArgumentNullException.ThrowIfNull(Pins);

        int x = Pins.Count * 25;
        this.AddPin(x, Height, 10, 10, PinAlignment.Bottom, name: $"{Pins.Count}");
    }

    [RelayCommand]
    private void Remove()
    {
        if (Pins?.LastOrDefault() is IPin pin && Pins.Count > 1) {
            Pins.Remove(pin);
        }
    }
}
