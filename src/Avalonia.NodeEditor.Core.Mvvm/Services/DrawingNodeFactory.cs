using Avalonia.NodeEditor.Mvvm;
using System.Collections.ObjectModel;

namespace Avalonia.NodeEditor.Core.Mvvm.Services;

public class DrawingNodeFactory : IDrawingNodeFactory
{
    private static readonly Lazy<IDrawingNodeFactory> _shared = new(() => new DrawingNodeFactory());
    public static IDrawingNodeFactory Shared => _shared.Value;

    public IPin CreatePin()
    {
        return new ObservablePin();
    }

    public IConnector CreateConnector()
    {
        return new ObservableConnector();
    }

    public IList<T> CreateList<T>()
    {
        return new ObservableCollection<T>();
    }
}
