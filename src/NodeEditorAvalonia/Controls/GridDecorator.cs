using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Immutable;

namespace NodeEditor.Controls;

public class GridDecorator : Decorator
{
    public static readonly StyledProperty<bool> EnableGridProperty =
        AvaloniaProperty.Register<GridDecorator, bool>(nameof(EnableGrid));

    public static readonly StyledProperty<double> GridCellWidthProperty =
        AvaloniaProperty.Register<GridDecorator, double>(nameof(GridCellWidth));

    public static readonly StyledProperty<double> GridCellHeightProperty =
        AvaloniaProperty.Register<GridDecorator, double>(nameof(GridCellHeight));

    public bool EnableGrid {
        get => GetValue(EnableGridProperty);
        set => SetValue(EnableGridProperty, value);
    }

    public double GridCellWidth {
        get => GetValue(GridCellWidthProperty);
        set => SetValue(GridCellWidthProperty, value);
    }

    public double GridCellHeight {
        get => GetValue(GridCellHeightProperty);
        set => SetValue(GridCellHeightProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == EnableGridProperty
            || change.Property == GridCellWidthProperty
            || change.Property == GridCellHeightProperty) {
            InvalidateVisual();
        }
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        if (!EnableGrid) {
            return;
        }

        double cw = GridCellWidth;
        double ch = GridCellHeight;
        if (cw <= 0.0 || ch <= 0.0) {
            return;
        }

        Rect rect = Bounds;
        double thickness = 1.0;

        ImmutableSolidColorBrush brush = new(Color.FromArgb(255, 222, 222, 222));
        ImmutablePen pen = new(brush, thickness);

        using DrawingContext.PushedState _ = context.PushTransform(Matrix.CreateTranslation(-0.5d, -0.5d));

        double ox = rect.X;
        double ex = rect.X + rect.Width;
        double oy = rect.Y;
        double ey = rect.Y + rect.Height;

        for (double x = ox + cw; x < ex; x += cw) {
            Point p0 = new(x + 0.5, oy + 0.5);
            Point p1 = new(x + 0.5, ey + 0.5);
            context.DrawLine(pen, p0, p1);
        }

        for (double y = oy + ch; y < ey; y += ch) {
            Point p0 = new(ox + 0.5, y + 0.5);
            Point p1 = new(ex + 0.5, y + 0.5);
            context.DrawLine(pen, p0, p1);
        }

        context.DrawRectangle(null, pen, rect);
    }
}
