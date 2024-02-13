using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.NodeEditor.Core;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.NodeEditor.Behaviors;

public class InsertTemplateOnDoubleTappedBehavior : Behavior<ListBoxItem>
{
    public static readonly StyledProperty<IDrawingNode?> DrawingProperty =
        AvaloniaProperty.Register<InsertTemplateOnDoubleTappedBehavior, IDrawingNode?>(nameof(Drawing));

    public IDrawingNode? Drawing {
        get => GetValue(DrawingProperty);
        set => SetValue(DrawingProperty, value);
    }

    protected override void OnAttached()
    {
        base.OnAttached();
        if (AssociatedObject is not null) {
            AssociatedObject.DoubleTapped += DoubleTapped;
        }
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        if (AssociatedObject is not null) {
            AssociatedObject.DoubleTapped -= DoubleTapped;
        }
    }

    private void DoubleTapped(object? sender, RoutedEventArgs args)
    {
        if (AssociatedObject is { DataContext: INodeTemplate template } && Drawing is { } drawing) {
            INode node = template.Template();
            node.Parent = drawing;
            node.Move(0.0, 0.0);
            drawing.Nodes?.Add(node);
            node.OnCreated();
        }
    }
}
