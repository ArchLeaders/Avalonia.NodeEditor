using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Immutable;

namespace Avalonia.NodeEditor.Controls;

public class SelectedAdorner : Control
{
    public static readonly StyledProperty<Rect> RectProperty =
        AvaloniaProperty.Register<SelectedAdorner, Rect>(nameof(Rect));

    public Rect Rect {
        get => GetValue(RectProperty);
        set => SetValue(RectProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == RectProperty) {
            InvalidateVisual();
        }
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        // TODO: Pull SystemAccentColor

        double thickness = 2.0;
        ImmutablePen pen = new(
            new ImmutableSolidColorBrush(new Color(0xFF, 0x17, 0x9D, 0xE3)),
            thickness);
        Rect bounds = Rect;
        Rect rect = bounds.Deflate(thickness * 0.5);
        context.DrawRectangle(null, pen, rect);
    }
}
