#pragma warning disable IDE0028 // Simplify collection initialization

using Avalonia.NodeEditor.Core;
using Avalonia.NodeEditor.Core.Mvvm.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Avalonia.NodeEditor.Mvvm;

public partial class ObservableDrawingNode : ObservableNode, IDrawingNode
{
    protected readonly DrawingNodeEditor _editor;
    protected ISet<INode>? _selectedNodes;
    protected ISet<IConnector>? _selectedConnectors;
    protected INodeSerializer? _serializer;

    public event SelectionChangedEventHandler? SelectionChanged;

    [ObservableProperty]
    private IList<INode>? _nodes = new ObservableCollection<INode>();

    [ObservableProperty]
    private IList<IConnector>? _connectors = new ObservableCollection<IConnector>();

    [ObservableProperty]
    private bool _enableMultiplePinConnections;

    [ObservableProperty]
    private bool _enableSnap;

    [ObservableProperty]
    private double _snapX;

    [ObservableProperty]
    private double _snapY;

    [ObservableProperty]
    private bool _enableGrid;

    [ObservableProperty]
    private double _gridCellWidth;

    [ObservableProperty]
    private double _gridCellHeight;

    ICommand IDrawingNode.CutNodesCommand => CutNodesCommand;
    ICommand IDrawingNode.CopyNodesCommand => CopyNodesCommand;
    ICommand IDrawingNode.PasteNodesCommand => PasteNodesCommand;
    ICommand IDrawingNode.DuplicateNodesCommand => DuplicateNodesCommand;
    ICommand IDrawingNode.DeleteNodesCommand => DeleteNodesCommand;
    ICommand IDrawingNode.SelectAllNodesCommand => SelectAllNodesCommand;
    ICommand IDrawingNode.DeselectAllNodesCommand => DeselectAllNodesCommand;

    public ObservableDrawingNode()
    {
        _editor = new DrawingNodeEditor(this, DrawingNodeFactory.Shared);
    }

    public void NotifySelectionChanged()
    {
        SelectionChanged?.Invoke(this, EventArgs.Empty);
    }

    public void NotifyDeselectedNodes()
    {
        ISet<INode>? selectedNodes = GetSelectedNodes();
        if (selectedNodes is { }) {
            foreach (INode selectedNode in selectedNodes) {
                selectedNode.OnDeselected();
            }
        }
    }

    public void NotifyDeselectedConnectors()
    {
        ISet<IConnector>? selectedConnectors = GetSelectedConnectors();
        if (selectedConnectors is { }) {
            foreach (IConnector selectedConnector in selectedConnectors) {
                selectedConnector.OnDeselected();
            }
        }
    }

    public ISet<INode>? GetSelectedNodes()
    {
        return _selectedNodes;
    }

    public void SetSelectedNodes(ISet<INode>? nodes)
    {
        _selectedNodes = nodes;
    }

    public ISet<IConnector>? GetSelectedConnectors()
    {
        return _selectedConnectors;
    }

    public void SetSelectedConnectors(ISet<IConnector>? connectors)
    {
        _selectedConnectors = connectors;
    }

    public INodeSerializer? GetSerializer()
    {
        return _serializer;
    }

    public void SetSerializer(INodeSerializer? serializer)
    {
        _serializer = serializer;
    }

    public bool IsPinConnected(IPin pin)
    {
        return _editor.IsPinConnected(pin);
    }

    public bool IsConnectorMoving()
    {
        return _editor.IsConnectorMoving();
    }

    public void CancelConnector()
    {
        _editor.CancelConnector();
    }

    public virtual bool CanSelectNodes()
    {
        return _editor.CanSelectNodes();
    }

    public virtual bool CanSelectConnectors()
    {
        return _editor.CanSelectConnectors();
    }

    public virtual bool CanConnectPin(IPin pin)
    {
        return EnableMultiplePinConnections || _editor.IsPinConnected(pin) == false;
    }

    public virtual void DrawingLeftPressed(double x, double y)
    {
        _editor.DrawingLeftPressed();
    }

    public virtual void DrawingRightPressed(double x, double y)
    {
        _editor.DrawingRightPressed(x, y);
    }

    public virtual void ConnectorLeftPressed(IPin pin, bool showWhenMoving)
    {
        _editor.ConnectorLeftPressed(pin, showWhenMoving);
    }

    public virtual void ConnectorMove(double x, double y)
    {
        _editor.ConnectorMove(x, y);
    }

    [RelayCommand]
    public virtual void CutNodes()
    {
        _editor.CutNodes();
    }

    [RelayCommand]
    public virtual void CopyNodes()
    {
        _editor.CopyNodes();
    }

    [RelayCommand]
    public virtual void PasteNodes()
    {
        _editor.PasteNodes();
    }

    [RelayCommand]
    public virtual void DuplicateNodes()
    {
        _editor.DuplicateNodes();
    }

    [RelayCommand]
    public virtual void DeleteNodes()
    {
        _editor.DeleteNodes();
    }

    [RelayCommand]
    public virtual void SelectAllNodes()
    {
        _editor.SelectAllNodes();
    }

    [RelayCommand]
    public virtual void DeselectAllNodes()
    {
        _editor.DeselectAllNodes();
    }
}
