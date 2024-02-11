using Avalonia;

namespace NodeEditor;

internal static class SnapHelper
{
    public static double Snap(double value, double snap)
    {
        if (snap == 0.0) {
            return value;
        }
        double c = value % snap;
        double r = c >= snap / 2.0 ? value + snap - c : value - c;
        return r;
    }

    public static Point Snap(Point point, double snapX, double snapY, bool enabled)
    {
        if (enabled) {
            double pointX = Snap(point.X, snapX);
            double pointY = Snap(point.Y, snapY);
            return new Point(pointX, pointY);
        }

        return point;
    }
}
