namespace Avalonia.NodeEditor.Core;

public interface IDrawingNodeFactory
{
    IPin CreatePin();
    IConnector CreateConnector();
    public IList<T> CreateList<T>();
}
