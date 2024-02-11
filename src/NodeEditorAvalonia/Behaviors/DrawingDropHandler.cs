using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using NodeEditor.Controls;
using NodeEditor.Model;

namespace NodeEditor.Behaviors;

public class DrawingDropHandler : DefaultDropHandler
{
    public static readonly StyledProperty<Control?> RelativeToProperty =
        AvaloniaProperty.Register<DrawingDropHandler, Control?>(nameof(RelativeTo));

    public Control? RelativeTo {
        get => GetValue(RelativeToProperty);
        set => SetValue(RelativeToProperty, value);
    }

    private bool Validate(IDrawingNode drawing, object? sender, DragEventArgs e, bool isExecuting)
    {
        Control? relativeTo = RelativeTo ?? sender as Control;
        if (relativeTo is null) {
            return false;
        }

        Point point = e.GetPosition(relativeTo);

        if (relativeTo is DrawingNode drawingNode) {
            point = SnapHelper.Snap(point, drawingNode.SnapX, drawingNode.SnapY, drawingNode.EnableSnap);
        }

        if (e.Data.Contains(DataFormats.Text)) {
            // TODO: text
            // 
            // if (isExecuting && e.Data.GetText() is string text) {
            // 
            // }

            return true;
        }

        foreach (string format in e.Data.GetDataFormats()) {
            object? data = e.Data.Get(format);

            switch (data) {
                case INodeTemplate template: {
                    if (isExecuting) {
                        INode? node = drawing.Clone(template.Template);
                        if (node is { }) {
                            node.Parent = drawing;
                            node.Move(point.X, point.Y);
                            drawing.Nodes?.Add(node);
                            node.OnCreated();
                        }
                    }
                    return true;
                }
            }
        }

        if (e.Data.Contains(DataFormats.Files)) {
            // TODO: files, point.X, point.Y
            // 
            // var files = e.Data.GetFiles()?.ToArray();
            // if (isExecuting) {
            // 
            // }

            return true;
        }

        return false;
    }

    public override bool Validate(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
    {
        if (targetContext is IDrawingNode drawing) {
            return Validate(drawing, sender, e, false);
        }

        return false;
    }

    public override bool Execute(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
    {
        if (targetContext is IDrawingNode drawing) {
            return Validate(drawing, sender, e, true);
        }

        return false;
    }
}
