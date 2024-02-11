using NodeEditor.Model;
using NodeEditor.Mvvm;
using System.Collections.ObjectModel;

namespace NodeEditorDemo.Services;

internal static class Demo
{
    public static IDrawingNode CreateDemoDrawing()
    {
        DrawingNodeViewModel drawing = new() {
            X = 0,
            Y = 0,
            Width = 900,
            Height = 600,
            Nodes = new ObservableCollection<INode>(),
            Connectors = new ObservableCollection<IConnector>(),
            EnableMultiplePinConnections = false,
            EnableSnap = true,
            SnapX = 15.0,
            SnapY = 15.0,
            EnableGrid = true,
            GridCellWidth = 15.0,
            GridCellHeight = 15.0,
        };

        INode rectangle0 = NodeFactory.CreateRectangle(30, 30, 60, 60, "rect0");
        rectangle0.Parent = drawing;
        drawing.Nodes.Add(rectangle0);

        INode rectangle1 = NodeFactory.CreateRectangle(240, 30, 60, 60, "rect1");
        rectangle1.Parent = drawing;
        drawing.Nodes.Add(rectangle1);

        if (rectangle0.Pins?[1] is { } && rectangle1.Pins?[0] is { }) {
            IConnector connector0 = NodeFactory.CreateConnector(rectangle0.Pins[1], rectangle1.Pins[0]);
            connector0.Parent = drawing;
            drawing.Connectors.Add(connector0);
        }

        INode rectangle2 = NodeFactory.CreateRectangle(30, 150, 60, 60, "rect2");
        rectangle2.Parent = drawing;
        drawing.Nodes.Add(rectangle2);

        INode ellipse0 = NodeFactory.CreateEllipse(240, 150, 60, 60, "ellipse0");
        ellipse0.Parent = drawing;
        drawing.Nodes.Add(ellipse0);

        INode signal0 = NodeFactory.CreateSignal(x: 30, y: 270, label: "in0", state: true);
        signal0.Parent = drawing;
        drawing.Nodes.Add(signal0);

        INode signal1 = NodeFactory.CreateSignal(x: 30, y: 390, label: "in1", state: false);
        signal1.Parent = drawing;
        drawing.Nodes.Add(signal1);

        INode signal2 = NodeFactory.CreateSignal(x: 420, y: 375, label: "out0", state: true);
        signal2.Parent = drawing;
        drawing.Nodes.Add(signal2);

        INode orGate0 = NodeFactory.CreateOrGate(300, 360);
        orGate0.Parent = drawing;
        drawing.Nodes.Add(orGate0);

        if (signal0.Pins?[1] is { } && orGate0.Pins?[2] is { }) {
            IConnector connector0 = NodeFactory.CreateConnector(signal0.Pins[1], orGate0.Pins[2]);
            connector0.Parent = drawing;
            drawing.Connectors.Add(connector0);
        }

        if (signal1.Pins?[1] is { } && orGate0.Pins?[0] is { }) {
            IConnector connector0 = NodeFactory.CreateConnector(signal1.Pins[1], orGate0.Pins[0]);
            connector0.Parent = drawing;
            drawing.Connectors.Add(connector0);
        }

        if (orGate0.Pins?[1] is { } && signal2.Pins?[0] is { }) {
            IConnector connector1 = NodeFactory.CreateConnector(orGate0.Pins[1], signal2.Pins[0]);
            connector1.Parent = drawing;
            drawing.Connectors.Add(connector1);
        }

        return drawing;
    }
}
