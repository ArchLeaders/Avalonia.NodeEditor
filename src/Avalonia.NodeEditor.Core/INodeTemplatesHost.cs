namespace Avalonia.NodeEditor.Core;

public interface INodeTemplatesHost
{
    IList<INodeTemplate>? Templates { get; set; }
}
