using Avalonia.NodeEditor.Mvvm;
using System.Collections.ObjectModel;

namespace Avalonia.NodeEditor.Core.Mvvm.Extensions;

public static class ObservableNodeExtensions
{
    public static IPin AddPin(this ObservableNode node, double x, double y, double width, double height, PinAlignment alignment = PinAlignment.None, string? name = null)
    {
        ObservablePin pin = new() {
            Name = name,
            Parent = node,
            X = x,
            Y = y,
            Width = width,
            Height = height,
            Alignment = alignment
        };

        node.Pins ??= new ObservableCollection<IPin>();
        node.Pins.Add(pin);

        return pin;
    }
}
