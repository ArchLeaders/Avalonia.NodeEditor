﻿using Avalonia.Controls.Presenters;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.NodeEditor.Core;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.NodeEditor.Behaviors;

public class PinPressedBehavior : Behavior<ContentPresenter>
{
    protected override void OnAttached()
    {
        base.OnAttached();

        if (AssociatedObject is { }) {
            AssociatedObject.AddHandler(InputElement.PointerPressedEvent, Pressed, RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
        }
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();

        if (AssociatedObject is { }) {
            AssociatedObject.RemoveHandler(InputElement.PointerPressedEvent, Pressed);
        }
    }

    private void Pressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.Handled) {
            return;
        }

        if (AssociatedObject?.DataContext is not IPin pin) {
            return;
        }

        if (pin.Parent is not { } nodeViewModel) {
            return;
        }

        if (nodeViewModel.Parent is IDrawingNode drawingNode) {
            PointerPoint info = e.GetCurrentPoint(AssociatedObject);

            if (info.Properties.IsLeftButtonPressed) {
                bool showWhenMoving = info.Pointer.Type == PointerType.Mouse;
                drawingNode.ConnectorLeftPressed(pin, showWhenMoving);
                e.Handled = true;
            }
        }
    }
}
