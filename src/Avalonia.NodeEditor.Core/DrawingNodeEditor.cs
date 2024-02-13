namespace Avalonia.NodeEditor.Core;

public sealed class DrawingNodeEditor(IDrawingNode node, IDrawingNodeFactory factory)
{
    private readonly IDrawingNode _node = node;
    private readonly IDrawingNodeFactory _factory = factory;
    private IConnector? _connector;
    private string? _clipboard;
    private double _pressedX = double.NaN;
    private double _pressedY = double.NaN;

    private class Clipboard
    {
        public ISet<INode>? SelectedNodes { get; set; }
        public ISet<IConnector>? SelectedConnectors { get; set; }
    }

    public bool IsPinConnected(IPin pin)
    {
        if (_node.Connectors is { }) {
            foreach (IConnector connector in _node.Connectors) {
                if (connector.Start == pin || connector.End == pin) {
                    return true;
                }
            }
        }

        return false;
    }

    public bool IsConnectorMoving()
    {
        return _connector is not null;
    }

    public void CancelConnector()
    {
        if (_connector is not null) {
            _node.Connectors?.Remove(_connector);
            _connector = null;
        }
    }

    public bool CanSelectNodes()
    {
        return _connector is null;
    }

    public bool CanSelectConnectors()
    {
        return _connector is null;
    }

    public bool CanConnectPin(IPin pin)
    {
        return _node.CanConnectPin(pin);
    }

    private static void NotifyPinsRemoved(INode node)
    {
        if (node.Pins is { }) {
            foreach (IPin pin in node.Pins) {
                pin.OnRemoved();
            }
        }
    }

    public void DrawingLeftPressed()
    {
        if (IsConnectorMoving()) {
            CancelConnector();
        }
    }

    public void DrawingRightPressed(double x, double y)
    {
        _pressedX = x;
        _pressedY = y;

        if (IsConnectorMoving()) {
            CancelConnector();
        }
    }

    public void ConnectorLeftPressed(IPin pin, bool showWhenMoving)
    {
        if (_node.Connectors is null) {
            return;
        }

        if (!CanConnectPin(pin) || !pin.CanConnect()) {
            return;
        }

        if (_connector is null) {
            double x = pin.X;
            double y = pin.Y;

            if (pin.Parent is { }) {
                x += pin.Parent.X;
                y += pin.Parent.Y;
            }

            IPin end = _factory.CreatePin();
            end.Parent = null;
            end.X = x;
            end.Y = y;
            end.Width = pin.Width;
            end.Height = pin.Height;
            end.OnCreated();

            IConnector connector = _factory.CreateConnector();
            connector.Parent = _node;
            connector.Start = pin;
            connector.End = end;
            pin.OnConnected();
            end.OnConnected();
            connector.OnCreated();

            if (showWhenMoving) {
                _node.Connectors ??= _factory.CreateList<IConnector>();
                _node.Connectors.Add(connector);
            }

            _connector = connector;
        }
        else {
            if (_connector.Start != pin) {
                IPin? end = _connector.End;
                _connector.End = pin;
                end?.OnDisconnected();
                pin.OnConnected();

                if (!showWhenMoving) {
                    _node.Connectors ??= _factory.CreateList<IConnector>();
                    _node.Connectors.Add(_connector);
                }

                _connector = null;
            }
        }
    }

    public void ConnectorMove(double x, double y)
    {
        if (_connector is { End: { } }) {
            _connector.End.X = x;
            _connector.End.Y = y;
            _connector.End.OnMoved();
        }
    }

    public void CutNodes()
    {
        INodeSerializer? serializer = _node.GetSerializer();
        if (serializer is null) {
            return;
        }

        ISet<INode>? selectedNodes = _node.GetSelectedNodes();
        ISet<IConnector>? selectedConnectors = _node.GetSelectedConnectors();

        if (selectedNodes is not { Count: > 0 } && selectedConnectors is not { Count: > 0 }) {
            return;
        }

        Clipboard clipboard = new() {
            SelectedNodes = selectedNodes,
            SelectedConnectors = selectedConnectors
        };

        _clipboard = serializer.Serialize(clipboard);

        if (clipboard.SelectedNodes is { }) {
            foreach (INode node in clipboard.SelectedNodes) {
                if (node.CanRemove()) {
                    _node.Nodes?.Remove(node);
                    node.OnRemoved();
                    NotifyPinsRemoved(node);
                }
            }
        }

        if (clipboard.SelectedConnectors is { }) {
            foreach (IConnector connector in clipboard.SelectedConnectors) {
                if (connector.CanRemove()) {
                    _node.Connectors?.Remove(connector);
                    connector.OnRemoved();
                }
            }
        }

        _node.NotifyDeselectedNodes();
        _node.NotifyDeselectedConnectors();

        _node.SetSelectedNodes(null);
        _node.SetSelectedConnectors(null);
        _node.NotifySelectionChanged();
    }

    public void CopyNodes()
    {
        INodeSerializer? serializer = _node.GetSerializer();
        if (serializer is null) {
            return;
        }

        ISet<INode>? selectedNodes = _node.GetSelectedNodes();
        ISet<IConnector>? selectedConnectors = _node.GetSelectedConnectors();

        if (selectedNodes is not { Count: > 0 } && selectedConnectors is not { Count: > 0 }) {
            return;
        }

        Clipboard clipboard = new() {
            SelectedNodes = selectedNodes,
            SelectedConnectors = selectedConnectors
        };

        _clipboard = serializer.Serialize(clipboard);
    }

