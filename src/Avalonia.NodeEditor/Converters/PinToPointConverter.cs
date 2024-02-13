using Avalonia.Data.Converters;
using Avalonia.NodeEditor.Core;
using System;
using System.Globalization;

namespace Avalonia.NodeEditor.Converters;

public class PinToPointConverter : IValueConverter
{
    public static PinToPointConverter Instance { get; } = new();

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is IPin pin) {
            double x = pin.X;
            double y = pin.Y;

            if (pin.Parent is { }) {
                x += pin.Parent.X;
                y += pin.Parent.Y;
            }

            return new Point(x, y);
        }

        return new Point();
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
