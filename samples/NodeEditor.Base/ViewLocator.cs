using Avalonia.Controls;
using Avalonia.Controls.Templates;
using NodeEditorDemo.ViewModels;
using System;

namespace NodeEditorDemo;

public class ViewLocator : IDataTemplate
{
    public Control Build(object? data)
    {
        string? name = data?.GetType().FullName?.Replace("ViewModel", "View");
        Type? type = name is null ? null : Type.GetType(name);

        if (type != null) {
            return (Control)Activator.CreateInstance(type)!;
        }
        else {
            return new TextBlock { Text = "Not Found: " + name };
        }
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}