    public void PasteNodes()
    {
        INodeSerializer? serializer = _node.GetSerializer();
        if (serializer is null) {
            return;
        }

        if (_clipboard is null) {
            return;
        }

        double pressedX = _pressedX;
        double pressedY = _pressedY;

        if (serializer.Deserialize<Clipboard?>(_clipboard) is not Clipboard clipboard) {
            return;
        }

        _node.NotifyDeselectedNodes();
        _node.NotifyDeselectedConnectors();

        _node.SetSelectedNodes(null);
        _node.SetSelectedConnectors(null);

        HashSet<INode> selectedNodes = [];
        HashSet<IConnector> selectedConnectors = [];

        if (clipboard.SelectedNodes is { Count: > 0 }) {
            double minX = 0.0;
            double minY = 0.0;
            int i = 0;

            foreach (INode node in clipboard.SelectedNodes) {
                minX = i == 0 ? node.X : Math.Min(minX, node.X);
                minY = i == 0 ? node.Y : Math.Min(minY, node.Y);
                i++;
            }

            double deltaX = double.IsNaN(pressedX) ? 0.0 : pressedX - minX;
            double deltaY = double.IsNaN(pressedY) ? 0.0 : pressedY - minY;

            foreach (INode node in clipboard.SelectedNodes) {
                if (node.CanMove()) {
                    node.Move(deltaX, deltaY);
                }

                node.Parent = _node;

                _node.Nodes?.Add(node);
                node.OnCreated();

                if (node.CanSelect()) {
                    selectedNodes.Add(node);
                    node.OnSelected();
                }
            }
        }

        if (clipboard.SelectedConnectors is { Count: > 0 }) {
            foreach (IConnector connector in clipboard.SelectedConnectors) {
                connector.Parent = _node;

                _node.Connectors?.Add(connector);
                connector.OnCreated();

                if (connector.CanSelect()) {
                    selectedConnectors.Add(connector);
                    connector.OnSelected();
                }
            }
        }

        _node.NotifyDeselectedNodes();

        if (selectedNodes.Count > 0) {
            _node.SetSelectedNodes(selectedNodes);
        }
        else {
            _node.SetSelectedNodes(null);
        }

        _node.NotifyDeselectedConnectors();

        if (selectedConnectors.Count > 0) {
            _node.SetSelectedConnectors(selectedConnectors);
        }
        else {
            _node.SetSelectedConnectors(null);
        }

        _node.NotifySelectionChanged();

        _pressedX = double.NaN;
        _pressedY = double.NaN;
    }

    public void DuplicateNodes()
    {
        _pressedX = double.NaN;
        _pressedY = double.NaN;

        CopyNodes();
        PasteNodes();
    }

    public void DeleteNodes()
    {
        ISet<INode>? selectedNodes = _node.GetSelectedNodes();
        ISet<IConnector>? selectedConnectors = _node.GetSelectedConnectors();
        bool notify = false;

        if (selectedNodes is { Count: > 0 }) {
            foreach (INode node in selectedNodes) {
                if (node.CanRemove()) {
                    _node.Nodes?.Remove(node);
                    node.OnRemoved();
                    NotifyPinsRemoved(node);
                }
            }

            _node.NotifyDeselectedNodes();

            _node.SetSelectedNodes(null);
            notify = true;
        }

        if (selectedConnectors is { Count: > 0 }) {
            foreach (IConnector connector in selectedConnectors) {
                if (connector.CanRemove()) {
                    _node.Connectors?.Remove(connector);
                    connector.OnRemoved();
                }
            }

            _node.NotifyDeselectedConnectors();

            _node.SetSelectedConnectors(null);
            notify = true;
        }

        if (notify) {
            _node.NotifySelectionChanged();
        }
    }

    public void SelectAllNodes()
    {
        bool notify = false;

        if (_node.Nodes is not null) {
            _node.NotifyDeselectedNodes();

            _node.SetSelectedNodes(null);

            HashSet<INode> selectedNodes = [];
            IList<INode> nodes = _node.Nodes;

            foreach (INode node in nodes) {
                if (node.CanSelect()) {
                    selectedNodes.Add(node);
                    node.OnSelected();
                }
            }

            if (selectedNodes.Count > 0) {
                _node.SetSelectedNodes(selectedNodes);
                notify = true;
            }
        }

        if (_node.Connectors is not null) {
            _node.NotifyDeselectedConnectors();

            _node.SetSelectedConnectors(null);

            HashSet<IConnector> selectedConnectors = [];
            IList<IConnector> connectors = _node.Connectors;

            foreach (IConnector connector in connectors) {
                if (connector.CanSelect()) {
                    selectedConnectors.Add(connector);
                    connector.OnSelected();
                }
            }

            if (selectedConnectors.Count > 0) {
                _node.SetSelectedConnectors(selectedConnectors);
                notify = true;
            }
        }

        if (notify) {
            _node.NotifySelectionChanged();
        }
    }

    public void DeselectAllNodes()
    {
        _node.NotifyDeselectedNodes();
        _node.NotifyDeselectedConnectors();

        _node.SetSelectedNodes(null);
        _node.SetSelectedConnectors(null);
        _node.NotifySelectionChanged();

        if (IsConnectorMoving()) {
            CancelConnector();
        }
    }
}
