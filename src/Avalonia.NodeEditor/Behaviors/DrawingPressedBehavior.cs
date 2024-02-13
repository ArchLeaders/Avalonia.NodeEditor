using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.NodeEditor.Core;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.NodeEditor.Behaviors;

public class DrawingPressedBehavior : Behavior<Control>
{
    protected override void OnAttached()
    {
        base.OnAttached();

        if (AssociatedObject is { }) {
            AssociatedObject.AddHandler(InputElement.PointerPressedEvent, Pressed, RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
        }
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();

        if (AssociatedObject is { }) {
            AssociatedObject.RemoveHandler(InputElement.PointerPressedEvent, Pressed);
        }
    }

    private void Pressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.Handled) {
            return;
        }

        if (AssociatedObject?.DataContext is not IDrawingNode drawingNode) {
            return;
        }

        if (e.Source is Control { DataContext: IPin }) {
            return;
        }

        PointerPoint info = e.GetCurrentPoint(AssociatedObject);
        (double x, double y) = e.GetPosition(AssociatedObject);

        if (info.Properties.IsLeftButtonPressed) {
            drawingNode.DrawingLeftPressed(x, y);
        }
        else if (info.Properties.IsRightButtonPressed) {
            drawingNode.DrawingRightPressed(x, y);
        }
    }
}
