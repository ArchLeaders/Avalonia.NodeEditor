using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;
using NodeEditor.Model;

namespace NodeEditor.Behaviors;

public class DrawingMovedBehavior : Behavior<ItemsControl>
{
    public static readonly StyledProperty<Control?> InputSourceProperty =
        AvaloniaProperty.Register<DrawingMovedBehavior, Control?>(nameof(InputSource));

    private Control? _inputSource;
    public Control? InputSource {
        get => GetValue(InputSourceProperty);
        set => SetValue(InputSourceProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == InputSourceProperty) {
            Cleanup();

            if (AssociatedObject is { } && InputSource is { }) {
                Initialize();
            }
        }
    }

    protected override void OnAttached()
    {
        base.OnAttached();
        Initialize();

    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        Cleanup();
    }

    private void Initialize()
    {
        if (AssociatedObject is null || InputSource is null) {
            return;
        }

        _inputSource = InputSource;
        _inputSource.AddHandler(InputElement.PointerMovedEvent, Moved, RoutingStrategies.Tunnel);
    }

    private void Cleanup()
    {
        if (AssociatedObject is null || InputSource is null) {
            return;
        }

        _inputSource = InputSource;
        _inputSource.RemoveHandler(InputElement.PointerMovedEvent, Moved);
    }

    private void Moved(object? sender, PointerEventArgs e)
    {
        if (AssociatedObject?.DataContext is not IDrawingNode drawingNode) {
            return;
        }

        PointerPoint info = e.GetCurrentPoint(_inputSource);
        if (info.Pointer.Type == PointerType.Mouse) {
            (double x, double y) = e.GetPosition(AssociatedObject);
            drawingNode.ConnectorMove(x, y);
        }
    }
}
