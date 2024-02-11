﻿using CommunityToolkit.Mvvm.ComponentModel;
using NodeEditor.Model;
using System.Collections.Generic;

namespace NodeEditor.Mvvm;

[ObservableObject]
public partial class EditorViewModel : INodeTemplatesHost, IEditor
{
    [ObservableProperty] private INodeSerializer? _serializer;
    [ObservableProperty] private INodeFactory? _factory;
    [ObservableProperty] private IList<INodeTemplate>? _templates;
    [ObservableProperty] private IDrawingNode? _drawing;
}
