using Avalonia.NodeEditor.Core;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Avalonia.NodeEditor.Mvvm;

public partial class ObservableConnector : ObservableObject, IConnector
{
    [ObservableProperty]
    private string? _name;

    [ObservableProperty]
    private IDrawingNode? _parent;

    [ObservableProperty]
    private ConnectorOrientation _orientation;

    [ObservableProperty]
    private IPin? _start;

    [ObservableProperty]
    private IPin? _end;

    [ObservableProperty]
    private double _offset = 50;

    public event EventHandler<ConnectorCreatedEventArgs>? Created;

    public event EventHandler<ConnectorRemovedEventArgs>? Removed;

    public event EventHandler<ConnectorSelectedEventArgs>? Selected;

    public event EventHandler<ConnectorDeselectedEventArgs>? Deselected;

    public event EventHandler<ConnectorStartChangedEventArgs>? StartChanged;

    public event EventHandler<ConnectorEndChangedEventArgs>? EndChanged;

    public virtual bool CanSelect()
    {
        return true;
    }

    public virtual bool CanRemove()
    {
        return true;
    }

    public void OnCreated()
    {
        Created?.Invoke(this, new ConnectorCreatedEventArgs(this));
    }

    public void OnRemoved()
    {
        Removed?.Invoke(this, new ConnectorRemovedEventArgs(this));
    }

    public void OnSelected()
    {
        Selected?.Invoke(this, new ConnectorSelectedEventArgs(this));
    }

    public void OnDeselected()
    {
        Deselected?.Invoke(this, new ConnectorDeselectedEventArgs(this));
    }

    public void OnStartChanged()
    {
        StartChanged?.Invoke(this, new ConnectorStartChangedEventArgs(this));
    }

    public void OnEndChanged()
    {
        EndChanged?.Invoke(this, new ConnectorEndChangedEventArgs(this));
    }

    partial void OnStartChanged(IPin? value)
    {
        if (value?.Parent is ObservableNode node) {
            node.PropertyChanged += (s, e) => {
                if (e.PropertyName is nameof(INode.X) or nameof(INode.Y)) {
                    OnPropertyChanged(nameof(Start));
                }
            };
        }
        else if (value is ObservablePin pin1) {
            pin1.PropertyChanged += (s, e) => {
                if (e.PropertyName is nameof(IPin.X) or nameof(IPin.Y)) {
                    OnPropertyChanged(nameof(Start));
                }
            };
        }

        if (value is ObservablePin pin) {
            pin.PropertyChanged += (s, e) => {
                if (e.PropertyName is nameof(IPin.Alignment)) {
                    OnPropertyChanged(nameof(Start));
                }
            };
        }
    }

    partial void OnEndChanged(IPin? value)
    {
        if (value?.Parent is ObservableNode node) {
            node.PropertyChanged += (s, e) => {
                if (e.PropertyName is nameof(INode.X) or nameof(INode.Y)) {
                    OnPropertyChanged(nameof(End));
                }
            };
        }
        else if (value is ObservablePin pin1) {
            pin1.PropertyChanged += (s, e) => {
                if (e.PropertyName is nameof(IPin.X) or nameof(IPin.Y)) {
                    OnPropertyChanged(nameof(End));
                }
            };
        }

        if (value is ObservablePin pin) {
            pin.PropertyChanged += (s, e) => {
                if (e.PropertyName is nameof(IPin.Alignment)) {
                    OnPropertyChanged(nameof(End));
                }
            };
        }
    }
}
