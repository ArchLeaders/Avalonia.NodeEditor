using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using System;

namespace Avalonia.NodeEditor.Controls;

public class SelectionAdorner : Control
{
    public static readonly StyledProperty<Point> TopLeftProperty =
        AvaloniaProperty.Register<SelectionAdorner, Point>(nameof(TopLeft));

    public static readonly StyledProperty<Point> BottomRightProperty =
        AvaloniaProperty.Register<SelectionAdorner, Point>(nameof(BottomRight));

    public Point TopLeft {
        get => GetValue(TopLeftProperty);
        set => SetValue(TopLeftProperty, value);
    }

    public Point BottomRight {
        get => GetValue(BottomRightProperty);
        set => SetValue(BottomRightProperty, value);
    }

    public Rect GetRect()
    {
        double topLeftX = Math.Min(TopLeft.X, BottomRight.X);
        double topLeftY = Math.Min(TopLeft.Y, BottomRight.Y);
        double bottomRightX = Math.Max(TopLeft.X, BottomRight.X);
        double bottomRightY = Math.Max(TopLeft.Y, BottomRight.Y);
        return new Rect(
            new Point(topLeftX, topLeftY),
            new Point(bottomRightX, bottomRightY));
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == TopLeftProperty || change.Property == BottomRightProperty) {
            InvalidateVisual();
        }
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        ImmutableSolidColorBrush brush = new(new Color(0xFF, 0x00, 0x00, 0xFF), 0.3);
        double thickness = 2.0;
        ImmutablePen pen = new(
            new ImmutableSolidColorBrush(new Color(0xFF, 0x00, 0x00, 0xFF)),
            thickness);

        Rect bounds = GetRect();
        Rect rect = bounds.Deflate(thickness * 0.5);
        context.DrawRectangle(brush, pen, rect);
    }
}
