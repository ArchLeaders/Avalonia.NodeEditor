using Avalonia.Controls;
using Avalonia.NodeEditor.Core;
using System;
using System.Collections.Generic;

namespace Avalonia.NodeEditor;

internal static class HitTestHelper
{
    public static double Length(Point pt0, Point pt1)
    {
        return Math.Sqrt(Math.Pow(pt1.X - pt0.X, 2) + Math.Pow(pt1.Y - pt0.Y, 2));
    }

    public static Point[] FlattenCubic(Point pt0, Point pt1, Point pt2, Point pt3)
    {
        int count = (int)Math.Max(1, Length(pt0, pt1) + Length(pt1, pt2) + Length(pt2, pt3));
        Point[] points = new Point[count];

        for (int i = 0; i < count; i++) {
            double t = (i + 1d) / count;
            double x = (1 - t) * (1 - t) * (1 - t) * pt0.X +
                    3 * t * (1 - t) * (1 - t) * pt1.X +
                    3 * t * t * (1 - t) * pt2.X +
                    t * t * t * pt3.X;
            double y = (1 - t) * (1 - t) * (1 - t) * pt0.Y +
                    3 * t * (1 - t) * (1 - t) * pt1.Y +
                    3 * t * t * (1 - t) * pt2.Y +
                    t * t * t * pt3.Y;

            points[i] = new Point(x, y);
        }

        return points;
    }

    public static bool HitTestConnector(IConnector connector, Rect rect)
    {
        IPin? start = connector.Start;
        IPin? end = connector.End;
        if (start is null || end is null) {
            return false;
        }

        double p0X = start.X;
        double p0Y = start.Y;
        if (start.Parent is not null) {
            p0X += start.Parent.X;
            p0Y += start.Parent.Y;
        }

        double p3X = end.X;
        double p3Y = end.Y;
        if (end.Parent is not null) {
            p3X += end.Parent.X;
            p3Y += end.Parent.Y;
        }

        double p1X = p0X;
        double p1Y = p0Y;

        double p2X = p3X;
        double p2Y = p3Y;

        connector.GetControlPoints(
            connector.Orientation,
            connector.Offset,
            start.Alignment,
            end.Alignment,
            ref p1X, ref p1Y,
            ref p2X, ref p2Y);

        Point pt0 = new(p0X, p0Y);
        Point pt1 = new(p1X, p1Y);
        Point pt2 = new(p2X, p2Y);
        Point pt3 = new(p3X, p3Y);

        Point[] points = FlattenCubic(pt0, pt1, pt2, pt3);

        for (int i = 0; i < points.Length; i++) {
            if (rect.Contains(points[i])) {
                return true;
            }
        }

        return false;
    }

    public static Rect GetConnectorBounds(IConnector connector)
    {
        IPin? start = connector.Start;
        IPin? end = connector.End;
        if (start is null || end is null) {
            return default;
        }

        double p0X = start.X;
        double p0Y = start.Y;
        if (start.Parent is { }) {
            p0X += start.Parent.X;
            p0Y += start.Parent.Y;
        }

        double p3X = end.X;
        double p3Y = end.Y;
        if (end.Parent is { }) {
            p3X += end.Parent.X;
            p3Y += end.Parent.Y;
        }

        double p1X = p0X;
        double p1Y = p0Y;

        double p2X = p3X;
        double p2Y = p3Y;

        connector.GetControlPoints(
            connector.Orientation,
            connector.Offset,
            start.Alignment,
            end.Alignment,
            ref p1X, ref p1Y,
            ref p2X, ref p2Y);

        Point pt0 = new(p0X, p0Y);
        Point pt1 = new(p1X, p1Y);
        Point pt2 = new(p2X, p2Y);
        Point pt3 = new(p3X, p3Y);

        Point[] points = FlattenCubic(pt0, pt1, pt2, pt3);

        double topLeftX = 0.0;
        double topLeftY = 0.0;
        double bottomRightX = 0.0;
        double bottomRightY = 0.0;

        for (int i = 0; i < points.Length; i++) {
            if (i == 0) {
                topLeftX = points[i].X;
                topLeftY = points[i].Y;
                bottomRightX = points[i].X;
                bottomRightY = points[i].Y;
            }
            else {
                topLeftX = Math.Min(topLeftX, points[i].X);
                topLeftY = Math.Min(topLeftY, points[i].Y);
                bottomRightX = Math.Max(bottomRightX, points[i].X);
                bottomRightY = Math.Max(bottomRightY, points[i].Y);
            }
        }

        return new Rect(
            new Point(topLeftX, topLeftY),
            new Point(bottomRightX, bottomRightY));
    }

    public static void FindSelectedNodes(ItemsControl? itemsControl, Rect rect)
    {
        if (itemsControl?.DataContext is not IDrawingNode drawingNode) {
            return;
        }

        drawingNode.NotifyDeselectedNodes();
        drawingNode.NotifyDeselectedConnectors();
        drawingNode.SetSelectedNodes(null);
        drawingNode.SetSelectedConnectors(null);
        drawingNode.NotifySelectionChanged();

        HashSet<INode> selectedNodes = [];
        HashSet<IConnector> selectedConnectors = [];

        if (drawingNode.CanSelectNodes()) {
            foreach (Control control in itemsControl.GetRealizedContainers()) {
                if (control is not { DataContext: INode node } containerControl) {
                    continue;
                }

                Rect bounds = containerControl.Bounds;

                if (!rect.Intersects(bounds)) {
                    continue;
                }

                if (node.CanSelect()) {
                    selectedNodes.Add(node);
                }
            }
        }

        if (drawingNode.CanSelectConnectors()) {
            if (drawingNode.Connectors is { Count: > 0 }) {
                foreach (IConnector connector in drawingNode.Connectors) {
                    if (!HitTestConnector(connector, rect)) {
                        continue;
                    }

                    if (connector.CanSelect()) {
                        selectedConnectors.Add(connector);
                    }
                }
            }
        }

        bool notify = false;

        if (selectedConnectors.Count > 0) {
            drawingNode.SetSelectedConnectors(selectedConnectors);
            notify = true;
        }

        if (selectedNodes.Count > 0) {
            drawingNode.SetSelectedNodes(selectedNodes);
            notify = true;
        }

        if (notify) {
            drawingNode.NotifySelectionChanged();
        }
    }

    public static Rect CalculateSelectedRect(ItemsControl? itemsControl)
    {
        if (itemsControl?.DataContext is not IDrawingNode drawingNode) {
            return default;
        }

        Rect selectedRect = new();

        itemsControl.UpdateLayout();

        ISet<INode>? selectedNodes = drawingNode.GetSelectedNodes();
        ISet<IConnector>? selectedConnectors = drawingNode.GetSelectedConnectors();

        if (selectedNodes is { Count: > 0 } && drawingNode.Nodes is { Count: > 0 }) {
            foreach (INode node in selectedNodes) {
                int index = drawingNode.Nodes.IndexOf(node);
                Control? selectedControl = itemsControl.ContainerFromIndex(index);
                Rect bounds = selectedControl?.Bounds ?? default;
                selectedRect = selectedRect == default ? bounds : selectedRect.Union(bounds);
            }
        }

        if (selectedConnectors is { Count: > 0 } && drawingNode.Connectors is { Count: > 0 }) {
            foreach (IConnector connector in selectedConnectors) {
                Rect bounds = GetConnectorBounds(connector);
                selectedRect = selectedRect == default ? bounds : selectedRect.Union(bounds);
            }
        }

        return selectedRect;
    }
}
